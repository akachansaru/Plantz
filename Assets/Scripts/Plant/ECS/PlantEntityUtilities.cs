using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public static class PlantEntityUtilities
{
    public static Entity CreateRootEmpty(EntityManager entityManager, Biomes biomeType, float3 position, int index)
    {
        Entity root = entityManager.CreateEntity();
        entityManager.AddComponent(root, typeof(EmptyComponentTag));

        #region Taxonomy Info
        TaxonomyECS taxonomy = new TaxonomyECS(biomeType);

        // Family
        entityManager.AddComponentData(root, new FamilyName { Value = taxonomy.Species.Genus.Family.FamilyNum });
        entityManager.AddComponentData(root, new BranchComponent { CurrStep = 0, BranchPattern = taxonomy.Species.Genus.Family.BranchPattern });

        // Genus
        entityManager.AddComponentData(root, new GenusName { Value = taxonomy.Species.Genus.GenusNum });
        entityManager.AddComponentData(root, new StemGrowthTimeComponent { Value = taxonomy.Species.Genus.TimeBetweenPlantGrowth, CurrTime = 0f });
        entityManager.AddComponentData(root, new MaxPlantSizeComponent { Value = taxonomy.Species.Genus.PlantMaxSize, IsMaxSize = false });

        entityManager.AddComponentData(root, new LeavesPerNodeComp { Value = taxonomy.Species.Genus.LeavesPerNode });
        entityManager.AddComponentData(root, new NodesBetweenLeavesComp { Value = taxonomy.Species.Genus.NodesBetweenLeaves });
        entityManager.AddComponentData(root, new LeafInitialSizeComp { Value = taxonomy.Species.Genus.LeafInitialSize });
        entityManager.AddComponentData(root, new LeafMaxSizeComp { Value = taxonomy.Species.Genus.LeafMaxSize });
        entityManager.AddComponentData(root, new WidenLeafTimeComponent { Value = taxonomy.Species.Genus.WidenLeafTime, CurrTime = 0f });

        entityManager.AddComponentData(root, new FlowersPerNodeComp { Value = taxonomy.Species.Genus.FlowersPerNode });
        entityManager.AddComponentData(root, new NodesBetweenFlowersComp { Value = taxonomy.Species.Genus.NodesBetweenFlowers });
        entityManager.AddComponentData(root, new FlowerInitialSizeComp { Value = taxonomy.Species.Genus.FlowerInitialSize });
        entityManager.AddComponentData(root, new FlowerMaxSizeComp { Value = taxonomy.Species.Genus.FlowerMaxSize });
        entityManager.AddComponentData(root, new WidenFlowerTimeComponent { Value = taxonomy.Species.Genus.WidenFlowerTime, CurrTime = 0f });

        // Species
        entityManager.AddComponentData(root, new SpeciesName { Value = taxonomy.Species.SpeciesNum });
        entityManager.AddComponentData(root, new RarityComponent { Value = taxonomy.Species.Rarity });
        entityManager.AddComponentData(root, new WidenStemTimeComponent { Value = taxonomy.Species.WidenStemTime, CurrTime = 0f });
        entityManager.AddComponentData(root, new InitialStemWidthComp { Value = taxonomy.Species.InitialStemWidth });
        entityManager.AddComponentData(root, new MaxStemWidthComp { Value = taxonomy.Species.MaxStemWidth, IsMaxSize = false });
        entityManager.AddComponentData(root, new StemWidenPercentComp { Value = taxonomy.Species.StemWidenPercent });
        entityManager.AddComponentData(root, new BranchRotationComponent { Value = taxonomy.Species.BranchRotation });
        entityManager.AddComponentData(root, new TrunkRotationComp { Value = taxonomy.Species.TrunkRotationX });
        entityManager.AddComponentData(root, new InternodesComp { Value = taxonomy.Species.Internodes });
        entityManager.AddComponentData(root, new BranchesPerCycleComp { Value = taxonomy.Species.BranchesPerCycle });
        DynamicBuffer<Biomes> nativeBiomes = entityManager.AddBuffer<BiomesBufferElement>(root).Reinterpret<Biomes>();
        foreach (Biomes biome in taxonomy.Species.NativeBiomes)
        {
            nativeBiomes.Add(biome);
        }
        #endregion

        entityManager.AddComponentData(root, new NumStepsGrownComp { Value = 0 }); // Number of times the plant has grown. Not the same as age or size

        entityManager.AddComponent(root, typeof(LocalToWorld)); // Needs this or the children won't take on the transform
        entityManager.AddComponentData(root, new Translation { Value = position });
        entityManager.AddComponentData(root, new Rotation { Value = quaternion.identity });
        entityManager.AddComponentData(root, new NonUniformScale { Value = new float3(1, 1, 1) });
        entityManager.AddComponentData(root, new RootComponent { Value = root }); // Add to base root to give info to all children empties

        entityManager.AddComponent(root, typeof(StemCountComponent)); // Only on root
        entityManager.AddComponent(root, typeof(GrowingStemTag));

        entityManager.AddComponentData(root, new IDComponent { ID = 'E', num = index });
        entityManager.SetName(root, "Root empty: " + "E" + index);
        return root;
    }

    public static Entity CreateBaseStem(EntityManager entityManager, Entity parent, int index)
    {
        Entity stem = entityManager.Instantiate(PrefabToEntity.stemEntity);

        entityManager.AddComponentData(stem, new Parent { Value = parent });
        entityManager.AddComponent(stem, typeof(LocalToParent)); // This makes the Translation and scale relative to the Parent

        entityManager.AddComponent<StemTag>(stem);
        entityManager.AddComponent(stem, typeof(BaseStemTag));
        entityManager.AddComponentData(stem, new RootComponent { Value = parent });
        entityManager.SetComponentData(stem, new NonUniformScale
        {
            Value = new float3(entityManager.GetComponentData<InitialStemWidthComp>(parent).Value.value, entityManager.GetComponentData<InitialStemWidthComp>(parent).Value.value, entityManager.GetComponentData<InitialStemWidthComp>(parent).Value.value)
        });

        entityManager.AddComponentData(stem, new IDComponent { ID = 'P', num = index });
        entityManager.SetName(stem, "Base Stem: " + "P" + index);
        return stem;
    }

    public static Entity CreateEmpty(EntityManager entityManager, Entity parent)
    {
        Entity empty = entityManager.CreateEntity();
        entityManager.AddComponent(empty, typeof(LocalToWorld)); // Needs this or the children won't take on the transform
        entityManager.AddComponent(empty, typeof(LocalToParent)); // This makes the Translation and scale relative to the Parent

        entityManager.AddComponent(empty, typeof(GrowingStemTag));
        entityManager.AddComponent(empty, typeof(EmptyComponentTag));

        entityManager.AddComponentData(empty, new NonUniformScale { Value = new float3(1, 1, 1) });

        entityManager.AddComponentData(empty, new Parent { Value = parent });

        entityManager.AddComponentData(empty, entityManager.GetComponentData<BranchComponent>(parent));

        entityManager.AddComponentData(empty, new TrunkRotationComp
        {
            Value = entityManager.GetComponentData<TrunkRotationComp>(parent).Value
        });

        // All stems need access to this
        entityManager.AddComponentData(empty, new RootComponent
        {
            Value = entityManager.HasComponent<Parent>(parent)
            ? entityManager.GetComponentData<RootComponent>(entityManager.GetComponentData<Parent>(parent).Value).Value // Same root as parent
            : parent // If the parent has no parent then that parent is the root
        });

        Entity baseStem = entityManager.GetBuffer<Child>(entityManager.GetComponentData<RootComponent>(empty).Value)[0].Value;
        float height = entityManager.GetComponentData<NonUniformScale>(baseStem).Value.y;
        entityManager.AddComponentData(empty, new Translation
        {
            Value = new float3(0, entityManager.GetComponentData<BranchComponent>(empty).CurrStep * height, 0)
        });

        int plantNum = entityManager.GetComponentData<IDComponent>(parent).num;
        entityManager.AddComponentData(empty, new IDComponent
        {
            ID = 'E',
            num = plantNum
        });
        entityManager.SetName(empty, "E" + plantNum);
        return empty;
    }

    public static float3[] GenerateCompPositions(int compsPerNode)
    {
        float3[] positions = new float3[compsPerNode];
        switch (compsPerNode)
        {
            case 1:
                positions[0] = new float3(0, 0, 1);
                break;
            case 2:
                positions[0] = new float3(0, 0, 1);
                positions[1] = new float3(0, 0, -1);
                break;
            case 3:
                positions[0] = new float3(0, 0, 1);
                positions[1] = new float3(0, 0, -1);
                positions[2] = new float3(1, 0, 0);
                break;
            case 4:
                positions[0] = new float3(0, 0, 1);
                positions[1] = new float3(0, 0, -1);
                positions[2] = new float3(1, 0, 0);
                positions[3] = new float3(-1, 0, 0);
                break;
            default:
                break;
        }
        return positions;
    }

    public static quaternion CalculateRotation(float3 position)
    {
        // No rotation for x, around y and z for y, around y for z
        float3 rotation = float3.zero;
        if (position.x < 0)
        {
            rotation = new float3(0, 180, 0);
        }
        else if (position.y < 0)
        {
            rotation = new float3(0, 0, -90);
        }
        else if (position.y > 0)
        {
            rotation = new float3(0, 0, 90);
        }
        else if (position.z < 0)
        {
            rotation = new float3(0, 90, 0);
        }
        else if (position.z > 0)
        {
            rotation = new float3(0, -90, 0);
        }
        return quaternion.Euler(rotation);
    }
}
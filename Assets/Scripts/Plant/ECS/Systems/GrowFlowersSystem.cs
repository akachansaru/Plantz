using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

public class GrowFlowersSystem : SystemBase
{
    private EntityManager entityManager;

    protected override void OnCreate()
    {
        base.OnCreate();
        entityManager = EntityManager;
    }

    protected override void OnUpdate()
    {
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<GrowFlowersTag>(), ComponentType.Exclude<LoadTag>());
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

        foreach (Entity stem in entities)
        {
            Entity root = entityManager.GetComponentData<RootComponent>(stem).Value;
            float3 initScale = entityManager.GetComponentData<FlowerInitialSizeComp>(root).Value.value * ConstantValues.PlantConsts.FlowerPrefabScale.ToVector3();
            int flowersPerNode = entityManager.GetComponentData<FlowersPerNodeComp>(root).Value;
            float3[] flowerPositions = PlantEntityUtilities.GenerateCompPositions(flowersPerNode);

            for (int l = 0; l < flowersPerNode; l++)
            {
                Entity newFlower = entityManager.Instantiate(PrefabToEntity.flowerEntity);
                entityManager.AddComponent<FlowerTag>(newFlower);
                entityManager.AddComponentData(newFlower, new RootComponent { Value = root });
                entityManager.AddComponentData(newFlower, new IsFlowerMaxSizeComp { Value = false });

                entityManager.AddComponentData(newFlower, new Parent { Value = stem });
                entityManager.AddComponent(newFlower, typeof(LocalToParent)); // This makes the Translation and scale relative to the Parent

                float3 position = flowerPositions[l];

                // Set the initial flower size
                entityManager.AddComponentData(newFlower, new NonUniformScale
                {
                    Value = initScale
                });

                entityManager.AddComponentData(newFlower, new Translation
                {
                    Value = new float3(position.x + (-.5f + (initScale.x / 2)) * math.normalize(position).x,
                        position.y + (-.5f + (initScale.x / 2)) * math.normalize(position).y,
                        position.z + (-.5f + (initScale.x / 2)) * math.normalize(position).z)
                });

                entityManager.AddComponentData(newFlower, new Rotation
                {
                    Value = PlantEntityUtilities.CalculateRotation(position)
                });

                int plantNum = entityManager.GetComponentData<IDComponent>(stem).num;
                entityManager.AddComponentData(newFlower, new IDComponent
                {
                    ID = 'L',
                    num = plantNum
                });
                entityManager.SetName(newFlower, "F" + plantNum);
            }

            entityManager.RemoveComponent(stem, typeof(GrowFlowersTag));
        }

        entities.Dispose();
    }
}

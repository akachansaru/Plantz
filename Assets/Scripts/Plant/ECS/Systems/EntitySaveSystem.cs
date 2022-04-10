using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;
using System.Collections.Generic;
using System.Linq;

public class EntitySaveSystem : SystemBase
{
    private EntityManager entityManager;

    protected override void OnCreate()
    {
        base.OnCreate();
        entityManager = EntityManager;
    }

    [BurstCompile] // Don't know if this is doing anything here
    protected override void OnUpdate()
    {
        // look at each entity and save all of its comps to new EntityStem object
        // Then save that object to the plant it belongs to
        if (EntitySaveTest.readyToSave)
        {
            // Create list of all EntityPlants
            EntityQuery numPlantQuery = GetEntityQuery(ComponentType.ReadOnly<MaxPlantSizeComponent>()); // All plants. MaxSizeComp is only on the base
            NativeArray<Entity> plantEntities = numPlantQuery.ToEntityArray(Allocator.TempJob);
            List<EntityPlant> entityPlants = (from Entity entity in plantEntities select new EntityPlant()).ToList();
            plantEntities.Dispose();

            EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<IDComponent>()); // All stem comps - empty and not
            NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

            foreach (Entity entity in entities)
            {
                entityManager.AddComponentData(entity, new SaveIDComponent
                {
                    Self = entity.Index,
                    Parent = entityManager.HasComponent<Parent>(entity) ? entityManager.GetComponentData<Parent>(entity).Value.Index : -999,
                    Root = entityManager.GetComponentData<RootComponent>(entity).Value.Index,
                    IsEmpty = entityManager.HasComponent<EmptyComponentTag>(entity)
                });

                EntityStem entityStem = ConvertEntity(entity);
                int index = entityManager.GetComponentData<IDComponent>(entity).num;
                entityPlants[index].stems.Add(entityStem);
            }

            entities.Dispose();
            EntitySaveTest.readyToSave = false;
            GlobalControl.Instance.savedValues.EntityPlants = entityPlants;
            GlobalControl.Instance.Save();
        }
    }

    private EntityStem ConvertEntity(Entity entity) // Will need to have a base class for stem, flower, leaf, etc
    {
        // entity could be stem or empty or later leaf, flower etc.
        // Save all comps in entity to a new EntityStem object
        EntityStem entityStem = new EntityStem
        {
            Name = entityManager.GetName(entity),
            PlantID = entityManager.GetComponentData<IDComponent>(entity).num,
            TypeID = entityManager.GetComponentData<IDComponent>(entity).ID,

            Position = entityManager.GetComponentData<Translation>(entity).Value,
            Rotation = entityManager.GetComponentData<Rotation>(entity).Value,
            Scale = entityManager.GetComponentData<NonUniformScale>(entity).Value,

            SaveID = entityManager.GetComponentData<SaveIDComponent>(entity).Self,
            ParentID = entityManager.GetComponentData<SaveIDComponent>(entity).Parent,
            RootID = entityManager.GetComponentData<SaveIDComponent>(entity).Root,
            IsEmpty = entityManager.GetComponentData<SaveIDComponent>(entity).IsEmpty,

            CurrBranchPatternStep = entityManager.HasComponent<BranchComponent>(entity) ? entityManager.GetComponentData<BranchComponent>(entity).CurrStep : -999,
            BranchPattern = entityManager.HasComponent<BranchComponent>(entity) ? entityManager.GetComponentData<BranchComponent>(entity).BranchPattern : BranchPatterns.Alternate,
            BranchAngle = entityManager.HasComponent<BranchRotationComponent>(entity) ? entityManager.GetComponentData<BranchRotationComponent>(entity).Value : -999,

            HasGrowingStemTag = entityManager.HasComponent<GrowingStemTag>(entity),
            HasGrowNormalTag = entityManager.HasComponent<GrowNormalTag>(entity),
            HasGrowBranchTag = entityManager.HasComponent<GrowBranchTag>(entity),
            HasGrowContinueTag = entityManager.HasComponent<GrowContinueTag>(entity),

            IsRoot = entityManager.HasComponent<MaxPlantSizeComponent>(entity),
            IsBaseStem = entityManager.HasComponent<BaseStemTag>(entity),
            MaxSize = entityManager.HasComponent<MaxPlantSizeComponent>(entity) ? entityManager.GetComponentData<MaxPlantSizeComponent>(entity).Value : new StatECS(true),
            IsMaxSize = entityManager.HasComponent<MaxPlantSizeComponent>(entity) ? entityManager.GetComponentData<MaxPlantSizeComponent>(entity).IsMaxSize : true, // probably don't want this on every one
            StemCount = entityManager.HasComponent<StemCountComponent>(entity) ? entityManager.GetComponentData<StemCountComponent>(entity).Value : -999,

            StemGrowthRate = entityManager.HasComponent<StemGrowthTimeComponent>(entity) ? entityManager.GetComponentData<StemGrowthTimeComponent>(entity).Value : new StatECS(true),
            StemGrowthRateCurrTime = entityManager.HasComponent<StemGrowthTimeComponent>(entity) ? entityManager.GetComponentData<StemGrowthTimeComponent>(entity).CurrTime : -999,
            WidenTime = entityManager.HasComponent<WidenStemTimeComponent>(entity) ? entityManager.GetComponentData<WidenStemTimeComponent>(entity).Value : new StatECS(true), // only on root
            WidenCurrTime = entityManager.HasComponent<WidenStemTimeComponent>(entity) ? entityManager.GetComponentData<WidenStemTimeComponent>(entity).CurrTime : -999,
        };

        return entityStem;
    }
}

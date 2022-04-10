using Unity.Entities;
using Unity.Collections;
using Unity.Burst;

public class MaxPlantSizeSystem : SystemBase
{
    private EntityManager entityManager;

    private int totNumPlants = 1000; // Temp for testing

    protected override void OnCreate()
    {
        base.OnCreate();
        entityManager = EntityManager;
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        // Count how many stems are in each plant
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<StemTag>(), ComponentType.Exclude<LoadTag>());
        NativeArray<Entity> stemEntities = query.ToEntityArray(Allocator.TempJob);

        // Loop over this query and save each entity that has the same id in the corresponding index of plants
        NativeArray<int> plants = new NativeArray<int>(totNumPlants, Allocator.TempJob);// This should be a buffer so I don't have to know how many plants there are
        foreach (Entity entity in stemEntities)
        {
            int plantNum = entityManager.GetComponentData<IDComponent>(entity).num;
            plants[plantNum]++;
        }
        stemEntities.Dispose();
        
        EntityQuery numPlantQuery = GetEntityQuery(ComponentType.ReadOnly<MaxPlantSizeComponent>()); // All plants. MaxSizeComp is only on the base
        NativeArray<Entity> plantEntities = numPlantQuery.ToEntityArray(Allocator.TempJob);
        foreach(Entity entity in plantEntities)
        {
            // entityManager.GetComponentData<Child>(entity); // **** Find a way for this to work. Need to get all children of the base empty (recursive)
            MaxPlantSizeComponent maxSizeComponent = entityManager.GetComponentData<MaxPlantSizeComponent>(entity);
            if (!maxSizeComponent.IsMaxSize)
            {
                int currentSize = plants[entityManager.GetComponentData<IDComponent>(entity).num];
                entityManager.SetComponentData(entity, new StemCountComponent { Value = currentSize });
                //Debug.Log(entityManager.GetComponentData<IDComponent>(entity).num + ": " + currentSize);
                if (currentSize >= maxSizeComponent.Value.value) // See if I can estimate the next num of comps after growing. stop if it will be too large
                {
                    entityManager.SetComponentData(entity, new MaxPlantSizeComponent
                    {
                        Value = entityManager.GetComponentData<MaxPlantSizeComponent>(entity).Value,
                        IsMaxSize = true
                    });
                }
            }
        }
        plantEntities.Dispose();
        plants.Dispose();
    }
}

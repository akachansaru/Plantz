using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

public class WidenStemSystem : SystemBase
{
    private EntityManager entityManager;

    protected override void OnCreate()
    {
        base.OnCreate();
        entityManager = EntityManager;
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        EntityQuery numPlantQuery = GetEntityQuery(ComponentType.ReadOnly<StemTag>(), ComponentType.Exclude<LoadTag>());
        NativeArray<Entity> plantEntities = numPlantQuery.ToEntityArray(Allocator.TempJob);

        foreach (Entity entity in plantEntities)
        {
            Entity root = entityManager.GetComponentData<RootComponent>(entity).Value;

            if (!entityManager.GetComponentData<MaxStemWidthComp>(root).IsMaxSize)
            {
                StatECS widenTime = entityManager.GetComponentData<WidenStemTimeComponent>(root).Value;
                float currTime = entityManager.GetComponentData<WidenStemTimeComponent>(root).CurrTime;
                float widenAmt = entityManager.GetComponentData<StemWidenPercentComp>(root).Value.value * 
                                 entityManager.GetComponentData<InitialStemWidthComp>(root).Value.value; // This is inconsistant with how the leaves widen

                float3 newWidth = new float3(
                           entityManager.GetComponentData<NonUniformScale>(entity).Value.x + widenAmt,
                           entityManager.GetComponentData<NonUniformScale>(entity).Value.y,
                           entityManager.GetComponentData<NonUniformScale>(entity).Value.z + widenAmt);

                // Once the base stem has reached max size none of the other stems should widen anymore
                if (entityManager.HasComponent<BaseStemTag>(entity))
                {
                    if (newWidth.x >= entityManager.GetComponentData<MaxStemWidthComp>(root).Value.value)
                    {
                        entityManager.SetComponentData(root, new MaxStemWidthComp
                        {
                            Value = entityManager.GetComponentData<MaxStemWidthComp>(root).Value,
                            IsMaxSize = true
                        });
                    }
                }

                if (currTime >= widenTime.value) // Could add a tag in WidenStemTimerSystem so I don't have to check this (see notes there)
                {
                    entityManager.SetComponentData(entity, new NonUniformScale { Value = newWidth });
                }
            }
        }
        plantEntities.Dispose();
    }
}

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

public class WidenFlowerSystem : SystemBase
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
        EntityQuery numPlantQuery = GetEntityQuery(ComponentType.ReadOnly<FlowerTag>());
        NativeArray<Entity> plantEntities = numPlantQuery.ToEntityArray(Allocator.TempJob);

        foreach (Entity entity in plantEntities)
        {
            Entity root = entityManager.GetComponentData<RootComponent>(entity).Value;

            if (!entityManager.GetComponentData<IsFlowerMaxSizeComp>(entity).Value) // This is set for each flower individually
            {
                StatECS widenTime = entityManager.GetComponentData<WidenFlowerTimeComponent>(root).Value;
                float currTime = entityManager.GetComponentData<WidenFlowerTimeComponent>(root).CurrTime;
                float widenAmt = entityManager.GetComponentData<FlowerMaxSizeComp>(root).Value.value *
                                 entityManager.GetComponentData<FlowerInitialSizeComp>(root).Value.value;                

                if (currTime >= widenTime.value) // Could add a tag in WidenStemTimerSystem so I don't have to check this (see notes there)
                {
                    float3 newWidth = new float3(
                                       entityManager.GetComponentData<NonUniformScale>(entity).Value.x + widenAmt,
                                       entityManager.GetComponentData<NonUniformScale>(entity).Value.y + widenAmt,
                                       entityManager.GetComponentData<NonUniformScale>(entity).Value.z + widenAmt);

                    entityManager.SetComponentData(entity, new NonUniformScale { Value = newWidth });

                    // Adjust position
                    float3 localPosition = entityManager.GetComponentData<Translation>(entity).Value;
                    float3 moveAmt = new float3(
                        widenAmt * math.normalize(localPosition).x,
                        widenAmt * math.normalize(localPosition).y,
                        widenAmt * math.normalize(localPosition).z) / 2;
                    entityManager.SetComponentData(entity, new Translation { Value = localPosition + moveAmt });

                    if (newWidth.x >= entityManager.GetComponentData<FlowerMaxSizeComp>(root).Value.value)
                    {
                        entityManager.SetComponentData(entity, new IsFlowerMaxSizeComp { Value = true });
                        entityManager.AddComponent<HasPollenTag>(entity);
                    }
                }
            }
        }
        plantEntities.Dispose();
    }
}

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

public class WidenLeafSystem : SystemBase
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
        EntityQuery numPlantQuery = GetEntityQuery(ComponentType.ReadOnly<LeafTag>());
        NativeArray<Entity> plantEntities = numPlantQuery.ToEntityArray(Allocator.TempJob);

        foreach (Entity entity in plantEntities)
        {
            Entity root = entityManager.GetComponentData<RootComponent>(entity).Value;

            if (!entityManager.GetComponentData<IsLeafMaxSizeComp>(entity).Value) // This is set for each leaf individually
            {
                StatECS widenTime = entityManager.GetComponentData<WidenLeafTimeComponent>(root).Value;
                float currTime = entityManager.GetComponentData<WidenLeafTimeComponent>(root).CurrTime;
                float widenAmt = entityManager.GetComponentData<LeafMaxSizeComp>(root).Value.value *
                                 entityManager.GetComponentData<LeafInitialSizeComp>(root).Value.value;                

                if (currTime >= widenTime.value) // Could add a tag in WidenStemTimerSystem so I don't have to check this (see notes there)
                {
                    float3 newWidth = new float3(
                                       entityManager.GetComponentData<NonUniformScale>(entity).Value.x + widenAmt,
                                       entityManager.GetComponentData<NonUniformScale>(entity).Value.y,
                                       entityManager.GetComponentData<NonUniformScale>(entity).Value.z + widenAmt);

                    entityManager.SetComponentData(entity, new NonUniformScale { Value = newWidth });

                    // Adjust position
                    float3 localPosition = entityManager.GetComponentData<Translation>(entity).Value;
                    // this is what it would need to be when moving them for the stem growth?
                    //float growthAmt = entityManager.GetComponentData<StemWidenPercentComp>(root).Value.value * 
                    //                  entityManager.GetComponentData<InitialStemWidthComp>(root).Value.value / 2; // Divide by 2 because it's growing evenly on both sides
                    float3 moveAmt = new float3(
                        widenAmt * math.normalize(localPosition).x,
                        widenAmt * math.normalize(localPosition).y,
                        widenAmt * math.normalize(localPosition).z) / 2;
                    entityManager.SetComponentData(entity, new Translation { Value = localPosition + moveAmt });

                    if (newWidth.x >= entityManager.GetComponentData<LeafMaxSizeComp>(root).Value.value)
                    {
                        entityManager.SetComponentData(entity, new IsLeafMaxSizeComp { Value = true });
                    }
                }
            }
        }
        plantEntities.Dispose();
    }

    // Use this for WidenLeafSystem
    //private void AdjustLeafAndFlowerPositions(List<GameObject> empties)
    //{
    //    float growthAmt = ConstantValues.PlantConsts.StemGrowthPercent / 2; // Divide by 2 because it's growing evenly on both sides

    //    foreach (GameObject empty in empties)
    //    {
    //        foreach (Transform child in empty.transform)
    //        {
    //            if (child.GetComponent<LeafFE>() || child.GetComponent<FlowerFE>())
    //            {
    //                Vector3 moveAmt = new Vector3(
    //                    growthAmt * child.localPosition.normalized.x,
    //                    growthAmt * child.localPosition.normalized.y,
    //                    growthAmt * child.localPosition.normalized.z);
    //                if (child.GetComponent<LeafFE>())
    //                {
    //                    child.GetComponent<LeafFE>().SetPosition(child.localPosition + moveAmt);
    //                }
    //                else
    //                {
    //                    child.GetComponent<FlowerFE>().SetPosition(child.localPosition + moveAmt);
    //                }
    //            }
    //        }
    //    }
    // }
}

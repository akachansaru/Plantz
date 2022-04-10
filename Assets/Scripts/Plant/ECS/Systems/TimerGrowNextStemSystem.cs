using Unity.Entities;
using Unity.Collections;
using Unity.Burst;

public class TimerGrowNextStemSystem : SystemBase
{
    private EntityManager entityManager;
    private float deltaTime;

    protected override void OnCreate()
    {
        base.OnCreate();
        entityManager = EntityManager;
        deltaTime = UnityEngine.Time.deltaTime;
    }

    [BurstCompile]
    protected override void OnUpdate()
    {        
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<StemGrowthTimeComponent>());
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

        foreach(Entity root in entities)
        {
            StatECS growNextTime = entityManager.GetComponentData<StemGrowthTimeComponent>(root).Value;
            float currTime = entityManager.GetComponentData<StemGrowthTimeComponent>(root).CurrTime;

            if (currTime < growNextTime.value)
            {
                // Increment CurrTime
                entityManager.SetComponentData(root, new StemGrowthTimeComponent { Value = growNextTime, CurrTime = currTime + deltaTime });
            }
            else
            {
                // Reset CurrTime
                entityManager.SetComponentData(root, new StemGrowthTimeComponent { Value = growNextTime, CurrTime = 0f });
                // Could loop over all StemComps here and add a StemGrowthTimeComponent tag to avoid checking all StemComps in GrowNextSystem
            }
        }
        entities.Dispose();
    }
}

using Unity.Entities;
using Unity.Collections;
using Unity.Burst;

public class TimerWidenFlowerSystem : SystemBase
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
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<WidenFlowerTimeComponent>());
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

        foreach(Entity root in entities)
        {
            StatECS widenTime = entityManager.GetComponentData<WidenFlowerTimeComponent>(root).Value;
            float currTime = entityManager.GetComponentData<WidenFlowerTimeComponent>(root).CurrTime;

            if (currTime < widenTime.value)
            {
                // Increment CurrTime
                entityManager.SetComponentData(root, new WidenFlowerTimeComponent { Value = widenTime, CurrTime = currTime + deltaTime });
            }
            else
            {
                // Reset CurrTime
                entityManager.SetComponentData(root, new WidenFlowerTimeComponent { Value = widenTime, CurrTime = 0f });
            }
        }
        entities.Dispose();
    }
}

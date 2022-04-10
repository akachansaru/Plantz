using Unity.Entities;
using Unity.Collections;
using Unity.Burst;

public class TimerWidenLeafSystem : SystemBase
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
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<WidenLeafTimeComponent>());
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

        foreach(Entity root in entities)
        {
            StatECS widenTime = entityManager.GetComponentData<WidenLeafTimeComponent>(root).Value;
            float currTime = entityManager.GetComponentData<WidenLeafTimeComponent>(root).CurrTime;

            if (currTime < widenTime.value)
            {
                // Increment CurrTime
                entityManager.SetComponentData(root, new WidenLeafTimeComponent { Value = widenTime, CurrTime = currTime + deltaTime });
            }
            else
            {
                // Reset CurrTime
                entityManager.SetComponentData(root, new WidenLeafTimeComponent { Value = widenTime, CurrTime = 0f });
            }
        }
        entities.Dispose();
    }
}

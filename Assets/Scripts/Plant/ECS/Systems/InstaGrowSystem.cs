using Unity.Entities;
using Unity.Collections;

public class InstaGrowSystem : SystemBase
{
    private EntityManager entityManager;

    protected override void OnCreate()
    {
        base.OnCreate();
        entityManager = EntityManager;
    }

    protected override void OnUpdate()
    {
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<InstaGrowComp>());
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

        foreach (Entity root in entities)
        {
            InstaGrowComp instaGrowComp = entityManager.GetComponentData<InstaGrowComp>(root);

            if (entityManager.GetComponentData<NumStepsGrownComp>(root).Value >= instaGrowComp.Value)
            {
                // Put things back to normal
                entityManager.RemoveComponent(root, typeof(InstaGrowComp));
                entityManager.AddComponentData(root, new StemGrowthTimeComponent { Value = instaGrowComp.RealGrowthTime });
                entityManager.AddComponentData(root, new WidenStemTimeComponent { Value = instaGrowComp.RealWidenTime });
            }
        }
        entities.Dispose();
    }
}

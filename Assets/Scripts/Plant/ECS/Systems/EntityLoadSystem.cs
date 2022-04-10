using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;

// Not sure if this is any faster than if it weren't in a System and were in LoadEntities.
public class EntityLoadSystem : SystemBase
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
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<LoadTag>());
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

        foreach (Entity entity in entities)
        {
            if (entityManager.HasComponent<Parent>(entity))
            {
                entityManager.AddComponentData(entity, new Parent
                {
                    Value = FindParent(entityManager.GetComponentData<SaveIDComponent>(entity).Parent)
                });
            }

            entityManager.AddComponentData(entity, new RootComponent
            {
                Value = FindParent(entityManager.GetComponentData<SaveIDComponent>(entity).Root)
            });

            entityManager.RemoveComponent(entity, typeof(LoadTag));
        }

        entities.Dispose();
    }

    private Entity FindParent(int parentID)
    {
        return LoadEntities.sortedEntites[parentID];
    }
}

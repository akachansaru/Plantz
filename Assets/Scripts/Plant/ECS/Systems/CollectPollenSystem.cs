using Unity.Entities;
using Unity.Collections;
using Unity.Burst;

public class CollectPollenSystem : SystemBase
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
        EntityQuery numPlantQuery = GetEntityQuery(ComponentType.ReadOnly<HasPollenTag>());
        NativeArray<Entity> plantEntities = numPlantQuery.ToEntityArray(Allocator.TempJob);

        Entity activePlant = Entity.Null; // This will be the plant that the menu is open on

        foreach (Entity entity in plantEntities)
        {
            Entity root = entityManager.GetComponentData<RootComponent>(entity).Value;

            if (entityManager.HasComponent<CollectingPollenTag>(root)) // CollectingPollenTag is added when the CollectPollen button is pressed (GameObjectEntityLink)
            {
                Player.PlayerInstance.InventoryManager.Inventory.InventoryPollenECS.Add(new PollenECS(new TaxonomyECS(root)));
                entityManager.RemoveComponent<HasPollenTag>(entity);
                activePlant = root;
            }
        }

        // This feels a little risky
        if (activePlant != Entity.Null)
        {
            entityManager.RemoveComponent<CollectingPollenTag>(activePlant);
        }

        plantEntities.Dispose();
    }
}

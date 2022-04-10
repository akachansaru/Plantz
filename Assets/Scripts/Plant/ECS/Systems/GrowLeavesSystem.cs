using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Assets.Scripts.Utilities;

public class GrowLeavesSystem : SystemBase
{
    private EntityManager entityManager;

    protected override void OnCreate()
    {
        base.OnCreate();
        entityManager = EntityManager;
    }

    protected override void OnUpdate()
    {
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<GrowLeavesTag>(), ComponentType.Exclude<LoadTag>());
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

        foreach (Entity stem in entities)
        {
            Entity root = entityManager.GetComponentData<RootComponent>(stem).Value;
            float3 initScale = entityManager.GetComponentData<LeafInitialSizeComp>(root).Value.value * ConstantValues.PlantConsts.LeafPrefabScale.ToVector3();
            int leavesPerNode = entityManager.GetComponentData<LeavesPerNodeComp>(root).Value;
            float3[] leafPositions = PlantEntityUtilities.GenerateCompPositions(leavesPerNode);

            for (int l = 0; l < leavesPerNode; l++)
            {
                Entity newLeaf = entityManager.Instantiate(PrefabToEntity.leafEntity);
                entityManager.AddComponent<LeafTag>(newLeaf);
                entityManager.AddComponentData(newLeaf, new RootComponent { Value = root });
                entityManager.AddComponentData(newLeaf, new IsLeafMaxSizeComp { Value = false });

                entityManager.AddComponentData(newLeaf, new Parent { Value = stem });
                entityManager.AddComponent(newLeaf, typeof(LocalToParent)); // This makes the Translation and scale relative to the Parent

                float3 position = leafPositions[l];

                // Set the initial leaf size
                entityManager.AddComponentData(newLeaf, new NonUniformScale
                {
                    Value = initScale
                });

                entityManager.AddComponentData(newLeaf, new Translation
                {
                    Value = new float3(position.x + (-.5f + (initScale.x / 2)) * math.normalize(position).x,
                        position.y + (-.5f + (initScale.x / 2)) * math.normalize(position).y,
                        position.z + (-.5f + (initScale.x / 2)) * math.normalize(position).z)
                });

                entityManager.AddComponentData(newLeaf, new Rotation
                {
                    Value = PlantEntityUtilities.CalculateRotation(position)
                });

                int plantNum = entityManager.GetComponentData<IDComponent>(stem).num;
                entityManager.AddComponentData(newLeaf, new IDComponent
                {
                    ID = 'L',
                    num = plantNum
                });
                entityManager.SetName(newLeaf, "L" + plantNum);
            }

            entityManager.RemoveComponent(stem, typeof(GrowLeavesTag));
        }

        entities.Dispose();
    }
}

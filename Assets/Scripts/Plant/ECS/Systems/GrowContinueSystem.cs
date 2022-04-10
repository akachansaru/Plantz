using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

public class GrowContinueSystem : SystemBase
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
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<GrowContinueTag>(), ComponentType.Exclude<LoadTag>());
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

        foreach (Entity empty in entities)
        {
            Entity newStem = entityManager.Instantiate(PrefabToEntity.stemEntity);

            entityManager.AddComponentData(empty, new BranchComponent
            {
                CurrStep = 0,
                BranchPattern = entityManager.GetComponentData<BranchComponent>(empty).BranchPattern
            });

            entityManager.AddComponentData(newStem, new Parent { Value = empty });

            entityManager.AddComponent(newStem, typeof(LocalToParent)); // This makes the Translation and scale relative to the Parent
            Entity root = entityManager.GetComponentData<RootComponent>(empty).Value;
            // All stems need access to this
            entityManager.AddComponentData(newStem, new RootComponent { Value = root });

            //entityManager.AddComponentData(newStem, new SaveIDComponent
            //{
            //    Self = newStem.Index,
            //    Parent = empty.Index,
            //    Root = entityManager.GetComponentData<RootComponent>(newStem).Value.Index,
            //    IsEmpty = false
            //});

            float height = entityManager.GetComponentData<NonUniformScale>(newStem).Value.y;
            float3 newPos = new float3(0, height, 0);
            entityManager.AddComponentData(newStem, new Translation { Value = newPos });

            // Set the initial stem size
            entityManager.SetComponentData(newStem, new NonUniformScale
            {
                Value = new float3(entityManager.GetComponentData<InitialStemWidthComp>(root).Value.value, height, entityManager.GetComponentData<InitialStemWidthComp>(root).Value.value)
            });

            entityManager.AddComponent<StemTag>(newStem);

            int plantNum = entityManager.GetComponentData<IDComponent>(empty).num;
            entityManager.AddComponentData(newStem, new IDComponent
            {
                ID = 'C',
                num = plantNum
            });
            entityManager.SetName(newStem, "C" + plantNum);

            entityManager.RemoveComponent(empty, typeof(GrowContinueTag));
        }

        entities.Dispose();
    }
}

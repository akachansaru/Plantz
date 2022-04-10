using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

public class GrowNormalSystem : SystemBase
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
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<GrowNormalTag>(), ComponentType.Exclude<LoadTag>());
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

        foreach (Entity empty in entities)
        {
            Entity newStem = entityManager.Instantiate(PrefabToEntity.stemEntity);

            entityManager.AddComponent<StemTag>(newStem);

            // Update the CurrStep on the empty
            entityManager.AddComponentData(empty, new BranchComponent
            {
                CurrStep = entityManager.GetComponentData<BranchComponent>(empty).CurrStep + 1,
                BranchPattern = entityManager.GetComponentData<BranchComponent>(empty).BranchPattern
            });

            Entity root = entityManager.GetComponentData<RootComponent>(empty).Value;
            // All stems need access to this
            entityManager.AddComponentData(newStem, new RootComponent { Value = root });

            //if (entityManager.HasComponent(empty, typeof(Child))) //.GetBuffer<Child>(empty).IsCreated)
            //{
            //    Entity child = entityManager.GetBuffer<Child>(empty)[0].Value;
            //    float originalHeight = 0.2f;

            //    float3 currScale = entityManager.GetComponentData<NonUniformScale>(child).Value;
            //    float amtChanged = originalHeight * entityManager.GetComponentData<BranchComponent>(empty).CurrStep;

            //    float3 currTranslation = entityManager.GetComponentData<Translation>(child).Value;
            //    entityManager.AddComponentData(child, new NonUniformScale
            //    {
            //        Value = new float3(currScale.x, amtChanged, currScale.z) // 0.2 would need to be original scale
            //    });

            //    // move it the correct amount
            //    entityManager.AddComponentData(child, new Translation
            //    {
            //        Value = new float3(currTranslation.x, amtChanged, currTranslation.z)
            //    });
            //}

            entityManager.AddComponentData(newStem, new Parent { Value = empty });
            entityManager.AddComponent(newStem, typeof(LocalToParent)); // This makes the Translation and scale relative to the Parent

            Entity baseStem = entityManager.GetBuffer<Child>(entityManager.GetComponentData<RootComponent>(empty).Value)[0].Value;
            float height = entityManager.GetComponentData<NonUniformScale>(baseStem).Value.y;
            entityManager.AddComponentData(newStem, new Translation
            {
                Value = new float3(0, entityManager.GetComponentData<BranchComponent>(empty).CurrStep * height, 0)
            });

            // Set the initial stem size
            entityManager.SetComponentData(newStem, new NonUniformScale 
            {
                Value = new float3(entityManager.GetComponentData<InitialStemWidthComp>(root).Value.value, height, entityManager.GetComponentData<InitialStemWidthComp>(root).Value.value)
            });

            entityManager.AddComponentData(newStem, new Rotation { Value = quaternion.identity });

            int plantNum = entityManager.GetComponentData<IDComponent>(empty).num;
            entityManager.AddComponentData(newStem, new IDComponent
            {
                ID = 'S',
                num = plantNum
            });
            entityManager.SetName(newStem, "S" + plantNum);

            entityManager.RemoveComponent(empty, typeof(GrowNormalTag));

            // Add a tag so leaves will grow if the branch is at the right steo
            if (entityManager.GetComponentData<BranchComponent>(empty).CurrStep == entityManager.GetComponentData<NodesBetweenLeavesComp>(root).Value)
            {
                entityManager.AddComponent(newStem, typeof(GrowLeavesTag));
            }

            // Add a tag so flowers will grow if the branch is at the right steo
            if (entityManager.GetComponentData<BranchComponent>(empty).CurrStep == entityManager.GetComponentData<NodesBetweenFlowersComp>(root).Value)
            {
                entityManager.AddComponent(newStem, typeof(GrowFlowersTag));
            }
        }

        entities.Dispose();
    }
}

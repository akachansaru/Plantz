using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Assets.Scripts.Utilities;

public class GrowNextSystem : SystemBase
{
    private EntityManager entityManager;

    protected override void OnCreate()
    {
        base.OnCreate();
        entityManager = EntityManager;
    }

    protected override void OnUpdate()
    {
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<GrowingStemTag>(), ComponentType.Exclude<LoadTag>());
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

        foreach (Entity empty in entities)
        {
            Entity root = entityManager.GetComponentData<RootComponent>(empty).Value;
            float growNextTime = entityManager.GetComponentData<StemGrowthTimeComponent>(root).Value.value;
            float currTime = entityManager.GetComponentData<StemGrowthTimeComponent>(root).CurrTime;

            if (currTime >= growNextTime) // Could add a tag in GrowNextTimerSystem so I don't have to check this (see notes there)
            {
                entityManager.SetComponentData(root, new NumStepsGrownComp { Value = entityManager.GetComponentData<NumStepsGrownComp>(root).Value + 1 });

                if (!entityManager.GetComponentData<MaxPlantSizeComponent>(entityManager.GetComponentData<RootComponent>(empty).Value).IsMaxSize)
                {
                    if (entityManager.GetComponentData<BranchComponent>(empty).CurrStep < entityManager.GetComponentData<InternodesComp>(root).Value)
                    {
                        entityManager.AddComponent(empty, typeof(GrowNormalTag));
                    }
                    else
                    {
                        int branchesPerCycle = entityManager.GetComponentData<BranchesPerCycleComp>(root).Value;
                        switch (entityManager.GetComponentData<BranchComponent>(empty).BranchPattern)
                        {
                            case BranchPatterns.Alternate:
                                Entity branchEmpty = GrowBranch(empty, 0);
                                Entity continueEmpty = GrowContinue(empty, 360 / branchesPerCycle);

                                entityManager.AddComponent(branchEmpty, typeof(GrowBranchTag));
                                entityManager.AddComponent(continueEmpty, typeof(GrowContinueTag));
                                break;
                            case BranchPatterns.Opposite:
                                Entity branchEmptyOpp = GrowBranch(empty, 0);
                                Entity branchEmptyOpp1 = GrowBranch(empty, 180);
                                Entity continueEmptyOpp = GrowContinue(empty, 360 / branchesPerCycle);

                                entityManager.AddComponent(branchEmptyOpp, typeof(GrowBranchTag));
                                entityManager.AddComponent(branchEmptyOpp1, typeof(GrowBranchTag));
                                entityManager.AddComponent(continueEmptyOpp, typeof(GrowContinueTag));
                                break;
                            case BranchPatterns.Whorled:
                                for (int b = 0; b < branchesPerCycle; b++)
                                {
                                    Entity branchEmptyWho = GrowBranch(empty, b * 360 / branchesPerCycle);
                                    entityManager.AddComponent(branchEmptyWho, typeof(GrowBranchTag));
                                }
                                Entity continueEmptyWho = GrowContinue(empty, 0);
                                entityManager.AddComponent(continueEmptyWho, typeof(GrowContinueTag));
                                break;
                            default:
                                break;
                        }
                        entityManager.RemoveComponent(empty, typeof(GrowingStemTag));
                        entityManager.RemoveComponent(empty, typeof(GrowNormalTag));
                    }
                }
            }
        }

        entities.Dispose();
    }

    private Entity GrowBranch(Entity empty, float incrementAngle)
    {
        Entity branchEmpty = PlantEntityUtilities.CreateEmpty(entityManager, empty);
        float3 branchRotation = entityManager.GetComponentData<BranchRotationComponent>(empty).Value;

        entityManager.AddComponentData(branchEmpty, new BranchRotationComponent
        {
            Value = entityManager.GetComponentData<BranchRotationComponent>(entityManager.GetComponentData<RootComponent>(branchEmpty).Value).Value //IncrementBranchSide(branchRotation, incrementAngle)
        });

        float3 newAngle = IncrementBranchSide(branchRotation, incrementAngle);
        // This angle will affect all children of this empty
        entityManager.AddComponentData(branchEmpty, new Rotation
        {
            Value = quaternion.EulerZYX(
                math.radians(RandomSign.Sign() * newAngle.x), // Gives it a nice bit of random variation
                math.radians(newAngle.y),
                math.radians(newAngle.z))
        });;

        return branchEmpty;
    }

    private Entity GrowContinue(Entity empty, float incrementAngle)
    {
        Entity continueEmpty = PlantEntityUtilities.CreateEmpty(entityManager, empty);
        StatECS trunkRotation = entityManager.GetComponentData<TrunkRotationComp>(empty).Value;

        entityManager.AddComponentData(continueEmpty, new Rotation
        {
            Value = quaternion.EulerZYX(math.radians(trunkRotation.GetValueWithVariance()), math.radians(0), math.radians(0))
        });

        // Don't change the rotation of this trunk, but add it for the next branch that grows
        entityManager.AddComponentData(continueEmpty, new BranchRotationComponent
        {
            Value = IncrementBranchSide(entityManager.GetComponentData<BranchRotationComponent>(empty).Value, incrementAngle)
        });

        return continueEmpty;
    }

    private float3 IncrementBranchSide(float3 branchAngle, float incrementAngle)
    {
        return new float3(branchAngle.x, branchAngle.y + incrementAngle, branchAngle.z);
    }
}

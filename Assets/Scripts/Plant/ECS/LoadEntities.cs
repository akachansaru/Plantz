using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class LoadEntities : MonoBehaviour
{
    private EntityManager entityManager;
    //public List<Entity> allEntities = new List<Entity>();

    public static Entity[] sortedEntites; // Add each entity to index SaveIDComp.Self for faster lookups
    private int numEntities = 0;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (GlobalControl.Instance.savedValues.EntityPlants.Count > 0)
        {
            Debug.Log("Loading plants");

            // Count the number of entities to make sorted array
            foreach (EntityPlant entityPlant1 in GlobalControl.Instance.savedValues.EntityPlants)
            {
                numEntities += entityPlant1.stems.Count;
            }
            sortedEntites = new Entity[numEntities * 2]; // Multiplying by 2 because the Entity IDs are higher than the # of entities for some reason

            // Create an entity for each saved entity. Contains components except Parent. A System is used for assigning parents
            foreach (EntityPlant entityPlant in GlobalControl.Instance.savedValues.EntityPlants)
            {
                LoadPlant(entityPlant);
            }
        }
    }

    private void LoadPlant(EntityPlant entityPlant)
    {
        // Have a bool in the root and in the system it checks if it's true and loads it? Instead of this
        foreach(EntityStem entityStem in entityPlant.stems)
        {
            LoadStem(entityStem);
        }
    }

    private void LoadStem(EntityStem entityStem)
    {
        Entity entity;
        bool isEmpty = entityStem.IsEmpty;
        if (!isEmpty)
        {
            entity = entityManager.Instantiate(PrefabToEntity.stemEntity);
            entityManager.AddComponentData(entity, new NonUniformScale { Value = entityStem.Scale });
            entityManager.AddComponent<StemTag>(entity);
        }
        else
        {
            entity = entityManager.CreateEntity(); //PlantEntityUtilities.CreateEmpty(entityManager, Entity.Null);
            entityManager.AddComponent(entity, typeof(LocalToWorld)); // Needs this or the children won't take on the transform
            entityManager.AddComponent(entity, typeof(EmptyComponentTag));
            entityManager.AddComponentData(entity, new NonUniformScale { Value = new float3(1, 1, 1) });
            entityManager.AddComponentData(entity, new BranchComponent
            {
                CurrStep = entityStem.CurrBranchPatternStep,
                BranchPattern = entityStem.BranchPattern
            });

            entityManager.AddComponentData(entity, new BranchRotationComponent
            {
                Value = entityStem.BranchAngle,
            });
        }

        entityManager.SetName(entity, entityStem.Name);
        entityManager.AddComponentData(entity, new IDComponent { ID = entityStem.TypeID, num = entityStem.PlantID });
        entityManager.AddComponentData(entity, new Translation { Value = entityStem.Position });
        entityManager.AddComponentData(entity, new Rotation { Value = entityStem.Rotation });
        
        if (entityStem.IsRoot)
        {
            entityManager.AddComponentData(entity, new StemGrowthTimeComponent { Value = entityStem.StemGrowthRate, CurrTime = entityStem.StemGrowthRateCurrTime });
            entityManager.AddComponentData(entity, new MaxPlantSizeComponent { Value = entityStem.MaxSize, IsMaxSize = entityStem.IsMaxSize });
            entityManager.AddComponentData(entity, new StemCountComponent { Value = entityStem.StemCount });
            entityManager.AddComponentData(entity, new RootComponent { Value = entity }); // Only add it to the root for now. In EntityLoadSystem will add RootComp to all stems without a RootComp
            entityManager.AddComponentData(entity, new WidenStemTimeComponent { Value = entityStem.WidenTime, CurrTime = entityStem.WidenCurrTime });
        }

        if (entityStem.IsBaseStem)
        {
            entityManager.AddComponent(entity, typeof(BaseStemTag));
        }

        if (entityStem.ParentID >= 0)
        {
            entityManager.AddComponent(entity, typeof(Parent));
            entityManager.AddComponent(entity, typeof(LocalToParent));
            entityManager.AddComponentData(entity, new SaveIDComponent 
            { 
                Self = entityStem.SaveID, 
                Parent = entityStem.ParentID,
                Root = entityStem.RootID,
                IsEmpty = isEmpty 
            });
        }
        else
        {
            entityManager.AddComponentData(entity, new SaveIDComponent 
            { 
                Self = entityStem.SaveID, 
                Parent = -999,
                Root = entityStem.RootID,
                IsEmpty = isEmpty });
        }

        entityManager.AddComponent(entity, typeof(LoadTag)); // This needs to be removed once it is loaded

        if (entityStem.HasGrowingStemTag)
        {
            entityManager.AddComponent(entity, typeof(GrowingStemTag));
        }

        if (entityStem.HasGrowNormalTag)
        {
            entityManager.AddComponent(entity, typeof(GrowNormalTag));
        }

        if (entityStem.HasGrowBranchTag)
        {
            entityManager.AddComponent(entity, typeof(GrowBranchTag));
        }

        if (entityStem.HasGrowContinueTag)
        {
            entityManager.AddComponent(entity, typeof(GrowContinueTag));
        }

        try
        {
            sortedEntites[entityStem.SaveID] = entity;
        }
        catch (Exception e)
        {
            Debug.Log(entityStem.SaveID + "/" + sortedEntites.Length);
            Debug.LogError(e.Message);
        }
        //allEntities.Add(entity);
    }
}

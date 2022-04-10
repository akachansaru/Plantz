using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;

// This is being used for the greenhouse plants right now. Outdoor plants are initiated in InitializePlantsECS
public class PlantLocations : MonoBehaviour
{
    private readonly float stemGrowthTimeInstaGrow = 0.1f; // Time stats will use this to convert for instagrowing

    [SerializeField] private int instaGrowNum = 10;

    public List<Transform> plantLocations;

    private EntityManager entityManager;

    public void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (GlobalControl.Instance.savedValues.EntityPlants.Count == 0) // This will be GreenhousePlants
        {
            int index = 0;
            foreach (Transform plantLocation in plantLocations)
            {
                if (!plantLocation.GetComponent<BenchSegment>().IsOccupied)
                {
                    // Create pot then create entities starting in that
                    GameObject pot = Instantiate(Resources.Load(ConstantValues.Prefabs.PlantECSBase), plantLocation) as GameObject;
                    Vector3 potPos = pot.transform.position;
                    pot.transform.position = new Vector3(potPos.x, (plantLocation.localScale.y + pot.transform.localScale.y) / 2, potPos.z);

                    plantLocation.GetComponent<BenchSegment>().SetPlantOnBench(pot);

                    Biomes randBiome = (Biomes)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Biomes)).Length);
                    Entity rootEmpty = PlantEntityUtilities.CreateRootEmpty(entityManager, randBiome, new float3(pot.transform.position), index);

                    // Instagrow. This is the same as in InitialzePlantsECS. Combine
                    StatECS realGrowthRate = entityManager.GetComponentData<StemGrowthTimeComponent>(rootEmpty).Value;
                    StatECS realWidenTime = entityManager.GetComponentData<WidenStemTimeComponent>(rootEmpty).Value;
                    float instaGrowRatio = stemGrowthTimeInstaGrow / realGrowthRate.value;

                    entityManager.AddComponentData(rootEmpty, new InstaGrowComp
                    {
                        Value = instaGrowNum,
                        RealGrowthTime = realGrowthRate,
                        RealWidenTime = realWidenTime
                    });

                    entityManager.AddComponentData(rootEmpty, new StemGrowthTimeComponent
                    {
                        Value = new StatECS(stemGrowthTimeInstaGrow, stemGrowthTimeInstaGrow)
                    }); // Almost 0 growth rate while it insta grows

                    entityManager.AddComponentData(rootEmpty, new WidenStemTimeComponent
                    {
                        Value = new StatECS(realWidenTime.value * instaGrowRatio, realWidenTime.value * instaGrowRatio)
                    });
                    //////
                    ///
                    Entity baseStem = PlantEntityUtilities.CreateBaseStem(entityManager, rootEmpty, index);

                    pot.GetComponent<GameObjectEntityLink>().BaseEntity = rootEmpty; // So there is a link between entities and GOs

                    index++;
                }
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Creates random plants distributed accross the landscape
/// </summary>
[RequireComponent(typeof(Biome))]
public class InitializePlantsECS : MonoBehaviour
{
    private readonly float stemGrowthTimeInstaGrow = 0.1f; // Time stats will use this to convert for instagrowing

    private Biome biome;
    [SerializeField] private int numPlants = 0;
    [SerializeField] private float plantSpacingDistance = 2f; // The minimum distance between plants
    [SerializeField] private int instaGrowNum = 10;

    private string listName;
    private List<Plant> saveList;
    private Terrain terrain;
    public Vector3 BiomeEdges { get; private set; }
    private TreeInstance[] treeInstances;

    private EntityManager entityManager;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (gameObject.layer != ConstantValues.Layers.Ground)
        {
            Debug.LogWarning("Changed terrain layer to Ground");
            gameObject.layer = ConstantValues.Layers.Ground;
        }
        biome = GetComponent<Biome>();
        saveList = GetSaveList(biome.BiomeType, out listName);
        terrain = GetComponent<Terrain>();
        treeInstances = terrain.terrainData.treeInstances;
        BiomeEdges = terrain.terrainData.size; // (150, 150, 150)

        if (saveList.Count == 0)
        { // New game
            for (int p = 0; p < numPlants; p++)
            {
                CreatePlant(p);
            }
        }
        else
        {
            //foreach (Plant plant in saveList)
            //{
            //    // This will be the saved plant
            //    PlantUtilities.LoadPlant(plant, Resources.Load(ConstantValues.Prefabs.Plant) as GameObject, transform, plant.Position.ToVector3());
            //}
        }
    }

    /// <summary>
    /// Generates a random position and tests if it will intersect an obstacle. If not, assigns that position to a new plant
    /// and instantiates it.
    /// Tries 10 times to place each plant. If it hits obstacles each time, gives up. Stops iterating the first time there is no obstacle.
    /// </summary>
    private void CreatePlant(int index)
    {
        //for (int i=0; i < 10; i++)
        //{
        Vector3 testPosition = GetRandPosition();
        int layers = ~(1 << ConstantValues.Layers.Ground); // Will raycast against all layers except Ground
        Collider[] colliders = Physics.OverlapSphere(testPosition, plantSpacingDistance, layers, QueryTriggerInteraction.Collide);
        // FIXME: Needs to check if intersecting another plant as well. QueryTriggerInteraction.Collide isn't working
        if (!HitTerrainTree(testPosition) && colliders.Length == 0)
        {
            GameObject pot = Instantiate(Resources.Load(ConstantValues.Prefabs.PlantECSBase), testPosition, Quaternion.identity) as GameObject;

            // This is when the species is applied
            Entity rootEmpty = PlantEntityUtilities.CreateRootEmpty(entityManager, biome.BiomeType, new float3(pot.transform.position), index);

            // Instagrow. This is the same as in PlantLocation. Combine
            StatECS realGrowthRate = entityManager.GetComponentData<StemGrowthTimeComponent>(rootEmpty).Value;
            StatECS realWidenTime = entityManager.GetComponentData<WidenStemTimeComponent>(rootEmpty).Value;
            float instaGrowRatio = stemGrowthTimeInstaGrow / realGrowthRate.value;

            entityManager.AddComponentData(rootEmpty, new InstaGrowComp { 
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

            Entity baseStem = PlantEntityUtilities.CreateBaseStem(entityManager, rootEmpty, index);

            pot.GetComponent<GameObjectEntityLink>().BaseEntity = rootEmpty; // So there is a link between entities and GOs
        }
    }

    private bool HitTerrainTree(Vector3 testPosition)
    {
        bool hitTree = false;
        foreach (TreeInstance treeInstance in treeInstances)
        {
            Vector3 worldTreePos = Vector3.Scale(treeInstance.position, GetComponent<Terrain>().terrainData.size) + transform.position;
            Vector2 treePos2D = new Vector2(worldTreePos.x, worldTreePos.z);
            Vector2 plantPos2D = new Vector2(testPosition.x, testPosition.z);
            if (Vector2.Distance(treePos2D, plantPos2D) < plantSpacingDistance)
            {
                hitTree = true;
                break;
            }
        }
        return hitTree;
    }

    private Vector3 GetRandPosition()
    {
        Vector3 position = new Vector3(UnityEngine.Random.Range(0, BiomeEdges.x), 0, UnityEngine.Random.Range(0, BiomeEdges.z));
        position.y = terrain.SampleHeight(position + transform.position); // Add in position to convert to world coords
        position += transform.position;
        return position;
    }

    private List<Plant> GetSaveList(Biomes biome, out string listName)
    {
        List<Plant> saveList;
        switch (biome)
        {
            case Biomes.Forest:
                saveList = GlobalControl.Instance.savedValues.ForestPlants;
                listName = ConstantValues.SaveLists.Forest;
                break;
            case Biomes.Desert:
                saveList = GlobalControl.Instance.savedValues.DesertPlants;
                listName = ConstantValues.SaveLists.Desert;
                break;
            case Biomes.Swamp:
                saveList = GlobalControl.Instance.savedValues.SwampPlants;
                listName = ConstantValues.SaveLists.Swamp;
                break;
            default:
                saveList = GlobalControl.Instance.savedValues.ForestPlants;
                listName = ConstantValues.SaveLists.Forest;
                Debug.LogError("Biome not found");
                break;
        }
        return saveList;
    }
}

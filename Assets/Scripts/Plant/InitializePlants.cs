using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates random plants distributed accross the landscape
/// </summary>
[RequireComponent(typeof(Biome))]
public class InitializePlants : MonoBehaviour
{
    private Biome biome;
    [SerializeField] private int numPlants = 0;
    [SerializeField] private float plantSpacingDistance = 2f; // The minimum distance between plants
    [SerializeField] private int instaGrowNum = 10;
    //[SerializeField] private PlantPooling plantPooling = null;

    private string listName;
    private List<Plant> saveList;
    private Terrain terrain;
    public Vector3 BiomeEdges { get; private set; }
    private TreeInstance[] treeInstances;

    private void Start()
    {
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
                CreatePlant();
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
    private void CreatePlant() // really tough task
    {
        //for (int i=0; i < 10; i++)
        //{
            Vector3 testPosition = GetRandPosition();
            int layers = ~(1 << ConstantValues.Layers.Ground); // Will raycast against all layers except Ground
            Collider[] colliders = Physics.OverlapSphere(testPosition, plantSpacingDistance, layers, QueryTriggerInteraction.Collide);
            // FIXME: Needs to check if intersecting another plant as well. QueryTriggerInteraction.Collide isn't working
            if (!HitTerrainTree(testPosition) && colliders.Length == 0)
            {
                Plant newPlant = PlantUtilities.CreateRandomPlant(biome.BiomeType, saveList, listName); // This will load a saved plant and name it correctly
                newPlant.InstaGrow(instaGrowNum, 1, 1);
                newPlant.Position = new Vector3Serializable(testPosition);
                PlantUtilities.LoadPlant(newPlant, Resources.Load(ConstantValues.Prefabs.Plant) as GameObject, transform, testPosition);

            //GameObject newPlantGO = Instantiate(Resources.Load("Prefabs/GrownPlants/P0 Greenhouse"), transform) as GameObject;
            //GameObject newPlantGO = plantPooling.GetNextAvailable();
            //Plant newPlant = newPlantGO.GetComponent<PlantFE>().Plant;
            //newPlantGO.transform.localPosition = testPosition;
            //newPlant.Position = new Vector3Serializable(testPosition);
            //saveList.Add(newPlant);

            // Get rid of components that are only needed for greenhouse plants
            //Destroy(newPlantGO.GetComponent<PickUpPlant>());
            //Destroy(newPlantGO.GetComponent<PlantMenu>());

            //newPlantGO.AddComponent<DestroyObject>(); // FIXME: the player has to be looking directly at the bottom of the plant for this to work
            //need to make a larger collider. Probably will need to have tools to switch between so the code will know which interactable to use(collect pollen, destroy, etc.)
            //break;
        }
       // }
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
        Vector3 position = new Vector3(Random.Range(0, BiomeEdges.x), 0, Random.Range(0, BiomeEdges.z));
        position.y = terrain.SampleHeight(position + transform.position); // Add in position to convert to world coords
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

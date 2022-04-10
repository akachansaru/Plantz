using UnityEngine;
using System.Collections.Generic;
using Assets.Resources.Scripts.Utilities;

public class PlantUtilities : MonoBehaviour
{
    /// <summary>
    /// Randomly generates the stem and leaf parameters for a new plant and saves to saveList
    /// </summary>
    /// <param name="saveList"></param>
    /// <returns></returns>
    public static Plant CreateRandomPlant(Biomes biome, List<Plant> saveList, string listName)
    {
        Plant plant = new Plant(new Taxonomy(biome), saveList, listName);
        saveList.Add(plant);

        return plant;
    }

    /// <summary>
    /// Takes the saved plant and turns it into GameObjects
    /// </summary>
    /// <param name="p"></param>
    /// <param name="blankPlant"></param>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static GameObject LoadPlant(Plant plant, GameObject blankPlant)
    {
        return LoadPlantHelper(plant, blankPlant, null, Vector3.zero);
    }

    public static GameObject LoadPlant(Plant plant, GameObject blankPlant, Transform transform, Vector3 localPosition)
    {
        return LoadPlantHelper(plant, blankPlant, transform, localPosition);
    }

    private static GameObject LoadPlantHelper(Plant plant, GameObject blankPlant, Transform transform, Vector3 localPosition)
    {
        GameObject plantGO = Instantiate(blankPlant, transform);
        plantGO.transform.localPosition = localPosition;
        plantGO.GetComponent<PlantFE>().Plant = plant;
        plantGO.name = plant.PlantID;
        TransformUtilities.SetGlobalScale(plantGO.transform, plantGO.GetComponent<PlantFE>().Plant.Pot.PotSize.ToVector3());

        // Instantiate each stem componenent
        foreach (List<Stem> branch in plant.Stems)
        {
            LoadBranch(branch, plantGO.transform);
        }
        return plantGO;
    }

    private static void LoadBranch(List<Stem> branch, Transform plantT)
    {
        foreach (Stem s in branch)
        {
            LoadStem(s, plantT);
        }
    }

    private static Transform branchBaseT;
    private static void LoadStem(Stem stem, Transform plantT)
    {
        GameObject stemGO = Instantiate(Resources.Load(ConstantValues.Prefabs.Stem)) as GameObject;
        stemGO.GetComponent<StemFE>().Stem = stem;

        // If it has no parent than it is the first branch. Save it so the other stems in the branch can attach
        if (stem.Parent == plantT.name)
        {
            stemGO.transform.SetParent(plantT);
            branchBaseT = stemGO.transform;
        }
        else
        {
            // Still have to find the parent by name if it's the start of a branch
            if (stem.Branch.IsBranchBase)
            {
                stemGO.transform.SetParent(GameObject.Find(stem.Parent).transform);
                branchBaseT = stemGO.transform;
            }
            else
            {
                stemGO.transform.SetParent(branchBaseT);
            }
        }

        stemGO.name = stem.StemID;
        stemGO.transform.localPosition = stem.Position.ToVector3();

        Quaternion rotation = Quaternion.identity;
        rotation.eulerAngles = stem.Rotation.ToVector3();
        stemGO.transform.localRotation = rotation;

        TransformUtilities.SetGlobalScale(stemGO.transform, stem.GlobalScale.ToVector3());

        // Move the first stem comp to be about one of it's height below the top of the pot
        if (stem.Parent == plantT.name)
        {
            stemGO.transform.localPosition = new Vector3(0, 0.5f - stemGO.transform.localScale.y, 0);
        }

        // these nulls might need to be > 0
        if (stem.Leaves != null)
        {
            foreach (Leaf leaf in stem.Leaves)
            {
                GameObject gameObject = Instantiate(Resources.Load(ConstantValues.Prefabs.Leaf)) as GameObject;
                gameObject.GetComponent<LeafFE>().Load(leaf, false, plantT.GetComponent<PlantFE>().LeafGOs);

                //LoadComp(leaf, plantT.gameObject, ConstantValues.Prefabs.Leaf, plantT.GetComponent<PlantFE>().LeafGOs);
            }
        }

        if (stem.Flowers != null)
        {
            foreach (Flower flower in stem.Flowers)
            {
                // This was moved from a deleted method (LoadFlower). Will need tweaking here
                //if (flowerGO.transform.parent.GetComponent<StemFE>().Stem.Branch.YearGrown == GlobalControl.Instance.savedValues.Year)
                //{
                //    plantGO.GetComponent<PlantFE>().ThisYearsFlowerGOs.Add(flowerGO);
                //}
                //else
                //{
                //    plantGO.GetComponent<PlantFE>().LastYearsFlowerGOs.Add(flowerGO);
                //}
                GameObject gameObject = Instantiate(Resources.Load(ConstantValues.Prefabs.Flower)) as GameObject;
                gameObject.GetComponent<FlowerFE>().Load(flower, false, plantT.GetComponent<PlantFE>().FlowerGOs);
                //LoadComp(flower, plantT.gameObject, ConstantValues.Prefabs.Flower, plantT.GetComponent<PlantFE>().FlowerGOs);
            }
        }

        if (stem.Fruit != null)
        {
            foreach (Fruit fruit in stem.Fruit)
            {
                GameObject gameObject = Instantiate(Resources.Load(ConstantValues.Prefabs.Fruit)) as GameObject;
                gameObject.GetComponent<FruitFE>().Load(fruit, false, plantT.GetComponent<PlantFE>().FruitGOs);

                //LoadComp(fruit, plantT.gameObject, ConstantValues.Prefabs.Fruit, plantT.GetComponent<PlantFE>().FruitGOs);
            }
        }

        if (stemGO.GetComponent<StemFE>().Stem.IsEmpty)
        {
            stemGO.GetComponent<MeshRenderer>().enabled = false;
        }

        // Don't want it to grow if it's an empty placeholder
        //if (!stemGO.GetComponent<StemFE>().Stem.IsEmpty)
        //{
        plantT.gameObject.GetComponent<PlantFE>().StemsGOs.Add(stemGO);
        // }

        if (stem.IsFirstStem)
        {
            plantT.GetComponent<PlantFE>().FirstStemGO = stemGO;
        }
    }

    // This should probably be moved to LeafFE
    //public static GameObject LoadDroppedLeaf(Leaf leaf, GameObject plantGO)
    //{
    //    if (leaf.IsDropped)
    //    {
    //        //GameObject newLeafGO = LoadHelper(leaf, plantGO, ConstantValues.Prefabs.Leaf, plantGO.GetComponent<PlantFE>().LeafGOs);
    //        GameObject newLeafGO = Instantiate(Resources.Load(ConstantValues.Prefabs.Leaf)) as GameObject;
    //        newLeafGO.GetComponent<LeafFE>().Load(leaf, true, plantGO.GetComponent<PlantFE>().LeafGOs);

    //        LeafFE.ChangeSize(newLeafGO, 
    //            -(newLeafGO.transform.localScale.x - plantGO.GetComponent<PlantFE>().Plant.Taxonomy.Species.Genus.LeafInitialSize.x),
    //            ConstantValues.PlantConsts.LeafPrefabScale.ToVector3(),
    //            newLeafGO.GetComponent<LeafFE>());
    //        return newLeafGO;
    //    }
    //    else
    //    {
    //        Debug.LogError("Leaf is not dropped. Returning null.");
    //        return null;
    //    }
    //}

    // THis has been moved to IPlantCompFE. Need to remove
    //public static GameObject LoadComp(IPlantComponent comp, GameObject plantGO, string prefabPath, List<GameObject> gameObjects)
    //{
    //    if (!comp.IsDropped)
    //    {
    //        return LoadHelper(comp, plantGO, prefabPath, gameObjects);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Comp " + comp.GetID() + " is dropped. Did not load.");
    //        return null;
    //    }
    //}

    //// THis has been moved to IPlantCompFE. Need to remove
    //private static GameObject LoadHelper(IPlantComponent comp, GameObject plantGO, string prefabPath, List<GameObject> gameObjects)
    //{
    //    GameObject gameObject = Instantiate(Resources.Load(prefabPath)) as GameObject;

    //    if (comp is Flower flower)
    //    {
    //        gameObject.GetComponent<FlowerFE>().Comp = flower;
    //    }
    //    else if (comp is Leaf leaf)
    //    {
    //        gameObject.GetComponent<LeafFE>().Comp = leaf;
    //    }
    //    else if (comp is Fruit fruit)
    //    {
    //        gameObject.GetComponent<FruitFE>().Comp = fruit;
    //    }
    //    else
    //    {
    //        Debug.LogError("Type of component not found.");
    //    }
    //    gameObject.name = comp.GetID();
    //    gameObject.transform.SetParent(GameObject.Find(comp.Parent).transform);
    //    gameObject.transform.localPosition = comp.LocalPosition.ToVector3();
    //    gameObject.transform.localScale = comp.LocalScale.ToVector3();
    //    Quaternion rotation = Quaternion.identity;
    //    rotation.eulerAngles = comp.LocalRotation.ToVector3();
    //    gameObject.transform.localRotation = rotation;

    //    gameObjects.Add(gameObject);

    //    return gameObject;
    //}
}
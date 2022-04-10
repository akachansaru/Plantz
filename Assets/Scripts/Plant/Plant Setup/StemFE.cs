using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Resources.Scripts.Utilities;

// This is added to each plant component game object to adjust attributes in the editor
public class StemFE : MonoBehaviour
{
    [SerializeField] private Stem stem = null;
    [SerializeField] private float amtGrown = 0f;

    public Stem Stem { get { return stem; } set { if (value == null) { Debug.LogError("Stem is null."); } stem = value; } }
    public float AmtGrown { get { return amtGrown; } private set { amtGrown = value; } }

    /// <summary>
    /// Adds leaves to the newly added stem if there are enough steps after the last leaves.
    /// </summary>
    /// <param name="plant"></param>
    /// <param name="leafGOs"></param>
    public void AddLeaves(PlantFE plantFE, List<GameObject> leafGOs)
    {
        Plant plant = plantFE.Plant;
        Branch branch = GetComponent<StemFE>().Stem.Branch;

        if (plant.Taxonomy.Species.Genus.NodesBetweenLeaves != 0)
        {
            if (plant.Taxonomy.Species.Genus.LeavesPerNode > 0)
            {
                if (branch.CurrStep % plant.Taxonomy.Species.Genus.NodesBetweenLeaves == 0 && branch.CurrStep != 0)
                {
                    GameObject emptyGO = plantFE.CreateEmptyStem(gameObject);

                    emptyGO.GetComponent<StemFE>().Stem.LeafPositions = GenerateCompPositions(plant.Taxonomy.Species.Genus.LeavesPerNode);
                    emptyGO.GetComponent<StemFE>().Stem.Leaves = new List<Leaf>(); // FIXME: Might not need this since I initialized it in Stem

                    for (int l = 0; l < plant.Taxonomy.Species.Genus.LeavesPerNode; l++)
                    {
                        GameObject newLeafGO = Instantiate(Resources.Load(ConstantValues.Prefabs.Leaf)) as GameObject;
                        Stem emptyStem = emptyGO.GetComponent<StemFE>().Stem;
                        newLeafGO.GetComponent<LeafFE>().SetupCompGO(emptyGO.transform, Stem, l, "L", emptyStem.Leaves,
                            plant.Taxonomy.Species.Genus.LeafInitialSize.ToVector3(), emptyStem.LeafPositions);

                        newLeafGO.GetComponent<MeshRenderer>().enabled = GetComponent<MeshRenderer>().enabled; // For when the plant grows while being held

                        leafGOs.Add(newLeafGO);
                        plant.Leaves.Add(emptyGO.GetComponent<StemFE>().Stem.Leaves[l]);
                    }
                }
            }
            else
            {
                Debug.Log("No leaves to add");
            }
        }
        else
        {
            Debug.LogError("leafNodes set to 0. Need to fix");
        }
    }
    
    //private GameObject SetupLeaf(PlantFE plantFE, Transform emptyT, int num)
    //{
    //    Plant plant = plantFE.Plant;
    //    GameObject newLeafGO = Instantiate(Resources.Load(ConstantValues.Prefabs.Leaf)) as GameObject;

    //    newLeafGO.GetComponent<LeafFE>().Comp = new Leaf(name, num);
    //    newLeafGO.name = newLeafGO.GetComponent<LeafFE>().Comp.GetID();

    //    LeafFE leafFE = newLeafGO.GetComponent<LeafFE>();
    //    Stem emptyStem = emptyT.GetComponent<StemFE>().Stem;
    //    leafFE.SetParent(emptyT);
    //    leafFE.SetScale(plant.Taxonomy.Species.Genus.LeafInitialSize.ToVector3());
    //    leafFE.SetPosition(new Vector3(
    //        emptyStem.LeafPositions[num].ToVector3().x +
    //        (-.5f + (newLeafGO.transform.localScale.x / 2)) * emptyStem.LeafPositions[num].ToVector3().normalized.x,
    //        emptyStem.LeafPositions[num].ToVector3().y +
    //        (-.5f + (newLeafGO.transform.localScale.x / 2)) * emptyStem.LeafPositions[num].ToVector3().normalized.y,
    //        emptyStem.LeafPositions[num].ToVector3().z +
    //        (-.5f + (newLeafGO.transform.localScale.x / 2)) * emptyStem.LeafPositions[num].ToVector3().normalized.z));
    //    // FIXME (potential): Only using the x comp of the localScale since that's the only length that matters with current leaf model
    //    leafFE.SetRotation(GenerateLeafRotation(emptyStem.LeafPositions[num]));

    //    emptyStem.Leaves.Add(newLeafGO.GetComponent<LeafFE>().Comp);
    //    //emptyStem.Leaves[num].OriginalPosition = new Vector3Serializable(newLeafGO.transform.localPosition);
    //    return newLeafGO;
    //}

    //private GameObject SetupFlower(PlantFE plantFE, Transform emptyT, int num)
    //{
    //    Plant plant = plantFE.Plant;
    //    GameObject newFlowerGO = Instantiate(Resources.Load(ConstantValues.Prefabs.Flower)) as GameObject;

    //    newFlowerGO.GetComponent<FlowerFE>().Comp = new Flower(name, num);
    //    newFlowerGO.name = newFlowerGO.GetComponent<FlowerFE>().Comp.GetID();

    //    FlowerFE flowerFE = newFlowerGO.GetComponent<FlowerFE>();
    //    Stem emptyStem = emptyT.GetComponent<StemFE>().Stem;
    //    flowerFE.SetParent(emptyT);
    //    flowerFE.SetScale(plant.Taxonomy.Species.Genus.FlowerInitialSize.ToVector3());
    //    flowerFE.SetPosition(new Vector3(
    //        emptyStem.FlowerPositions[num].ToVector3().x +
    //        (-.5f + (newFlowerGO.transform.localScale.x / 2)) * emptyStem.FlowerPositions[num].ToVector3().normalized.x,
    //        emptyStem.FlowerPositions[num].ToVector3().y +
    //        (-.5f + (newFlowerGO.transform.localScale.x / 2)) * emptyStem.FlowerPositions[num].ToVector3().normalized.y,
    //        emptyStem.FlowerPositions[num].ToVector3().z +
    //        (-.5f + (newFlowerGO.transform.localScale.x / 2)) * emptyStem.FlowerPositions[num].ToVector3().normalized.z));
    //    // FIXME (potential): Only using the x comp of the localScale since that's the only length that matters with current flower model
    //    flowerFE.SetRotation(GenerateLeafRotation(emptyStem.FlowerPositions[num]));

    //    emptyStem.Flowers.Add(newFlowerGO.GetComponent<FlowerFE>().Comp);
    //    return newFlowerGO;
    //}

    private Vector3Serializable[] GenerateCompPositions(int compsPerNode)
    {
        Vector3Serializable[] positions = new Vector3Serializable[compsPerNode];
        switch (compsPerNode)
        {
            case 0:
                Debug.LogWarning("No compsPerNode.");
                break;
            case 1:
                positions[0] = new Vector3Serializable(0, 0, 1);
                break;
            case 2:
                positions[0] = new Vector3Serializable(0, 0, 1);
                positions[1] = new Vector3Serializable(0, 0, -1);
                break;
            case 3:
                positions[0] = new Vector3Serializable(0, 0, 1);
                positions[1] = new Vector3Serializable(0, 0, -1);
                positions[2] = new Vector3Serializable(1, 0, 0);
                break;
            case 4:
                positions[0] = new Vector3Serializable(0, 0, 1);
                positions[1] = new Vector3Serializable(0, 0, -1);
                positions[2] = new Vector3Serializable(1, 0, 0);
                positions[3] = new Vector3Serializable(-1, 0, 0);
                break;
            default:
                Debug.LogError("Invalid compsPerNode.");
                break;
        }
        return positions;
    }

    private Vector3 GenerateLeafRotation(Vector3Serializable leafPosition)
    {
        // No rotation for x, around y and z for y, around y for z
        Vector3 rotation = Vector3.zero;
        if (leafPosition.x < 0)
        {
            rotation = new Vector3(0, 180, 0);
        }
        else if (leafPosition.y < 0)
        {
            rotation = new Vector3(0, 0, -90);
        }
        else if (leafPosition.y > 0)
        {
            rotation = new Vector3(0, 0, 90);
        }
        else if (leafPosition.z < 0)
        {
            rotation = new Vector3(0, 90, 0);
        }
        else if (leafPosition.z > 0)
        {
            rotation = new Vector3(0, -90, 0);
        }
        return rotation;
    }

    /// <summary>
    /// Adds leaves to the newly added stem if there are enough steps after the last leaves.
    /// </summary>
    /// <param name="plant"></param>
    /// <param name="leafGOs"></param>
    public void AddFlowers(PlantFE plantFE, List<GameObject> flowerGOs, List<Flower> flowers)
    { // FIXME: this was just copied from AddLeaves. Could make one method that does both leaves and flowers
        Plant plant = plantFE.Plant;
        Branch branch = GetComponent<StemFE>().Stem.Branch;

        if (plant.Taxonomy.Species.Genus.NodesBetweenFlowers != 0)
        {
            if (plant.Taxonomy.Species.Genus.FlowersPerNode > 0)
            {
                if (branch.CurrStep % plant.Taxonomy.Species.Genus.NodesBetweenFlowers == 0 && branch.CurrStep != 0)
                {
                    GameObject emptyGO = plantFE.CreateEmptyStem(gameObject);

                    emptyGO.GetComponent<StemFE>().Stem.FlowerPositions = GenerateCompPositions(plant.Taxonomy.Species.Genus.FlowersPerNode);
                    emptyGO.GetComponent<StemFE>().Stem.Flowers = new List<Flower>(); // FIXME: Might not need this since I initialized it in Stem

                    for (int l = 0; l < plant.Taxonomy.Species.Genus.FlowersPerNode; l++)
                    {
                        //GameObject newFlowerGO = SetupFlower(plantFE, emptyGO.transform, l);
                        GameObject newFlowerGO = Instantiate(Resources.Load(ConstantValues.Prefabs.Flower)) as GameObject;
                        Stem emptyStem = emptyGO.GetComponent<StemFE>().Stem;
                        newFlowerGO.GetComponent<FlowerFE>().SetupCompGO(emptyGO.transform, Stem, l, "F", emptyStem.Flowers,
                            plant.Taxonomy.Species.Genus.FlowerInitialSize.ToVector3(), emptyStem.FlowerPositions);

                        newFlowerGO.GetComponent<MeshRenderer>().enabled = GetComponent<MeshRenderer>().enabled; // For when the plant grows while being held

                        flowerGOs.Add(newFlowerGO);
                        flowers.Add(emptyGO.GetComponent<StemFE>().Stem.Flowers[l]);
                    }
                }
            }
            else
            {
                Debug.Log("No flowers to add");
            }
        }
        else
        {
            Debug.LogError("flowerNodes set to 0. Need to fix");
        }
    }

    public void SetName(string name)
    {
        gameObject.name = name;
        Stem.CreateID(name);
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        stem.Parent = parent.name;
    }

    public void SetPosition(Vector3 localPosition)
    {
        transform.localPosition = localPosition;
        stem.Position = new Vector3Serializable(localPosition);
    }

    /// <summary>
    /// Set the local scale for the Game Object and the global scale for the stem
    /// </summary>
    /// <param name="localScale"></param>
    public void SetScaleLocal(Vector3 localScale)
    {
        transform.localScale = localScale;
        stem.GlobalScale = new Vector3Serializable(transform.lossyScale);
    }

    public void SetScaleGlobal(Vector3 globalScale)
    {
        // There shouldn't be children yet since this is only called when a new stem is made so not unparenting
        TransformUtilities.SetGlobalScale(transform, globalScale);
        stem.GlobalScale = new Vector3Serializable(transform.lossyScale);
    }

    public void SetScaleGlobal(float globalScale)
    {
        // There shouldn't be children yet since this is only called when a new stem is made so not unparenting
        TransformUtilities.SetGlobalScale(transform, new Vector3(globalScale, globalScale, globalScale));
        stem.GlobalScale = new Vector3Serializable(transform.lossyScale);
    }

    public void SetRotation(Vector3 localRotation)
    {
        Quaternion quat = Quaternion.identity;
        quat.eulerAngles = localRotation;
        transform.localRotation = quat;
        stem.Rotation = new Vector3Serializable(transform.localRotation.eulerAngles);
    }

    public void SetRotation(Quaternion localRotation)
    {
        transform.localRotation = localRotation;
        stem.Rotation = new Vector3Serializable(localRotation.eulerAngles);
    }
}
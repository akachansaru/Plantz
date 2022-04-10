using UnityEngine;
using System;
using System.Collections.Generic;

// This is what is actually saved
[Serializable]
public class Stem : IEquatable<Stem>
{
    [SerializeField] private string stemID;
    [SerializeField] private string parent = "Not assigned";
    [SerializeField] private int branchNum = -1;
    [SerializeField] private List<Leaf> leaves = new List<Leaf>();
    [SerializeField] private List<Flower> flowers = new List<Flower>();
    [SerializeField] private List<Fruit> fruit = new List<Fruit>();

    [SerializeField] private Vector3Serializable[] leafPositions = null;
    [SerializeField] private Vector3Serializable[] flowerPositions = null;
    [SerializeField] private List<Vector3Serializable> fruitPositions = new List<Vector3Serializable>();

    [SerializeField] private Vector3Serializable position = new Vector3Serializable(Vector3.zero);
    [SerializeField] private Vector3Serializable globalScale = new Vector3Serializable(Vector3.zero);
    [SerializeField] private Vector3Serializable rotation = new Vector3Serializable(Vector3.zero);
    [SerializeField] private Branch branch = null;
    [SerializeField] private bool isEmpty = false; // For the empy GOs created to avoid leaf/flower scaling issues

    public string StemID { get { return stemID; } private set { stemID = value; } }
    public bool IsFirstStem { get; set; } = false;
    public string Parent { get { return parent; } set { if (value == null) { Debug.LogWarning("Parent is null."); } parent = value; } }
    public int BranchNum { get { return branchNum; } set { if (value < 0) { Debug.LogWarning("BranchNum is negative."); } branchNum = value; } }
    public List<Leaf> Leaves { get { return leaves; } set { if (value == null) { Debug.LogWarning("Leaves is null."); } leaves = value; } }
    public List<Flower> Flowers { get { return flowers; } set { if (value == null) { Debug.LogWarning("Flowers is null."); } flowers = value; } }
    public List<Fruit> Fruit { get { return fruit; } set { if (value == null) { Debug.LogWarning("Fruit is null."); } fruit = value; } }

    public Vector3Serializable[] LeafPositions { get { return leafPositions; } set { leafPositions = value; } }
    public Vector3Serializable[] FlowerPositions { get { return flowerPositions; } set { flowerPositions = value; } }

    public Vector3Serializable Position { get { return position; } set { position = value; } }
    public Vector3Serializable GlobalScale { get { return globalScale; } set { globalScale = value; } }
    public Vector3Serializable Rotation { get { return rotation; } set { rotation = value; } }
    public Branch Branch { get { return branch; } set { branch = value; } }
    public bool IsEmpty { get { return isEmpty; } set { isEmpty = value; } }

    private void CreateID(Plant plant, int stemNum)
    {
        stemID = "S" + stemNum + " " + plant.PlantID;
    }

    public void CreateID(string ID)
    {
        stemID = ID;
    }

    public Stem(Plant plant, int stemNum)
    {
        CreateID(plant, stemNum);
    }

    private Stem()
    {

    }

    public Stem Clone()
    {
        Stem stem = new Stem
        {
            StemID = StemID, // Get rid of?
            Parent = Parent,
            BranchNum = BranchNum,
            Leaves = Leaves,
            Flowers = Flowers, // Just added 11/28/20
            LeafPositions = LeafPositions,
            Position = Position,
            GlobalScale = GlobalScale,
            Rotation = Rotation,
            branch = branch
        };
        return stem;
    }

    /// <summary>
    /// Adds leaves to the newly added stem if there are enough steps after the last leaves.
    /// </summary>
    /// <param name="plant"></param>
    /// <param name="leaves"></param>
    public void AddLeaves(Plant plant, List<Leaf> leaves)
    {
        if (plant.Taxonomy.Species.Genus.NodesBetweenLeaves != 0)
        {
            if (plant.Taxonomy.Species.Genus.LeavesPerNode > 0)
            {
                if (branch.CurrStep % plant.Taxonomy.Species.Genus.NodesBetweenLeaves == 0 && branch.CurrStep != 0)
                {
                    Stem emptyStem = plant.CreateEmptyStem(this);

                    emptyStem.LeafPositions = GenerateLeafPositions(plant.Taxonomy.Species.Genus.LeavesPerNode);
                    Leaves = new List<Leaf>(); // This seems necessary but not sure why since I initialized it in Stem

                    for (int l = 0; l < plant.Taxonomy.Species.Genus.LeavesPerNode; l++)
                    {
                        Leaf newLeaf = SetupLeaf(plant, emptyStem, l);
                        //newLeafGO.GetComponent<MeshRenderer>().enabled = GetComponent<MeshRenderer>().enabled; // For when the plant grows while being held

                        leaves.Add(newLeaf);
                        //plant.Leaves.Add(emptyStem.Leaves[l]);
                    }
                }
            }
            else
            {
                Debug.LogWarning("No leaves to add: " + plant.PlantID);
            }
        }
        else
        {
            Debug.LogError("leafNodes set to 0. Need to fix");
        }
    }

    private Leaf SetupLeaf(Plant plant, Stem emptyStem, int num)
    {
        //GameObject newLeafGO = Instantiate(Resources.Load(ConstantValues.Prefabs.Leaf)) as GameObject;
        Leaf newLeaf = new Leaf(StemID, num);

        //LeafFE leafFE = newLeafGO.GetComponent<LeafFE>();
        newLeaf.Parent = emptyStem.StemID;
        newLeaf.LocalScale = plant.Taxonomy.Species.Genus.LeafInitialSize;
        newLeaf.LocalPosition = (new Vector3Serializable(
            emptyStem.LeafPositions[num].ToVector3().x +
            (-.5f + (newLeaf.LocalScale.x / 2)) * emptyStem.LeafPositions[num].ToVector3().normalized.x,
            emptyStem.LeafPositions[num].ToVector3().y +
            (-.5f + (newLeaf.LocalScale.x / 2)) * emptyStem.LeafPositions[num].ToVector3().normalized.y,
            emptyStem.LeafPositions[num].ToVector3().z +
            (-.5f + (newLeaf.LocalScale.x / 2)) * emptyStem.LeafPositions[num].ToVector3().normalized.z));
        // FIXME (potential): Only using the x comp of the localScale since that's the only length that matters with current leaf model
        newLeaf.LocalRotation = GenerateLeafRotation(emptyStem.LeafPositions[num]);

        emptyStem.Leaves.Add(newLeaf);
        //emptyStem.Leaves[num].OriginalPosition = newLeaf.LocalPosition;
        return newLeaf;
    }

    private Flower SetupFlower(Plant plant, Stem emptyStem, int num)
    {
        Flower newFlower = new Flower(StemID, num);

        newFlower.Parent = emptyStem.StemID;
        newFlower.LocalScale = plant.Taxonomy.Species.Genus.FlowerInitialSize;
        newFlower.LocalPosition = new Vector3Serializable(
            emptyStem.FlowerPositions[num].ToVector3().x +
            (-.5f + (newFlower.LocalScale.x / 2)) * emptyStem.FlowerPositions[num].ToVector3().normalized.x,
            emptyStem.FlowerPositions[num].ToVector3().y +
            (-.5f + (newFlower.LocalScale.x / 2)) * emptyStem.FlowerPositions[num].ToVector3().normalized.y,
            emptyStem.FlowerPositions[num].ToVector3().z +
            (-.5f + (newFlower.LocalScale.x / 2)) * emptyStem.FlowerPositions[num].ToVector3().normalized.z);
        // FIXME (potential): Only using the x comp of the localScale since that's the only length that matters with current flower model
        newFlower.LocalRotation = GenerateLeafRotation(emptyStem.FlowerPositions[num]);

        emptyStem.Flowers.Add(newFlower);
        return newFlower;
    }

    private Vector3Serializable[] GenerateLeafPositions(int leavesPerNode)
    {
        Vector3Serializable[] leafPositions = new Vector3Serializable[leavesPerNode];
        switch (leavesPerNode)
        {
            case 0:
                Debug.LogWarning("No leaversPerNode.");
                break;
            case 1:
                leafPositions[0] = new Vector3Serializable(0, 0, 1);
                break;
            case 2:
                leafPositions[0] = new Vector3Serializable(0, 0, 1);
                leafPositions[1] = new Vector3Serializable(0, 0, -1);
                break;
            case 3:
                leafPositions[0] = new Vector3Serializable(0, 0, 1);
                leafPositions[1] = new Vector3Serializable(0, 0, -1);
                leafPositions[2] = new Vector3Serializable(1, 0, 0);
                break;
            case 4:
                leafPositions[0] = new Vector3Serializable(0, 0, 1);
                leafPositions[1] = new Vector3Serializable(0, 0, -1);
                leafPositions[2] = new Vector3Serializable(1, 0, 0);
                leafPositions[3] = new Vector3Serializable(-1, 0, 0);
                break;
            default:
                Debug.LogError("Invalid leavesPerNode.");
                break;
        }
        return leafPositions;
    }

    private Vector3Serializable GenerateLeafRotation(Vector3Serializable leafPosition)
    {
        // No rotation for x, around y and z for y, around y for z
        Vector3Serializable rotation = new Vector3Serializable(Vector3.zero);
        if (leafPosition.x < 0)
        {
            rotation = new Vector3Serializable(0, 180, 0);
        }
        else if (leafPosition.y < 0)
        {
            rotation = new Vector3Serializable(0, 0, -90);
        }
        else if (leafPosition.y > 0)
        {
            rotation = new Vector3Serializable(0, 0, 90);
        }
        else if (leafPosition.z < 0)
        {
            rotation = new Vector3Serializable(0, 90, 0);
        }
        else if (leafPosition.z > 0)
        {
            rotation = new Vector3Serializable(0, -90, 0);
        }
        return rotation;
    }

    /// <summary>
    /// Adds leaves to the newly added stem if there are enough steps after the last leaves.
    /// </summary>
    /// <param name = "plant" ></ param >
    /// < param name= "leafGOs" ></ param >
    public void AddFlowers(Plant plant, List<Flower> flowers)
    { // FIXME: this was just copied from AddLeaves. Could make one method that does both leaves and flowers
        if (plant.Taxonomy.Species.Genus.NodesBetweenFlowers != 0)
        {
            if (plant.Taxonomy.Species.Genus.FlowersPerNode > 0)
            {
                if (branch.CurrStep % plant.Taxonomy.Species.Genus.NodesBetweenFlowers == 0 && branch.CurrStep != 0)
                {
                    Stem emptyStem = plant.CreateEmptyStem(this);

                    emptyStem.FlowerPositions = GenerateLeafPositions(plant.Taxonomy.Species.Genus.FlowersPerNode);
                    Flowers = new List<Flower>(); // FIXME: Might not need this since I initialized it in Stem

                    for (int l = 0; l < plant.Taxonomy.Species.Genus.FlowersPerNode; l++)
                    {
                        Flower newFlower = SetupFlower(plant, emptyStem, l);

                        flowers.Add(newFlower);
                        //plant.ThisYearsFlowers.Add(emptyStem.Flowers[l]);
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

    public bool Equals(Stem other)
    {
        return StemID == other.StemID;
    }
}
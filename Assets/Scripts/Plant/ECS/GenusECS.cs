using Unity.Mathematics;
using UnityEngine;

public class GenusECS
{
    public int GenusNum { get; private set; }
    public Rarity Rarity { get; private set; }
    public FamilyECS Family { get; private set; }

    public StatECS PlantMaxSize { get; private set; }
    public StatECS TimeBetweenPlantGrowth { get; private set; }

    // Leaves
    public int LeavesPerNode { get; private set; }
    public int NodesBetweenLeaves { get; private set; }
    public StatECS LeafInitialSize { get; private set; }
    public StatECS LeafMaxSize { get; private set; }
    public StatECS WidenLeafTime { get; private set; }

    // Flowers
    public int FlowersPerNode { get; private set; }
    public int NodesBetweenFlowers { get; private set; }
    public StatECS FlowerInitialSize { get; private set; }
    public StatECS FlowerMaxSize { get; private set; }
    public StatECS WidenFlowerTime { get; private set; }


    public GenusECS(int speciesNum)
    {
        GenusNum = GetGenus(speciesNum);
        Rarity = GetRarity(GenusNum);
        Family = new FamilyECS(GenusNum);

        PlantMaxSize = GetPlantMaxSize(GenusNum);
        TimeBetweenPlantGrowth = GetTimeBetweenPlantGrowth(GenusNum);

        LeavesPerNode = GetLeavesPerNode(GenusNum);
        NodesBetweenLeaves = GetNodesBetweenLeaves(GenusNum);
        LeafInitialSize = GetLeafInitialSize(GenusNum);
        LeafMaxSize = GetLeafMaxSize(GenusNum);
        WidenLeafTime = GetWidenLeafTime(GenusNum);

        FlowersPerNode = GetFlowersPerNode(GenusNum);
        NodesBetweenFlowers = GetNodesBetweenFlowers(GenusNum);
        FlowerInitialSize = GetFlowerInitialSize(GenusNum);
        FlowerMaxSize = GetFlowerMaxSize(GenusNum);
        WidenFlowerTime = GetWidenFlowerTime(GenusNum);
    }

    // Genus is determined by the species
    private int GetGenus(int species)
    {
        switch (species)
        {
            case 0:
                return 1;
            case 1:
                return 0;
            case 2:
                return 2;
            case 3:
                return 0;
            default:
                Debug.LogError("Invalid species num");
                return 0;
        }
    }

    private StatECS GetTimeBetweenPlantGrowth(int genus)
    {
        switch (genus)
        {
            case 0:
                return new StatECS(20, 20.2f);
            case 1:
                return new StatECS(30, 30.3f);
            case 2:
                return new StatECS(40, 40.4f);
            default:
                Debug.LogError("Invalid genus num");
                return new StatECS(1, 1.1f);
        }
    }

    private StatECS GetPlantMaxSize(int genus)
    {
        switch (genus)
        {
            case 0:
                return new StatECS(100, 110);
            case 1:
                return new StatECS(200, 220f);
            case 2:
                return new StatECS(600, 660f);

            default:
                Debug.LogError("Invalid genus num");
                return new StatECS(100, 110f);
        }
    }

    // Leaves
    private int GetLeavesPerNode(int genus)
    {
        switch (genus)
        {
            case 0:
                return 3;
            case 1:
                return 4;
            case 2:
                return 2;

            default:
                Debug.LogError("Invalid genus num");
                return 2;
        }
    }

    private int GetNodesBetweenLeaves(int genus)
    {
        switch (genus)
        {
            case 0:
                return 3;
            case 1:
                return 2;
            case 2:
                return 2;

            default:
                Debug.LogError("Invalid genus num");
                return 2;
        }
    }

    private StatECS GetLeafInitialSize(int genus)
    {
        switch (genus)
        {
            case 0:
                return new StatECS(0.2f, 0.22f);
            case 1:
                return new StatECS(0.1f, 0.11f);
            case 2:
                return new StatECS(0.15f, 0.2f);

            default:
                Debug.LogError("Invalid genus num");
                return new StatECS(0.2f, 0.22f);
        }
    }

    private StatECS GetLeafMaxSize(int genus)
    {
        switch (genus)
        {
            case 0:
                return new StatECS(0.8f, 0.88f);
            case 1:
                return new StatECS(0.4f, 0.44f);
            case 2:
                return new StatECS(0.6f, 0.66f);

            default:
                Debug.LogError("Invalid genus num");
                return new StatECS(0.2f, 0.22f);
        }
    }

    private StatECS GetWidenLeafTime(int genus)
    {
        switch (genus)
        {
            case 0:
                return new StatECS(5f, 5.05f);
            case 1:
                return new StatECS(10f, 10.1f);
            case 2:
                return new StatECS(20f, 20.2f);
            default:
                Debug.LogError("Invalid genus num");
                return new StatECS(2f, 2.2f);
        }
    }

    // Flowers
    private int GetFlowersPerNode(int genus)
    {
        switch (genus)
        {
            case 0:
                return 4;
            case 1:
                return 4;
            case 2:
                return 2;

            default:
                Debug.LogError("Invalid genus num");
                return 2;
        }
    }

    private int GetNodesBetweenFlowers(int genus)
    {
        switch (genus)
        {
            case 0:
                return 1;
            case 1:
                return 1;
            case 2:
                return 1;

            default:
                Debug.LogError("Invalid genus num");
                return 2;
        }
    }

    private StatECS GetFlowerInitialSize(int genus)
    {
        switch (genus)
        {
            case 0:
                return new StatECS(0.2f, 0.22f);
            case 1:
                return new StatECS(0.1f, 0.11f);
            case 2:
                return new StatECS(0.15f, 0.2f);

            default:
                Debug.LogError("Invalid genus num");
                return new StatECS(0.2f, 0.22f);
        }
    }

    private StatECS GetFlowerMaxSize(int genus)
    {
        switch (genus)
        {
            case 0:
                return new StatECS(0.8f, 0.88f);
            case 1:
                return new StatECS(0.4f, 0.44f);
            case 2:
                return new StatECS(0.6f, 0.66f);

            default:
                Debug.LogError("Invalid genus num");
                return new StatECS(0.2f, 0.22f);
        }
    }

    private StatECS GetWidenFlowerTime(int genus)
    {
        switch (genus)
        {
            case 0:
                return new StatECS(5f, 5.05f);
            case 1:
                return new StatECS(10f, 10.1f);
            case 2:
                return new StatECS(20f, 20.2f);
            default:
                Debug.LogError("Invalid genus num");
                return new StatECS(2f, 2.2f);
        }
    }

    private Rarity GetRarity(int genus)
    {
        switch (genus)
        {
            case 0:
                return Rarity.Common;
            case 1:
                return Rarity.Uncommon;
            case 2:
                return Rarity.Rare;
            default:
                Debug.LogError("Invalid genus num");
                return Rarity.Common;
        }
    }
}


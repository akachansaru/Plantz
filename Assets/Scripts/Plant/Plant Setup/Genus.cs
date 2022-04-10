using Assets.Scripts.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The genus dictates size and shape of leaves, size of plant (x), growth speed (x), internodes (x),
/// nodes between leaves (x), number of flower petals, leaf arangement (opposite, alternate, whorled)
/// </summary>
[Serializable]
public class Genus
{
    // Plant
    [SerializeField] private string genusName;
    [SerializeField] private Rarity rarity; // Converted to ECS
    [SerializeField] private Family family; // Converted to ECS
    [SerializeField] private int plantMaxSize; // Converted to ECS
    [SerializeField] private float timeBetweenPlantGrowth; // Converted to ECS // This is the time for a new stem segment to grow
    //[SerializeField] private int internodes;
    //[SerializeField] private int branchesPerCycles; // Number of branches spiraling/whorling before coming back to the beginning. 4 would be 1 branch every 90 degrees

    //[SerializeField] private Vector4Serializable trunkRotation;
    //[SerializeField] private Vector4Serializable branchRotation;

    // Leaves
    [SerializeField] private int leavesPerNode; // Converted to ECS
    [SerializeField] private int nodesBetweenLeaves; // Converted to ECS
    [SerializeField] private Vector3Serializable leafInitialSize; // Converted to ECS
    [SerializeField] private Stat leafMaxSize; // Converted to ECS
    [SerializeField] private Stat leafGrowthRate; // Converted to ECS

    // Flowers
    [SerializeField] private int flowersPerNode; // Converted to ECS
    [SerializeField] private int nodesBetweenFlowers; // Converted to ECS
    [SerializeField] private Vector3Serializable flowerInitialSize; // Converted to ECS
    [SerializeField] private float flowerMaxSize; // Converted to ECS
    [SerializeField] private float flowerGrowthRate; // Converted to ECS

    // Fruit
    [SerializeField] private Vector3Serializable fruitInitialSize;
    [SerializeField] private float fruitMaxSize;
    [SerializeField] private float fruitGrowthRate;

    // Plant
    public string GenusName { get { return genusName; } protected set { genusName = value; } }
    public Rarity Rarity { get { return rarity; } protected set { rarity = value; } }
    public Family Family { get { return family; } protected set { family = value; } }
    public int PlantMaxSize { get { return plantMaxSize; } private set { plantMaxSize = value; } }
    /// <summary>
    /// In minutes. Time between new stems being added
    /// </summary>
    public float TimeBetweenPlantGrowth { get { return timeBetweenPlantGrowth; } private set { timeBetweenPlantGrowth = value; } }

    //public int Internodes { get { return internodes; } protected set { internodes = value; } }
    //public int BranchesPerCycle { get { return branchesPerCycles; } protected set { branchesPerCycles = value; } }

    //protected Vector4Serializable TrunkRotation { get { return trunkRotation; } set { trunkRotation = value; } }
    //protected Vector4Serializable BranchRotation { get { return branchRotation; } set { branchRotation = value; } }

    // Leaves
    public int LeavesPerNode { get { return leavesPerNode; } protected set { leavesPerNode = value; } }
    public int NodesBetweenLeaves { get { return nodesBetweenLeaves; } private set { nodesBetweenLeaves = value; } }
    public Vector3Serializable LeafInitialSize { get { return leafInitialSize; } private set { leafInitialSize = value; } }
    public Stat LeafMaxSize { get { return leafMaxSize; } private set { leafMaxSize = value; } }
    public Stat LeafGrowthRate { get { return leafGrowthRate; } set { leafGrowthRate = value; } }

    // Flowers
    public int FlowersPerNode { get { return flowersPerNode; } protected set { flowersPerNode = value; } }
    public int NodesBetweenFlowers { get { return nodesBetweenFlowers; } private set { nodesBetweenFlowers = value; } }
    public Vector3Serializable FlowerInitialSize { get { return flowerInitialSize; } private set { flowerInitialSize = value; } }
    public float FlowerMaxSize { get { return flowerMaxSize; } private set { flowerMaxSize = value; } }
    public float FlowerGrowthRate { get { return flowerGrowthRate; } private set { flowerGrowthRate = value; } }

    // Fruit
    public Vector3Serializable FruitInitialSize { get { return fruitInitialSize; } private set { fruitInitialSize = value; } }
    public float FruitMaxSize { get { return fruitMaxSize; } private set { fruitMaxSize = value; } }
    public float FruitGrowthRate { get { return fruitGrowthRate; } private set { fruitGrowthRate = value; } }


    // Plant
    protected void SetPlantMaxSize(int value, float variancePercent)
    {
        PlantMaxSize = value + (int)UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
    }

    /// <summary>
    /// timeBetweenGrowth units between each growth.
    /// Ex: timeBetweenGrowth = 3, unit = Days => 3 days between each growth.
    /// </summary>
    /// <param name="timeBetweenGrowth"></param>
    /// <param name="unit"></param>
    /// <param name="variancePercent"></param>
    protected void SetTimeBetweenPlantGrowth(float timeBetweenGrowth, TimeUnits unit, float variancePercent)
    {
        // Break it down into minutes
        float value;
        switch (unit)
        {
            case TimeUnits.Year:
                value = timeBetweenGrowth * GameTime.YEAR; // GameTime.YEAR etc. are unitless and just to convert to minutes
                break;
            case TimeUnits.Season:
                value = timeBetweenGrowth * GameTime.SEASON;
                break;
            case TimeUnits.Day:
                value = timeBetweenGrowth * GameTime.DAY;
                break;
            case TimeUnits.Hour:
                value = timeBetweenGrowth * GameTime.HOUR;
                break;
            case TimeUnits.Minute:
                value = timeBetweenGrowth;
                break;
            default:
                value = 0;
                Debug.LogError("Invalid TimeUnit");
                break;
        }
        TimeBetweenPlantGrowth = value + UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
    }

    // Leaves
    protected void SetNodesBetweenLeaves(int value, float variancePercent)
    {
        NodesBetweenLeaves = value + (int)UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
    }

    protected void SetLeafInitialSize(float value, float variancePercent)
    {
        float prefabScale = value / ConstantValues.PlantConsts.LeafPrefabScale.x;
        float temp = value + UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
        LeafInitialSize = new Vector3Serializable(temp, ConstantValues.PlantConsts.LeafPrefabScale.y * prefabScale,
            ConstantValues.PlantConsts.LeafPrefabScale.z * prefabScale);
    }

    protected void SetLeafMaxSize(float value, float variancePercent)
    {
        LeafMaxSize = new Stat(value, value + value * variancePercent, ConstantValues.StatID.LeafMaxSize); // value + UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
    }

    /// <summary>
    /// numGrowthSeasons is the number of seasons (or fraction of season) over which the leaf is actively growing.
    /// Ex: numGrowthSeasons = 4 => leaves at full size in one year
    /// Ex: numGrowthSeasons = 0.5 => leaves at full size in half a season
    /// </summary>
    /// <param name="numGrowthSeasons"></param>
    /// <param name="variancePercent"></param>
    protected void SetLeafGrowthRate(float timeToMaturity, TimeUnits unit, float variancePercent)
    {
        float value;
        switch (unit)
        {
            case TimeUnits.Year:
                value = timeToMaturity * GameTime.YEAR / ((LeafMaxSize.Value - LeafInitialSize.x) / ConstantValues.PlantConsts.GrowthAmt);
                break;
            case TimeUnits.Season:
                value = timeToMaturity * GameTime.SEASON / ((LeafMaxSize.Value - LeafInitialSize.x) / ConstantValues.PlantConsts.GrowthAmt);
                break;
            case TimeUnits.Day:
                value = timeToMaturity * GameTime.DAY / ((LeafMaxSize.Value - LeafInitialSize.x) / ConstantValues.PlantConsts.GrowthAmt);
                break;
            case TimeUnits.Hour:
                value = timeToMaturity * GameTime.HOUR / ((LeafMaxSize.Value - LeafInitialSize.x) / ConstantValues.PlantConsts.GrowthAmt);
                break;
            default:
                value = 0;
                Debug.LogError("Invalid TimeUnit");
                break;
        }
        LeafGrowthRate = new Stat(value, value + value * variancePercent, ConstantValues.StatID.LeafGrowthRate); //value + UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
    }

    // Flowers
    protected void SetNodesBetweenFlowers(int value, float variancePercent)
    {
        NodesBetweenFlowers = value + (int)UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
    }

    protected void SetFlowerInitialSize(float value, float variancePercent)
    {
        // Might need to change this like I did for Fruit
        float prefabScale = value / ConstantValues.PlantConsts.FlowerPrefabScale.x;
        float temp = value + UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
        FlowerInitialSize = new Vector3Serializable(temp, ConstantValues.PlantConsts.FlowerPrefabScale.y * prefabScale,
            ConstantValues.PlantConsts.FlowerPrefabScale.z * prefabScale);
    }

    protected void SetFlowerMaxSize(float value, float variancePercent)
    {
        FlowerMaxSize = value + UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
    }

    protected void SetFlowerGrowthRate(float timeToMaturity, TimeUnits unit, float variancePercent)
    {
        float value;
        switch (unit)
        {
            case TimeUnits.Year:
                value = timeToMaturity * GameTime.YEAR / ((FlowerMaxSize - FlowerInitialSize.x) / ConstantValues.PlantConsts.GrowthAmt);
                break;
            case TimeUnits.Season:
                value = timeToMaturity * GameTime.SEASON / ((FlowerMaxSize - FlowerInitialSize.x) / ConstantValues.PlantConsts.GrowthAmt);
                break;
            case TimeUnits.Day:
                value = timeToMaturity * GameTime.DAY / ((FlowerMaxSize - FlowerInitialSize.x) / ConstantValues.PlantConsts.GrowthAmt);
                break;
            case TimeUnits.Hour:
                value = timeToMaturity * GameTime.HOUR / ((FlowerMaxSize - FlowerInitialSize.x) / ConstantValues.PlantConsts.GrowthAmt);
                break;
            default:
                value = 0;
                Debug.LogError("Invalid TimeUnit");
                break;
        }
        FlowerGrowthRate = value + UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
    }

    protected void SetFruitInitialSize(float value, float variancePercent)
    {
        float variance = UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
        float prefabScale = (value + variance) / ConstantValues.PlantConsts.FruitPrefabScale.x;
        float temp = value + variance;
        FruitInitialSize = new Vector3Serializable(temp, ConstantValues.PlantConsts.FruitPrefabScale.y * prefabScale,
            ConstantValues.PlantConsts.FruitPrefabScale.z * prefabScale);
    }

    protected void SetFruitMaxSize(float value, float variancePercent)
    {
        FruitMaxSize = value + UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
    }

    protected void SetFruitGrowthRate(float timeToMaturity, TimeUnits unit, float variancePercent)
    {
        float value;
        switch (unit)
        {
            case TimeUnits.Year:
                value = timeToMaturity * GameTime.YEAR / ((FruitMaxSize - FruitInitialSize.x) / ConstantValues.PlantConsts.GrowthAmt);
                break;
            case TimeUnits.Season:
                value = timeToMaturity * GameTime.SEASON / ((FruitMaxSize - FruitInitialSize.x) / ConstantValues.PlantConsts.GrowthAmt);
                break;
            case TimeUnits.Day:
                value = timeToMaturity * GameTime.DAY / ((FruitMaxSize - FruitInitialSize.x) / ConstantValues.PlantConsts.GrowthAmt);
                break;
            case TimeUnits.Hour:
                value = timeToMaturity * GameTime.HOUR / ((FruitMaxSize - FruitInitialSize.x) / ConstantValues.PlantConsts.GrowthAmt);
                break;
            default:
                value = 0;
                Debug.LogError("Invalid TimeUnit");
                break;
        }
        FruitGrowthRate = value + UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
    }


    //public Vector3Serializable GetVariedBranchRotation(bool randomDirection)
    //{
    //    float x, y, z;
    //    if (randomDirection)
    //    {
    //        Debug.LogWarning("Random direction isn't working properly.");
    //        x = BranchRotation.x == 0 ? 0 : RandomSign.Sign() * (BranchRotation.x + UnityEngine.Random.Range(-BranchRotation.w, BranchRotation.w));
    //        y = BranchRotation.y == 0 ? 0 : RandomSign.Sign() * (BranchRotation.y + UnityEngine.Random.Range(-BranchRotation.w, BranchRotation.w));
    //        z = BranchRotation.z == 0 ? 0 : RandomSign.Sign() * (BranchRotation.z + UnityEngine.Random.Range(-BranchRotation.w, BranchRotation.w));
    //    }
    //    else
    //    {
    //        x = BranchRotation.x == 0 ? 0 : (BranchRotation.x + UnityEngine.Random.Range(-BranchRotation.w, BranchRotation.w));
    //        y = BranchRotation.y == 0 ? 0 : (BranchRotation.y + UnityEngine.Random.Range(-BranchRotation.w, BranchRotation.w));
    //        z = BranchRotation.z == 0 ? 0 : (BranchRotation.z + UnityEngine.Random.Range(-BranchRotation.w, BranchRotation.w));
    //    }
    //    return new Vector3Serializable(x, y, z);
    //}

    //public Vector3Serializable GetVariedTrunkRotation(bool randomDirection)
    //{
    //    float x, y, z;
    //    if (randomDirection)
    //    {
    //        x = TrunkRotation.x == 0 ? 0 : RandomSign.Sign() * (TrunkRotation.x + UnityEngine.Random.Range(-TrunkRotation.w, TrunkRotation.w));
    //        y = TrunkRotation.y == 0 ? 0 : RandomSign.Sign() * (TrunkRotation.y + UnityEngine.Random.Range(-TrunkRotation.w, TrunkRotation.w));
    //        z = TrunkRotation.z == 0 ? 0 : RandomSign.Sign() * (TrunkRotation.z + UnityEngine.Random.Range(-TrunkRotation.w, TrunkRotation.w));
    //    }
    //    else
    //    {
    //        x = TrunkRotation.x == 0 ? 0 : (TrunkRotation.x + UnityEngine.Random.Range(-TrunkRotation.w, TrunkRotation.w));
    //        y = TrunkRotation.y == 0 ? 0 : (TrunkRotation.y + UnityEngine.Random.Range(-TrunkRotation.w, TrunkRotation.w));
    //        z = TrunkRotation.z == 0 ? 0 : (TrunkRotation.z + UnityEngine.Random.Range(-TrunkRotation.w, TrunkRotation.w));
    //    }
    //    return new Vector3Serializable(x, y, z);
    //}

    public override string ToString()
    {
        return GetType().Name;
    }
}

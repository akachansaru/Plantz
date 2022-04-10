using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
// The species dictates temp, light, humidity requirements, branching pattern, flower color, thickness of stems, stem/leaf growth rate
public class Species
{
    [SerializeField] private string speciesName;
    [SerializeField] private Genus genus; // Converted to ECS
    [SerializeField] private Rarity rarity; // Converted to ECS

    [Space]
    [SerializeField] private Stat stemGrowthRate; // Converted to ECS // For widening the stem
    [SerializeField] private Stat stemMaxSize; // Converted to ECS
    [SerializeField] private Stat stemInitialSize; // Converted to ECS
    [SerializeField] private Stat trunkRotationX; // Converted to ECS
    [SerializeField] private Stat trunkRotationY; // Converted to ECS
    [SerializeField] private Stat trunkRotationZ; // Converted to ECS
    //[SerializeField] private Vector4Serializable trunkRotation;
    [SerializeField] private Vector4Serializable branchRotation; // Converted to ECS

    [Space]
    [SerializeField] private List<Biomes> nativeBiomes; // Converted to ECS
    [SerializeField] private List<Seasons> growingSeasons;
    [SerializeField] private List<Seasons> floweringSeasons;
    //[SerializeField] private bool onNewGrowth;
    [SerializeField] private float stemGrowthAmt; // Converted to ECS // Will be 10% of StemInitialSize
    [SerializeField] private int branchesPerCycles;  // Converted to ECS // Number of branches spiraling/whorling before coming back to the beginning. 4 would be 1 branch every 90 degrees
    [SerializeField] private int internodes; // Converted to ECS

    public string SpeciesName { get { return speciesName; } set { speciesName = value; } }
    public Genus Genus { get { return genus; } set { genus = value; } }
    public Rarity Rarity { get { return rarity; } protected set { rarity = value; } }

    // Stats. These are what can combine when pollinating a plant
    public List<Stat> CultivarStats { get; set; } = new List<Stat>(); // All cultivar stats are added here upon creation
    public Stat StemGrowthRate { get { return stemGrowthRate; } private set { stemGrowthRate = value; } }
    public Stat StemMaxSize { get { return stemMaxSize; } private set { stemMaxSize = value; } }
    public Stat StemInitialSize { get { return stemInitialSize; } private set { stemInitialSize = value; } }
    public Stat TrunkRotationX { get { return trunkRotationX; } private set { trunkRotationX = value; } }
    public Stat TrunkRotationY { get { return trunkRotationY; } private set { trunkRotationY = value; } }
    public Stat TrunkRotationZ { get { return trunkRotationZ; } private set { trunkRotationZ = value; } }

    //public Vector4Serializable TrunkRotation { get { return trunkRotation; } set { trunkRotation = value; } } // Need to make this a stat
    public Vector4Serializable BranchRotation { get { return branchRotation; } set { branchRotation = value; } } // Need to make this a stat

    // Other plant properties. These are set for the species (won't change with cultivars) and will need a special way to combine for hybrids
    public List<Biomes> NativeBiomes { get { return nativeBiomes; } protected set { nativeBiomes = value; } }
    public List<Seasons> GrowingSeasons { get { return growingSeasons; } protected set { growingSeasons = value; } }
    public List<Seasons> FloweringSeasons { get { return floweringSeasons; } protected set { floweringSeasons = value; } }
    //public bool OnNewGrowth { get { return onNewGrowth; } protected set { onNewGrowth = value; } }
    public float StemGrowthAmt { get { return stemGrowthAmt; } private set { stemGrowthAmt = value; } } // Will be 10% of StemInitialSize
    public int BranchesPerCycle { get { return branchesPerCycles; } protected set { branchesPerCycles = value; } }
    public int Internodes { get { return internodes; } protected set { internodes = value; } }

    public Species() { }

    public Species(string speciesName, Genus genus, Rarity rarity, List<Biomes> nativeBiomes, List<Seasons> growingSeasons, 
                   Stat stemMaxSize, Stat stemInitialSize, Stat stemGrowthRate, TimeUnits stemGrowthRateUnit,
                   List<Seasons> floweringSeasons, int branchesPerCycle, Stat trunkRotationX, Stat trunkRotationY, 
                   Stat trunkRotationZ, Vector4Serializable branchRotation, int internodes) 
    {
        SpeciesName = speciesName;
        Genus = genus;
        Rarity = rarity;
        NativeBiomes = nativeBiomes;
        GrowingSeasons = growingSeasons;
        StemMaxSize = stemMaxSize;
        StemInitialSize = stemInitialSize;
        SetStemGrowthRate(stemGrowthRate.Value, stemGrowthRateUnit, (stemGrowthRate.Max / stemGrowthRate.Value) - 1); // Not sure if this math is right
        FloweringSeasons = floweringSeasons;
        BranchesPerCycle = branchesPerCycle;
        TrunkRotationX = trunkRotationX;
        TrunkRotationY = trunkRotationY;
        TrunkRotationZ = trunkRotationZ;

        //TrunkRotation = trunkRotation;
        BranchRotation = branchRotation;
        Internodes = internodes;
    }

    /// <summary>
    /// For pollinating
    /// </summary>
    /// <param name="plant"></param>
    /// <param name="pollen"></param>
    public Species(Plant plant, Pollen pollen)
    {
        Species plantSpecies = plant.Taxonomy.Species;
        Species pollenSpecies = pollen.Taxonomy.Species;

        if (plantSpecies.SpeciesName == pollenSpecies.SpeciesName)
        {
            SpeciesName = plantSpecies.SpeciesName;
            Genus = plantSpecies.Genus;
            Rarity = plantSpecies.Rarity;
            GrowingSeasons = plantSpecies.GrowingSeasons;
            NativeBiomes = plantSpecies.NativeBiomes;

            List<Stat> cultivarStats = new List<Stat>(); // might not need this. Will add any stats that break the cultivar barrier to this to create the new cultivar
            StemGrowthRate = plantSpecies.StemGrowthRate.CombineSpecies(pollenSpecies.StemGrowthRate, cultivarStats);
            StemMaxSize = plantSpecies.StemMaxSize.CombineSpecies(pollenSpecies.StemMaxSize, cultivarStats);
            StemInitialSize = plantSpecies.StemInitialSize.CombineSpecies(pollenSpecies.StemInitialSize, cultivarStats);

            Genus.LeafGrowthRate = plantSpecies.Genus.LeafGrowthRate.CombineSpecies(pollenSpecies.Genus.LeafGrowthRate, cultivarStats);
            // Add the other Genus stats here

            if (cultivarStats.Count > 0)
            {
                // Create new cultivar with all the stats in cultivarStats. Player won't be notified until they harvest the fruit.
                // At that point the game will check if the same cultivar has already been created.
                Debug.Log("Created cultivar with " + cultivarStats.Count + " special stats! Could be a new one.");
                CultivarStats.AddRange(cultivarStats); // This will save the stats in Species
            }
        }
        else if (plantSpecies.Genus.GenusName == pollenSpecies.Genus.GenusName) // Hybrid
        {
            Debug.Log("Hybrid created");
            SpeciesName = plantSpecies.Genus.GenusName + " " + plantSpecies.SpeciesName + " X " +
                          pollenSpecies.Genus.GenusName + " " + pollenSpecies.SpeciesName;
            Genus = plantSpecies.Genus;
            Rarity = (Rarity) Mathf.Max((int) plantSpecies.Rarity, (int) pollenSpecies.Rarity); // Make this one more rarity than the max?
            GrowingSeasons = plantSpecies.GrowingSeasons;
            GrowingSeasons.AddRange(pollenSpecies.GrowingSeasons);
            NativeBiomes = plantSpecies.NativeBiomes;
            NativeBiomes.AddRange(pollenSpecies.NativeBiomes);
            StemGrowthRate = plantSpecies.StemGrowthRate.CombineHybrid(pollenSpecies.StemGrowthRate);
            StemMaxSize = plantSpecies.StemMaxSize.CombineHybrid(pollenSpecies.StemMaxSize);
            StemInitialSize = plantSpecies.StemInitialSize.CombineHybrid(pollenSpecies.StemInitialSize);
        }
        else if (plantSpecies.Genus.Family.FamilyName == pollenSpecies.Genus.Family.FamilyName)
        {
            Debug.LogError("family Hybrids not implemented");
        }
    }

    // Temparature range based on all of the biomes the plant can live in
    public RequirementRange GetTemperatureRange()
    {
        float low = 0f;
        float high = 0f;
        //float ave = 0;

        foreach (Biomes biome in NativeBiomes)
        {
            RequirementRange range = Biome.GetTemperatureRange(biome);
            //ave += range.Ave;

            if (range.Low < low)
            {
                low = range.Low;
            }
            if (range.High > high)
            {
                high = range.High;
            }
        }

        //ave /= NativeBiomes.Count;

        //return new RequirementRange(ave, high - ave, 0f);
        return new RequirementRange(low, high, 0f);
    }

    public RequirementRange GetHumidityRange()
    {
        float low = 0f;
        float high = 0f;
        //float ave = 0;

        foreach (Biomes biome in NativeBiomes)
        {
            RequirementRange range = Biome.GetHumidityRange(biome);
            //ave += range.Ave;

            if (range.Low < low)
            {
                low = range.Low;
            }
            if (range.High > high)
            {
                high = range.High;
            }
        }

        // ave /= NativeBiomes.Count;
        //RequirementRange newRange = new RequirementRange(ave, high - ave, 0f);
        RequirementRange newRange = new RequirementRange(low, high, 0f);
        //if (newRange.Low < 0)
        //{
        //    newRange.Low = 0;
        //}
        return newRange;
    }

    public RequirementRange GetSoilMoistureRange()
    {
        float low = 0f;
        float high = 0f;
        //float ave = 0;

        foreach (Biomes biome in NativeBiomes)
        {
            float moisture = Soil.GetFullMoistureLevel(biome);
            // ave += moisture;
            if (moisture < low)
            {
                low = moisture;
            }
            if (moisture > high)
            {
                high = moisture;
            }
        }

        // ave /= NativeBiomes.Count;
        //RequirementRange newRange = new RequirementRange(ave, high - ave, 0f);
        RequirementRange newRange = new RequirementRange(low, high, 0f);
        //if (newRange.Low < 0)
        //{
        //    newRange.Low = 0;
        //}
        return newRange;
    }

    /// <summary>
    /// SetStemGrowthRate(1, year) with only one growing season and SetStemGrowthRate(1, season) should be the same
    /// </summary>
    /// <param name="timeToMaturity"></param>
    /// <param name="unit"></param>
    /// <param name="variancePercent"></param>
    protected void SetStemGrowthRate(float timeToMaturity, TimeUnits unit, float variancePercent)
    {
        if (StemInitialSize.Value == 0 || StemInitialSize == null)
        {
            Debug.LogError("StemInitialSize needs to be initialized first.");
        }
        else
        {
            StemGrowthAmt = ConstantValues.PlantConsts.StemGrowthPercent * StemInitialSize.Value;
        }

        int numSeasons = Enum.GetNames(typeof(Seasons)).Length;
        int growingSeasonMultiplier = GrowingSeasons.Count == numSeasons ? 1 : numSeasons - GrowingSeasons.Count; // So it will take into account the off seasons
        float value;
        switch (unit)
        {
            case TimeUnits.Year:
                if (timeToMaturity < 1)
                {
                    Debug.LogError("Use seasons instead. Calculation will be wrong.");
                }
                value = (timeToMaturity * GameTime.YEAR / ((StemMaxSize.Value - StemInitialSize.Value) / StemGrowthAmt)) / growingSeasonMultiplier;
                break;
            case TimeUnits.Season:
                if (timeToMaturity >= 4)
                {
                    Debug.LogError("Use years instead. Calculation will be wrong.");
                }
                value = (timeToMaturity * GameTime.SEASON / ((StemMaxSize.Value - StemInitialSize.Value) / StemGrowthAmt));
                break;
            case TimeUnits.Day:
                value = (timeToMaturity * GameTime.DAY / ((StemMaxSize.Value - StemInitialSize.Value) / StemGrowthAmt));
                break;
            case TimeUnits.Hour:
                value = (timeToMaturity * GameTime.HOUR / ((StemMaxSize.Value - StemInitialSize.Value) / StemGrowthAmt));
                break;
            case TimeUnits.None: // If passing in the exact value it should be 
                value = timeToMaturity;
                break;
            default:
                value = 0;
                Debug.LogError("Invalid TimeUnit");
                break;
        }

        StemGrowthRate = new Stat(value, value + value * variancePercent, ConstantValues.StatID.StemGrowthRate);
    }


    //protected void SetStemMaxSize(float value, float variancePercent)
    //{
    //    StemMaxSize = new Stat(value, value + value * variancePercent, ConstantValues.StatID.StemMaxSize); //value + UnityEngine.Random.Range(-value * variancePercent, value * variancePercent);
    //}

    //protected void SetStemInitialSize(float value, float variancePercent)
    //{
    //    StemInitialSize = new Stat(value, value + value * variancePercent, ConstantValues.StatID.StemInitialSize);
    //}

    public Vector3Serializable GetVariedBranchRotation(bool randomDirection)
    {
        float x, y, z;
        if (randomDirection)
        {
            Debug.LogWarning("Random direction isn't working properly.");
            x = BranchRotation.x == 0 ? 0 : RandomSign.Sign() * (BranchRotation.x + UnityEngine.Random.Range(-BranchRotation.x * BranchRotation.w, BranchRotation.x * BranchRotation.w));
            y = BranchRotation.y == 0 ? 0 : RandomSign.Sign() * (BranchRotation.y + UnityEngine.Random.Range(-BranchRotation.y * BranchRotation.w, BranchRotation.y * BranchRotation.w));
            z = BranchRotation.z == 0 ? 0 : RandomSign.Sign() * (BranchRotation.z + UnityEngine.Random.Range(-BranchRotation.z * BranchRotation.w, BranchRotation.z * BranchRotation.w));
        }
        else
        {
            x = BranchRotation.x == 0 ? 0 : (BranchRotation.x + UnityEngine.Random.Range(-BranchRotation.x * BranchRotation.w, BranchRotation.x * BranchRotation.w));
            y = BranchRotation.y == 0 ? 0 : (BranchRotation.y + UnityEngine.Random.Range(-BranchRotation.y * BranchRotation.w, BranchRotation.y * BranchRotation.w));
            z = BranchRotation.z == 0 ? 0 : (BranchRotation.z + UnityEngine.Random.Range(-BranchRotation.z * BranchRotation.w, BranchRotation.z * BranchRotation.w));
        }
        return new Vector3Serializable(x, y, z);
    }

    public Vector3Serializable GetVariedTrunkRotation(bool randomDirection)
    {
        float x, y, z;
        //if (randomDirection)
        //{
        //    x = TrunkRotation.x == 0 ? 0 : RandomSign.Sign() * (TrunkRotation.x + UnityEngine.Random.Range(-TrunkRotation.x * TrunkRotation.w, TrunkRotation.x * TrunkRotation.w));
        //    y = TrunkRotation.y == 0 ? 0 : RandomSign.Sign() * (TrunkRotation.y + UnityEngine.Random.Range(-TrunkRotation.y * TrunkRotation.w, TrunkRotation.y * TrunkRotation.w));
        //    z = TrunkRotation.z == 0 ? 0 : RandomSign.Sign() * (TrunkRotation.z + UnityEngine.Random.Range(-TrunkRotation.z * TrunkRotation.w, TrunkRotation.z * TrunkRotation.w));
        //}
        //else
        //{
        //    x = TrunkRotation.x == 0 ? 0 : (TrunkRotation.x + UnityEngine.Random.Range(-TrunkRotation.x * TrunkRotation.w, TrunkRotation.x * TrunkRotation.w));
        //    y = TrunkRotation.y == 0 ? 0 : (TrunkRotation.y + UnityEngine.Random.Range(-TrunkRotation.y * TrunkRotation.w, TrunkRotation.y * TrunkRotation.w));
        //    z = TrunkRotation.z == 0 ? 0 : (TrunkRotation.z + UnityEngine.Random.Range(-TrunkRotation.z * TrunkRotation.w, TrunkRotation.z * TrunkRotation.w));
        //}
        x = TrunkRotationX.GetValueWithVariance();
        y = TrunkRotationY.GetValueWithVariance();
        z = TrunkRotationZ.GetValueWithVariance();
        return new Vector3Serializable(x, y, z);
    }

    public override string ToString()
    {
        return speciesName;
    }

    // I think I implemented Equals and GetHashCode correctly but not 100% sure so only checking speciesName above
    public override bool Equals(object obj)
    {
        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Species species = obj as Species;
            return speciesName == species.speciesName;
        }
    }

    public override int GetHashCode()
    {
        return speciesName.GetHashCode();
    }
}

using System;
using System.Collections.Generic;
using Assets.Scripts.Utilities;

// Add new families/genera/species here
// UNDONE add mutations that will be rare and sell for higher prices
// UNDONE add rarity levels for the parameters. eg largest and smallest 5% of the probability of plant size will be rare. Need to make separate random ranges to see what rarity tier, then calculate the paramater within that tier
#region Family Implementation
/// <summary>
/// Characterized by alternate branching pattern
/// </summary>
[Serializable]
public class FAlternate : Family
{
    public FAlternate()
    {
        FamilyName = GetType().Name;
        BranchPattern = BranchPatterns.Alternate;
        Rarity = Rarity.Common;
    }
}

/// <summary>
/// Characterized by opposite branching pattern
/// </summary>
[Serializable]
public class FOpposite : Family
{
    public FOpposite()
    {
        FamilyName = GetType().Name;
        BranchPattern = BranchPatterns.Opposite;
        Rarity = Rarity.Uncommon;
    }
}

/// <summary>
/// Characterized by whorled branching pattern
/// </summary>
[Serializable]
public class FWhorled : Family
{
    public FWhorled()
    {
        FamilyName = GetType().Name;
        BranchPattern = BranchPatterns.Whorled;
        Rarity = Rarity.Rare;
    }
}
#endregion

#region Genus Implementation
/// <summary>
/// Characterized by a tall, narrow silhouette
/// </summary>
[Serializable]
public class G1 : Genus
{
    public G1()
    {
        GenusName = GetType().Name;
        Family = new FAlternate();
        Rarity = Rarity.Common;
        SetPlantMaxSize(200, 0.05f);
        SetTimeBetweenPlantGrowth(2f, TimeUnits.Day, 0.1f);
        //BranchesPerCycle = 8;
        //TrunkRotation = new Vector4Serializable(5f, 0f, 0f, 0.1f);
        //BranchRotation = new Vector4Serializable(40f, 0f, 0f, 0.15f);
        //Internodes = 7;

        // Leaves
        SetNodesBetweenLeaves(2, 0f); // TODO make nodesBetweenLeaves not have a variance
        LeavesPerNode = 4;
        SetLeafInitialSize(0.1f, 0.05f);
        SetLeafMaxSize(1.4f, 0.1f);
        SetLeafGrowthRate(0.5f, TimeUnits.Season, 0.1f); // Fully mature in one season

        // Flower
        SetNodesBetweenFlowers(3, 0);
        FlowersPerNode = 2;
        SetFlowerInitialSize(0.1f, 0.05f);
        SetFlowerMaxSize(2f, 0.1f);
        SetFlowerGrowthRate(0.001f, TimeUnits.Season, 0f); // Fully mature in 1/4 of a season

        // Fruit
        SetFruitInitialSize(0.1f, 0.05f);
        SetFruitMaxSize(2f, 0.1f);
        SetFruitGrowthRate(0.25f, TimeUnits.Season, 0.1f); // Fully mature in 1/4 of a season
    }
}

/// <summary>
/// Characterized by a round silhouette
/// </summary>
[Serializable]
public class G2 : Genus
{
    public G2()
    {
        GenusName = GetType().Name;
        Family = new FAlternate();
        Rarity = Rarity.Rare;
        SetPlantMaxSize(70, 0.05f);
        SetTimeBetweenPlantGrowth(0.5f, TimeUnits.Season, 0.1f);
        // BranchesPerCycle = 4; // One branch every 90 degrees
        //TrunkRotation = new Vector4Serializable(15f, 0f, 0f, 0.3f);
        //BranchRotation = new Vector4Serializable(45f, 0f, 0f, 0.3f);
        // Internodes = 4;

        // Leaves
        SetNodesBetweenLeaves(2, 0);
        LeavesPerNode = 4;
        SetLeafInitialSize(0.1f, 0.05f);
        SetLeafMaxSize(1.4f, 0.2f);
        SetLeafGrowthRate(1f, TimeUnits.Season, 0.1f); // Fully mature in one season

        // Flower
        SetNodesBetweenFlowers(3, 0);
        FlowersPerNode = 2;
        SetFlowerInitialSize(0.1f, 0.05f);
        SetFlowerMaxSize(2f, 0.05f);
        SetFlowerGrowthRate(0.75f, TimeUnits.Season, 0.1f); // Fully mature in 3/4 of a season

        // Fruit
        SetFruitInitialSize(0.1f, 0.05f);
        SetFruitMaxSize(2f, 0.1f);
        SetFruitGrowthRate(0.25f, TimeUnits.Season, 0.1f); // Fully mature in 1/4 of a season
    }
}

[Serializable]
public class GSmall : Genus
{
    public GSmall()
    {
        GenusName = GetType().Name;
        Family = new FOpposite();
        Rarity = Rarity.Common;
        SetPlantMaxSize(10, 0.02f);
        SetTimeBetweenPlantGrowth(1f, TimeUnits.Day, 0.1f);
        //BranchesPerCycle = 6; // One branch every 90 degrees
        //TrunkRotation = new Vector4Serializable(30f, 0f, 0f, 0.03f);
        //BranchRotation = new Vector4Serializable(55f, 0f, 0f, 0.05f);
        //Internodes = 2;

        // Leaves
        SetNodesBetweenLeaves(1, 0);
        LeavesPerNode = 4;
        SetLeafInitialSize(0.1f, 0.03f);
        SetLeafMaxSize(1.4f, 0.05f);
        SetLeafGrowthRate(1f, TimeUnits.Season, 0.1f); // Fully mature in one season

        // Flower
        SetNodesBetweenFlowers(1, 0);
        FlowersPerNode = 4;
        SetFlowerInitialSize(0.1f, 0.01f);
        SetFlowerMaxSize(2f, 0.01f);
        SetFlowerGrowthRate(0.75f, TimeUnits.Season, 0.1f); // Fully mature in 3/4 of a season

        // Fruit
        SetFruitInitialSize(0.1f, 0.05f);
        SetFruitMaxSize(2f, 0.1f);
        SetFruitGrowthRate(0.25f, TimeUnits.Season, 0.1f); // Fully mature in 1/4 of a season
    }
}

[Serializable]
public class GFastGrowing : Genus
{
    public GFastGrowing()
    {
        GenusName = GetType().Name;
        Family = new FWhorled();
        Rarity = Rarity.Common;
        SetPlantMaxSize(200, 0.05f);
        SetTimeBetweenPlantGrowth(2f, TimeUnits.Day, 0.05f);
        //BranchesPerCycle = 8; // One branch every 45 degrees
        //TrunkRotation = new Vector4Serializable(5f, 0f, 0f, 0.05f);
        //BranchRotation = new Vector4Serializable(45f, 15f, 0f, 0.15f);
        //Internodes = 4;

        // Leaves
        SetNodesBetweenLeaves(3, 0);
        LeavesPerNode = 4;
        SetLeafInitialSize(0.1f, 0.1f);
        SetLeafMaxSize(0.75f, 0.1f);
        SetLeafGrowthRate(0.25f, TimeUnits.Day, 0.1f); // Fully mature in about a day

        // Flower
        SetNodesBetweenFlowers(2, 0);
        FlowersPerNode = 2;
        SetFlowerInitialSize(0.1f, 0.05f);
        SetFlowerMaxSize(1f, 0.05f);
        SetFlowerGrowthRate(.25f, TimeUnits.Day, 0.1f); // Fully mature in about a day

        // Fruit
        SetFruitInitialSize(0.1f, 0.05f);
        SetFruitMaxSize(2f, 0.1f);
        SetFruitGrowthRate(0.25f, TimeUnits.Season, 0.1f); // Fully mature in 1/4 of a season
    }
}
#endregion

#region Species Implementation

public class SpeciesImplementation
{
    public List<Taxonomy> GenerateStartingSpeciesList()
    {
        return new List<Taxonomy> 
        { 
            new Taxonomy(s1), new Taxonomy(s2), new Taxonomy(s3), new Taxonomy(s4), 
            new Taxonomy(s5), new Taxonomy(s6), new Taxonomy(s7) 
        };
    }

    Species s1 = new Species
    (
        "S1",                                                                       // Species name
        new G1(),                                                                   // Genus
        Rarity.Common,                                                              // Rarity
        new List<Biomes> { Biomes.Forest, Biomes.Desert },                          // Native biomes
        new List<Seasons> { Seasons.Spring },                                       // Growing seasons
        new Stat(1, 1.1f, ConstantValues.StatID.StemMaxSize),                       // StemMaxSize
        new Stat(0.1f, 0.11f, ConstantValues.StatID.StemInitialSize),               // StemInitialSize
        new Stat(2, 2.2f, ConstantValues.StatID.StemGrowthRate), TimeUnits.Year,    // StemGrowthRate
        new List<Seasons> { Seasons.Spring },                                       // Flowering season
        8,                                                                          // Branches per cycle
        new Stat(0, 5, ConstantValues.StatID.TrunkRotationX),                       // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationY),                       // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationZ),                       // Trunk rotation
        new Vector4Serializable(40f, 10, 40f, 0.15f),                               // Branch rotation
        6                                                                           // Internodes
    );

    Species s2 = new Species
    (
        "S2",                                                                       // Species name
        new G1(),                                                                   // Genus
        Rarity.Uncommon,                                                            // Rarity
        new List<Biomes> { Biomes.Forest, Biomes.Desert },                          // Native biomes
        new List<Seasons> { Seasons.Spring },                                       // Growing seasons
        new Stat(1, 1.1f, Rarity.UltraRare, ConstantValues.StatID.StemMaxSize),     // StemMaxSize
        new Stat(0.07f, 0.071f, Rarity.UltraRare, ConstantValues.StatID.StemInitialSize),   // StemInitialSize
        new Stat(4, 4.4f, ConstantValues.StatID.StemGrowthRate), TimeUnits.Year,    // StemGrowthRate
        new List<Seasons> { Seasons.Spring },                                       // Flowering season
        6,                                                                          // Branches per cycle
        new Stat(0, 35, ConstantValues.StatID.TrunkRotationX),                      // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationY),                       // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationZ),                       // Trunk rotation
        new Vector4Serializable(60f, 10, 30f, 0.15f),                               // Branch rotation
        4                                                                           // Internodes
    );

    Species s3 = new Species
    (
        "S3",                                                                       // Species name
        new G2(),                                                                   // Genus
        Rarity.Rare,                                                                // Rarity
        new List<Biomes> { Biomes.Forest },                                         // Native biomes
        new List<Seasons> { Seasons.Spring },                                       // Growing seasons
        new Stat(0.5f, 0.55f, ConstantValues.StatID.StemMaxSize),                   // StemMaxSize
        new Stat(0.05f, 0.055f, ConstantValues.StatID.StemInitialSize),             // StemInitialSize
        new Stat(6, 6.6f, ConstantValues.StatID.StemGrowthRate), TimeUnits.Year,    // StemGrowthRate
        new List<Seasons> { Seasons.Spring },                                       // Flowering season
        8,                                                                          // Branches per cycle
        new Stat(0, 45, ConstantValues.StatID.TrunkRotationX),                      // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationY),                       // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationZ),                       // Trunk rotation
        new Vector4Serializable(40f, 10, 20f, 0.15f),                               // Branch rotation
        7                                                                           // Internodes
    );

    Species s4 = new Species
    (
        "S4",                                                                       // Species name
        new G2(),                                                                   // Genus
        Rarity.Uncommon,                                                            // Rarity
        new List<Biomes> { Biomes.Forest },                                         // Native biomes
        new List<Seasons> { Seasons.Spring },                                       // Growing seasons
        new Stat(1, 1.2f, ConstantValues.StatID.StemMaxSize),                       // StemMaxSize
        new Stat(0.1f, 0.12f, ConstantValues.StatID.StemInitialSize),               // StemInitialSize
        new Stat(2, 2.2f, ConstantValues.StatID.StemGrowthRate), TimeUnits.Year,    // StemGrowthRate
        new List<Seasons> { Seasons.Spring },                                       // Flowering season
        6,                                                                          // Branches per cycle
        new Stat(0, 15, ConstantValues.StatID.TrunkRotationX),                      // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationY),                       // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationZ),                       // Trunk rotation
        new Vector4Serializable(30f, 0, 15f, 0.25f),                                // Branch rotation
        4                                                                           // Internodes
    );

    Species s5 = new Species
    (
        "S5",                                                                       // Species name
        new GSmall(),                                                               // Genus
        Rarity.UltraRare,                                                           // Rarity
        new List<Biomes> { Biomes.Forest },                                         // Native biomes
        new List<Seasons> { Seasons.Spring },                                       // Growing seasons
        new Stat(0.5f, 0.525f, ConstantValues.StatID.StemMaxSize),                  // StemMaxSize
        new Stat(0.04f, 0.042f, ConstantValues.StatID.StemInitialSize),             // StemInitialSize
        new Stat(15, 15.75f, ConstantValues.StatID.StemGrowthRate), TimeUnits.Year, // StemGrowthRate
        new List<Seasons> { Seasons.Spring },                                       // Flowering season
        8,                                                                          // Branches per cycle
        new Stat(0, 6, ConstantValues.StatID.TrunkRotationX),                       // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationY),                       // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationZ),                       // Trunk rotation
        new Vector4Serializable(10f, 5, 5f, 0.15f),                                 // Branch rotation
        2                                                                           // Internodes
    );

    Species s6 = new Species
    (
        "S6",                                                                       // Species name
        new GSmall(),                                                               // Genus
        Rarity.Common,                                                              // Rarity
        new List<Biomes> { Biomes.Forest },                                         // Native biomes
        new List<Seasons> { Seasons.Spring, Seasons.Summer },                       // Growing seasons
        new Stat(0.2f, 0.204f, ConstantValues.StatID.StemMaxSize),                  // StemMaxSize
        new Stat(0.01f, 0.015f, ConstantValues.StatID.StemInitialSize),             // StemInitialSize
        new Stat(2, 2.2f, ConstantValues.StatID.StemGrowthRate), TimeUnits.Year,    // StemGrowthRate
        new List<Seasons> { Seasons.Spring },                                       // Flowering season
        6,                                                                          // Branches per cycle
        new Stat(0, 6, ConstantValues.StatID.TrunkRotationX),                       // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationY),                       // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationZ),                       // Trunk rotation
        new Vector4Serializable(40f, 10, 0f, 0.15f),                                // Branch rotation
        2                                                                           // Internodes
    );

    Species s7 = new Species
    (
        "S7",                                                                       // Species name
        new GFastGrowing(),                                                         // Genus
        Rarity.Common,                                                              // Rarity
        new List<Biomes> { Biomes.Forest, Biomes.Desert, Biomes.Swamp },            // Native biomes
        new List<Seasons> { Seasons.Spring },                                       // Growing seasons
        new Stat(1, 1.1f, ConstantValues.StatID.StemMaxSize),                       // StemMaxSize
        new Stat(0.05f, 0.055f, ConstantValues.StatID.StemInitialSize),             // StemInitialSize
        new Stat(3, 3.3f, ConstantValues.StatID.StemGrowthRate), TimeUnits.Year,    // StemGrowthRate
        new List<Seasons> { Seasons.Spring },                                       // Flowering season
        4,                                                                          // Branches per cycle
        new Stat(0, 6, ConstantValues.StatID.TrunkRotationX),                       // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationY),                       // Trunk rotation
        new Stat(0, 1, ConstantValues.StatID.TrunkRotationZ),                       // Trunk rotation
        new Vector4Serializable(40f, 5, 15f, 0.15f),                                // Branch rotation
        7                                                                           // Internodes
    );
}
#endregion

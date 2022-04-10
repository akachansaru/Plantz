using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BranchPatterns { Opposite, Alternate, Whorled }; // UNDONE: Add random pattern
public enum Rarity { Common = 1, Uncommon = 2, Rare = 4, UltraRare = 16 }

[Serializable]
public class Taxonomy
{
    public static int SpeciesMultiplier { get; }  = 2;
    public static int GenusMultiplier { get; } = 4;
    public static int FamilyMultiplier { get; } = 8;

    [SerializeField] private Species species;
    [SerializeField] private string statID;

    public Species Species { get { return species; } }
    public string StatID { get { return statID; } set { statID = value; } }

    public Taxonomy(Species species)
    {
        this.species = species;
    }

    public Taxonomy(Biomes biome)
    {
        List<Species> biomeSpecies = GetSpeciesByBiome(biome);
        species = biomeSpecies[UnityEngine.Random.Range(0, biomeSpecies.Count)];
    }

    /// <summary>
    /// Creates a new Taxonomy when plant is pollinated with pollen. It will be a cross of both plants.
    /// </summary>
    /// <param name="plant"></param>
    /// <param name="pollen"></param>
    public Taxonomy(Plant plant, Pollen pollen)
    {
        if (plant.Taxonomy.Species.SpeciesName == pollen.Taxonomy.Species.SpeciesName)
        {
            //species = new Species(plant, pollen);
            species = CombineSpecies(plant, pollen);
        }
        else
        {
            Debug.LogWarning("Hybrids not yet implemented.");
            species = new Species(plant, pollen);
        }
    }

    private Species CombineSpecies(Plant plant, Pollen pollen)
    {
        Species plantSpecies = plant.Taxonomy.Species;
        Species pollenSpecies = pollen.Taxonomy.Species;

        List<Stat> cultivarStats = new List<Stat>(); // Will add any stats that break the cultivar barrier to this to create the new cultivar

        Stat stemGrowthRate = plantSpecies.StemGrowthRate.CombineSpecies(pollenSpecies.StemGrowthRate, cultivarStats);
        Stat stemMaxSize = plantSpecies.StemMaxSize.CombineSpecies(pollenSpecies.StemMaxSize, cultivarStats);
        Stat stemInitialSize = plantSpecies.StemInitialSize.CombineSpecies(pollenSpecies.StemInitialSize, cultivarStats);

        Stat genusLeafGrowthRate = plantSpecies.Genus.LeafGrowthRate.CombineSpecies(pollenSpecies.Genus.LeafGrowthRate, cultivarStats);
        // Add the other Genus stats here

        if (cultivarStats.Count > 0)
        {
            // Create new cultivar with all the stats in cultivarStats. Player won't be notified until they harvest the fruit.
            // At that point the game will check if the same cultivar has already been created and ask to name it if not.
            Debug.Log("Created cultivar with " + cultivarStats.Count + " special stats! Could be a new one.");

            foreach (Stat stat in cultivarStats)
            {
                StatID += stat.ID + ":";
            }
        }

        Species newSpecies = new Species(plantSpecies.SpeciesName, plantSpecies.Genus, plantSpecies.Rarity, plantSpecies.NativeBiomes, 
            plantSpecies.GrowingSeasons, stemMaxSize, stemInitialSize, stemGrowthRate, TimeUnits.None, // Using None so no calculation is done
            plantSpecies.FloweringSeasons, plantSpecies.BranchesPerCycle, plantSpecies.TrunkRotationX, plantSpecies.TrunkRotationY, plantSpecies.TrunkRotationZ,
            plantSpecies.BranchRotation, plantSpecies.Internodes);
        
        newSpecies.Genus.LeafGrowthRate = genusLeafGrowthRate; // BUG: Is this going to edit the entire Genus for other species?
        newSpecies.CultivarStats = cultivarStats;
        return newSpecies;
    }

    private void CombineHybrid()
    {
        //else if (plantSpecies.Genus.GenusName == pollenSpecies.Genus.GenusName) // Hybrid
        //{
        //    Debug.Log("Hybrid created");
        //    SpeciesName = plantSpecies.Genus.GenusName + " " + plantSpecies.SpeciesName + " X " +
        //                  pollenSpecies.Genus.GenusName + " " + pollenSpecies.SpeciesName;
        //    Genus = plantSpecies.Genus;
        //    Rarity = (Rarity)Mathf.Max((int)plantSpecies.Rarity, (int)pollenSpecies.Rarity); // Make this one more rarity than the max?
        //    GrowingSeasons = plantSpecies.GrowingSeasons;
        //    GrowingSeasons.AddRange(pollenSpecies.GrowingSeasons);
        //    NativeBiomes = plantSpecies.NativeBiomes;
        //    NativeBiomes.AddRange(pollenSpecies.NativeBiomes);
        //    StemGrowthRate = plantSpecies.StemGrowthRate.CombineHybrid(pollenSpecies.StemGrowthRate);
        //    StemMaxSize = plantSpecies.StemMaxSize.CombineHybrid(pollenSpecies.StemMaxSize);
        //    StemInitialSize = plantSpecies.StemInitialSize.CombineHybrid(pollenSpecies.StemInitialSize);
        //}
        //else if (plantSpecies.Genus.Family.FamilyName == pollenSpecies.Genus.Family.FamilyName)
        //{
        //    Debug.LogError("family Hybrids not implemented");
        //}
    }

    //private Species GetRandomSpecies(Biomes biome)
    //{
    //    List<Species> biomeSpecies = GetSpeciesByBiome(biome);
    //    Species species = biomeSpecies[UnityEngine.Random.Range(0, biomeSpecies.Count)];
    //    return species;
    //}

    private List<Taxonomy> GetTaxonomyList()
    {
        //IEnumerable<Type> allSpeciesTypes = typeof(Species).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Species)));
        //List<Species> allSpecies = new List<Species>();
        List<Taxonomy> allSpecies = GlobalControl.Instance.savedValues.AllSpecies;
        //foreach (Type t in allSpeciesTypes)
        //{
        //    allSpecies.Add(GetSpecies(t));
        //}
        return allSpecies;
    }

    private Species GetSpecies(Type type)
    {
        return (Species)Activator.CreateInstance(type);
    }

    private List<Species> GetSpeciesByBiome(Biomes biome)
    {
        List<Species> biomeSpecies = new List<Species>();

        foreach (Taxonomy tax in GetTaxonomyList())
        {
            if (tax.Species.NativeBiomes.Contains(biome))
            {
                biomeSpecies.Add(tax.species);
            }
        }
        return biomeSpecies;
    }

    public override string ToString()
    {
        return string.Format("{0} {1} {2}", Species.Genus.Family.FamilyName.ToUpper(), Species.Genus.GenusName, Species.SpeciesName.ToLower());
    }

    // Only looking at species name for equality. Not sure if this is what I want
    public override bool Equals(object obj)
    {
        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Taxonomy tax = obj as Taxonomy;
            return Species.SpeciesName == tax.Species.SpeciesName;
        }
    }

    public override int GetHashCode()
    {
        return Species.SpeciesName.GetHashCode();
    }

}

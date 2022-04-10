using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpeciesECS
{
    public int SpeciesNum { get; private set; }
    public Rarity Rarity { get; private set; }
    public GenusECS Genus { get; private set; }

    public StatECS WidenStemTime { get; private set; }
    public StatECS InitialStemWidth { get; private set; }
    public StatECS StemWidenPercent { get; private set; }
    public StatECS MaxStemWidth { get; private set; }
    public StatECS TrunkRotationX { get; private set; }
    public float3 BranchRotation { get; private set; }
    public int Internodes { get; private set; }
    public int BranchesPerCycle { get; private set; }
    public List<Biomes> NativeBiomes { get; private set; }

    public SpeciesECS(Entity rootEntity) 
    { 
        // go through each stat in rootEntity and add to this one's stats
    }

    public SpeciesECS(int speciesNum)
    {
        SpeciesNum = speciesNum;
        Rarity = GetRarity(speciesNum);
        Genus = new GenusECS(speciesNum);
        MaxStemWidth = GetMaxStemWidth(speciesNum);
        InitialStemWidth = GetInitialStemWidth(speciesNum);
        StemWidenPercent = GetStemWidenPercent(speciesNum);
        WidenStemTime = GetWidenStemTime(speciesNum);
        TrunkRotationX = GetTrunkRotationX(speciesNum);
        BranchRotation = GetBranchRotation(speciesNum);
        Internodes = GetInternodes(speciesNum);
        BranchesPerCycle = GetBranchesPerCycle(speciesNum);
        NativeBiomes = GetNativeBiomes(speciesNum);
    }

    public static List<int> GetSpeciesInBiome(Biomes biome)
    {
        List<int> species = new List<int>();
        int index = 0;
        while (GetNativeBiomes(index).Count > 0)
        {
            if (GetNativeBiomes(index).Contains(biome))
            {
                species.Add(index);
            }
            index++;
        }
        return species;
    }

    private static List<Biomes> GetNativeBiomes(int species)
    {
        switch (species)
        {
            case 0:
                return new List<Biomes> { Biomes.Forest, Biomes.Desert };
            case 1:
                return new List<Biomes> { Biomes.Forest, Biomes.Desert };
            case 2:
                return new List<Biomes> { Biomes.Swamp };
            case 3:
                return new List<Biomes> { Biomes.Desert };
            default:
                Debug.LogWarning("Invalid species num");
                return new List<Biomes>();
        }
    }

    private Rarity GetRarity(int species)
    {
        switch (species)
        {
            case 0:
                return Rarity.Common;
            case 1:
                return Rarity.Uncommon;
            case 2:
                return Rarity.Rare;
            case 3:
                return Rarity.UltraRare;
            default:
                Debug.LogError("Invalid species num");
                return Rarity.Common;
        }
    }

    private StatECS GetInitialStemWidth(int species)
    {
        switch (species)
        {
            case 0:
                return new StatECS(0.3f, 0.11f);
            case 1:
                return new StatECS(0.3f, 0.22f);
            case 2:
                return new StatECS(0.3f, 0.33f);
            case 3:
                return new StatECS(0.3f, 0.44f);
            default:
                Debug.LogError("Invalid species num");
                return new StatECS(1, 1.1f);
        }
    }


    private StatECS GetStemWidenPercent(int species)
    {
        switch (species)
        {
            case 0:
                return new StatECS(0.01f, 0.015f);
            case 1:
                return new StatECS(0.01f, 0.015f);
            case 2:
                return new StatECS(0.01f, 0.015f);
            case 3:
                return new StatECS(0.01f, 0.015f);
            default:
                Debug.LogError("Invalid species num");
                return new StatECS(0.01f, 0.015f);
        }
    }

    private StatECS GetMaxStemWidth(int species)
    {
        switch (species)
        {
            case 0:
                return new StatECS(0.2f, 0.22f);
            case 1:
                return new StatECS(0.3f, 0.33f);
            case 2:
                return new StatECS(0.4f, 0.44f);
            case 3:
                return new StatECS(0.5f, 0.55f);
            default:
                Debug.LogError("Invalid species num");
                return new StatECS(1, 1.1f);
        }
    }

    private StatECS GetWidenStemTime(int species)
    {
        switch (species)
        {
            case 0:
                return new StatECS(5f, 5.05f);
            case 1:
                return new StatECS(10f, 10.1f);
            case 2:
                return new StatECS(20f, 20.2f);
            case 3:
                return new StatECS(30f, 30.3f);
            default:
                Debug.LogError("Invalid species num");
                return new StatECS(2f, 2.2f);
        }
    }

    private StatECS GetTrunkRotationX(int species)
    {
        switch (species)
        {
            case 0:
                return new StatECS(0, 15);
            case 1:
                return new StatECS(0, 25);
            case 2:
                return new StatECS(0, 35);
            case 3:
                return new StatECS(0, 45);
            default:
                Debug.LogError("Invalid species num");
                return new StatECS(15, 16);
        }
    }

    private float3 GetBranchRotation(int species)
    {
        // Keep x small. Y will be adjusted in the branching pattern in GrowNextSystem
        switch (species)
        {
            case 0:
                return new float3(10, 0, 35);
            case 1:
                return new float3(10, 0, 35);
            case 2:
                return new float3(10, 0, 35);
            case 3:
                return new float3(10, 0, 35);
            default:
                Debug.LogError("Invalid species num");
                return new float3(0, 0, 45);
        }
    }

    private int GetInternodes(int species)
    {
        switch (species)
        {
            case 0:
                return 4;
            case 1:
                return 4;
            case 2:
                return 4;
            case 3:
                return 4;
            default:
                Debug.LogError("Invalid species num");
                return 4;
        }
    }

    private int GetBranchesPerCycle(int species)
    {
        switch (species)
        {
            case 0:
                return 4;
            case 1:
                return 4;
            case 2:
                return 4;
            case 3:
                return 4;
            default:
                Debug.LogError("Invalid species num");
                return 4;
        }
    }
}


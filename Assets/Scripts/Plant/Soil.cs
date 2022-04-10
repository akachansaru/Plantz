using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Soil
{
    [SerializeField] private Biomes biomeType = Biomes.Forest; // Each biome will have a soil type
    [SerializeField] private float hoursToDryOut = 0f; // In ideal climate (biomeType). Will dry out faster if in more dry and hot
    [SerializeField] private float fullMoistureLevel = 0f; // The amount of water the soil holds when fully watered. 0 to 100. The plant will rot if too wet
    [SerializeField] private float currentMoistureLevel = 0f;

    public Biomes BiomeType { get { return biomeType; } private set { biomeType = value; } }
    public float HoursToDryOut { get { return hoursToDryOut; } private set { hoursToDryOut = value; } }
    public float FullMoistureLevel { get { return fullMoistureLevel; } private set { fullMoistureLevel = value; } }
    public float CurrentMoistureLevel { get { return currentMoistureLevel; } private set { currentMoistureLevel = value; } }

    public Soil(Biomes biomeType)
    {
        BiomeType = biomeType;
        HoursToDryOut = GetHoursToDryOut(biomeType);
        FullMoistureLevel = GetFullMoistureLevel(biomeType);
        CurrentMoistureLevel = FullMoistureLevel; // Start off as saturated as possible
    }

    // For watering or drying out over time
    public void AdjustMoistureLevel(float amount)
    {
        if (CurrentMoistureLevel + amount < FullMoistureLevel && CurrentMoistureLevel + amount > 0f)
        {
            CurrentMoistureLevel += amount;
        }
        else if (CurrentMoistureLevel + amount > FullMoistureLevel)
        {
            CurrentMoistureLevel = FullMoistureLevel;
        }
        else if (CurrentMoistureLevel + amount < 0f)
        {
            CurrentMoistureLevel = 0f;
        }
    }

    public void Water()
    {
        AdjustMoistureLevel(FullMoistureLevel);
        // UNDONE: add time into this - adjusting moisture level little bits over time
    }

    public static float GetHoursToDryOut(Biomes biomeType)
    {
        switch (biomeType)
        {
            case Biomes.Forest:
                return GameTime.DAY;
            case Biomes.Desert:
                return GameTime.DAY / 2;
            case Biomes.Swamp:
                return GameTime.DAY * 3;
            default:
                Debug.LogError("Biome not found.");
                return GameTime.DAY;
        }
    }

    public static float GetFullMoistureLevel(Biomes biomeType)
    {
        switch (biomeType)
        {
            case Biomes.Forest:
                return 50f;
            case Biomes.Desert:
                return 30f;
            case Biomes.Swamp:
                return 80f;
            default:
                Debug.LogError("Biome not found.");
                return 50f;
        }
    } 
}

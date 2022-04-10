using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.Utilities;


public enum Biomes { Forest, Desert, Swamp }

public class Biome : MonoBehaviour
{
    [SerializeField] private Biomes biomeType = Biomes.Forest;

    [SerializeField] private RequirementRange temperatureRange;
    //[SerializeField] private float currentTemperature = 60f;
    [SerializeField] private RequirementRange humidityRange;
    //[SerializeField] private float currentHumidity = 40f;

    public Biomes BiomeType { get { return biomeType; } }

    private void Start()
    {
        temperatureRange = GetTemperatureRange(BiomeType);
        humidityRange = GetHumidityRange(BiomeType);
    }

    private void Update()
    {
        AjustTempToTimeOfDay();
    }

    public static RequirementRange GetTemperatureRange(Biomes biomeType)
    {
        RequirementRange temperatureRange;
        switch (biomeType)
        {
            case Biomes.Forest:
                temperatureRange = new RequirementRange(35f, 65f, 0f);
                break;
            case Biomes.Desert:
                temperatureRange = new RequirementRange(10f, 90f, 0f);
                break;
            case Biomes.Swamp:
                temperatureRange = new RequirementRange(40f, 70f, 0f);
                break;
            default:
                temperatureRange = new RequirementRange(35f, 65f, 0f);
                Debug.LogWarning("biome not in list");
                break;
        }
        return temperatureRange;
    }

    public static RequirementRange GetHumidityRange(Biomes biomeType)
    {
        RequirementRange humidityRange;
        switch (biomeType)
        {
            case Biomes.Forest:
                humidityRange = new RequirementRange(20f, 60f, 0f);
                break;
            case Biomes.Desert:
                humidityRange = new RequirementRange(0f, 10f, 0f);
                break;
            case Biomes.Swamp:
                humidityRange = new RequirementRange(60f, 90f, 0f);
                break;
            default:
                humidityRange = new RequirementRange(20f, 60f, 0f);
                Debug.LogWarning("biome not in list");
                break;
        }
        return humidityRange;
    }

    private void AjustTempToTimeOfDay()
    {

    }
}

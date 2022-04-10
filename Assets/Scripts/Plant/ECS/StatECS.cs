using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StatECS
{
    private const int cultivarProbability = 95; // Probability of generating a cultivar when combining stats if both have ultraRare rarity
    private const float cultivarFactor = 5f;    // Multiply (for getting larger) or divide (smaller) when creating a cultivar stat

    // Don't make these properties or private. They won't show up in the Entity inspector
    public Rarity rarity;
    public float value;
    public float mean;
    public float max;
    public bool isCultivarStat;
    //public string ID { get; set; }

    public StatECS(float mean, float max) //, string ID)
    {
        this.mean = mean;
        this.max = max;
        value = 0f; // Dummy so I can use CalculateValue
        rarity = Rarity.Common; // Dummy so I can use GetRarity
        isCultivarStat = false;
        Dice dice = new Dice(6, 2, 3, 9);
        int diceRoll = dice.RollDice();
        value = CalculateValue(dice, diceRoll, mean, max);
        rarity = GetRarity6d2(diceRoll);
        //this.ID = ID;
    }

    public StatECS(bool isNull)
    {
        mean = 0;
        max = 0;
        value = 0;
        isCultivarStat = false;
        rarity = Rarity.Common;
    }

    // I think this was to test creating cultivars
    //private StatECS(float mean, float max, Rarity minRarity)//, string ID)
    //{
    //    this.mean = mean;
    //    this.max = max;
    //    value = 0f; // Dummy so I can use CalculateValue
    //    rarity = Rarity.Common; // Dummy so I can use CalculateValue
    //    isCultivarStat = false;

    //    value = CalculateValue(new Dice(6, 2, 3, 9), mean, max, minRarity, out rarity);
    //    //this.ID = ID;
    //}

    private StatECS(float value, float mean, float max, Rarity rarity)//, string ID)
    {
        this.value = value;
        this.mean = mean;
        this.max = max;
        this.rarity = rarity;
        isCultivarStat = false;
        //this.ID = ID;
    }

    public StatECS CombineSpecies(StatECS other, List<StatECS> cultivarStats)
    {
        // mean and max will be the same for the same species
        //if (value == other.value && mean == other.mean && max == other.max)
        //{
        //    // For self pollinating. Just do new stat based on value, mean, max so a cultivar can't be created easily by self pollinating
        //    Debug.Log("self pollinated");
        //    return new Stat(CalculateValue(mean, max, out Rarity newRarity), mean, max, newRarity);
        //}
        //else
        //{

        Dice dice = new Dice(6, 2, 3, 9);
        int diceRoll = dice.RollDice();
        if (value > mean && other.value >= mean || value >= mean && other.value > mean) // resulting value should be above average
        {
            if (rarity == Rarity.UltraRare && other.rarity == Rarity.UltraRare && UnityEngine.Random.Range(0, 100) >= 100 - cultivarProbability)
            {
                return CreateCultivarStat(mean * cultivarFactor, max * cultivarFactor, cultivarStats);//, ID + "A");
            }
            else if (rarity == other.rarity) // ensure the new stat is at least Rarity
            {
                return new StatECS(CalculateValueAboveAve(dice, mean, max, rarity, out Rarity newRarity), mean, max, newRarity); //, ID + "A");
            }
            else
            {
                return new StatECS(CalculateValueAboveAve(dice, mean, max, Rarity.Common, out Rarity newRarity), mean, max, newRarity); //, ID + "A");
            }
        }
        else if (value < mean && other.value <= mean || value <= mean && other.value < mean) // resulting value should be below average
        {
            if (rarity == Rarity.UltraRare && other.rarity == Rarity.UltraRare && UnityEngine.Random.Range(0, 100) >= 100 - cultivarProbability)
            {
                return CreateCultivarStat(mean / cultivarFactor, max / cultivarFactor, cultivarStats); //, ID + "B");
            }
            else if (rarity == other.rarity) // ensure the new stat is at least Rarity
            {
                return new StatECS(CalculateValueBelowAve(dice, mean, max, rarity, out Rarity newRarity), mean, max, newRarity); //, ID + "B");
            }
            else
            {
                return new StatECS(CalculateValueBelowAve(dice, mean, max, Rarity.Common, out Rarity newRarity), mean, max, newRarity); //, ID + "B");
            }
        }
        else // If they're not both above or both below just calculate a new value
        {
            Rarity newRarity = GetRarity6d2(diceRoll);
            return new StatECS(CalculateValue(dice, diceRoll, mean, max), mean, max, newRarity); //, ID);
        }
        //}
    }

    private StatECS CreateCultivarStat(float mean, float max, List<StatECS> cultivarStats)//, string ID)
    {
        Dice dice = new Dice(6, 2, 3, 9);
        int diceRoll = dice.RollDice();
        Rarity newRarity = GetRarity6d2(diceRoll);
        StatECS cultivarStat = new StatECS(CalculateValue(dice, diceRoll, mean, max), mean, max, newRarity)
        {
            isCultivarStat = true
        };
        cultivarStats.Add(cultivarStat);
        return cultivarStat;
    }

    /// <summary>
    /// Creates a new stat using the average of the mean and the max
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public StatECS CombineHybrid(StatECS other)
    {
        Debug.Log("hybrid combine");
        float meanAve = (mean + other.mean) / 2f;
        float maxAve = (max + other.max) / 2f;
        return new StatECS(meanAve + UnityEngine.Random.Range(meanAve - maxAve, maxAve - meanAve), meanAve, maxAve, Rarity.Common); // TODO: ID not necessarily right. change the rarity to something meaningful
    }

    /// <summary>
    /// For stats like trunk rotation that need to be different each time it's applied
    /// </summary>
    /// <returns></returns>
    public float GetValueWithVariance()
    {
        Dice dice = new Dice(2, 12, 12, 12); // 2d12 to give it a nice, smooth probability spread
        return CalculateValue(dice, dice.RollDice(), mean, max);
    }

    private float CalculateValue(Dice dice, int diceRoll, float mean, float max)
    {
        float amtFromMean = max - mean;
        float stepSize = amtFromMean / dice.MaxStepsFromMean;
        int amtFromMeanDice = diceRoll - dice.CurveMean;
        return mean + amtFromMeanDice * stepSize;
    }

    // I think this was to test creating cultivars
    //private float CalculateValue(Dice dice, float mean, float max, Rarity minRarity, out Rarity newStatRarity)
    //{
    //    float amtFromMean = max - mean;
    //    float stepSize = amtFromMean / dice.MaxStepsFromMean;
    //    int diceRoll = GetMinRarity(dice, minRarity, out newStatRarity);

    //    int amtFromMeanDice = diceRoll - dice.CurveMean;
    //    return mean + amtFromMeanDice * stepSize;
    //}

    public float CalculateValueAboveAve(Dice dice, float mean, float max, Rarity minRarity, out Rarity newStatRarity)
    {
        Debug.Log("calculate above ave");
        float amtFromMean = max - mean;
        float stepSize = amtFromMean / dice.MaxStepsFromMean;
        int diceRoll = GetMinRarity(dice, minRarity, out newStatRarity);
        int amtFromMeanDice = Math.Abs(diceRoll - dice.CurveMean);
        return mean + amtFromMeanDice * stepSize;
    }

    private int GetMinRarity(Dice dice, Rarity minRarity, out Rarity newStatRarity)
    {
        int diceRoll = dice.RollDice();
        newStatRarity = GetRarity6d2(diceRoll);
        // Keep rolling until the desired minimum rarity is met
        while (newStatRarity < minRarity)
        {
            diceRoll = dice.RollDice();
            newStatRarity = GetRarity6d2(diceRoll);
        }
        return diceRoll;
    }

    public float CalculateValueBelowAve(Dice dice, float mean, float max, Rarity minRarity, out Rarity newStatRarity)
    {
        float amtFromMean = max - mean;
        float stepSize = amtFromMean / dice.MaxStepsFromMean;
        int diceRoll = GetMinRarity(dice, minRarity, out newStatRarity);

        int amtFromMeanDice = -Math.Abs(diceRoll - dice.CurveMean);
        return mean + amtFromMeanDice * stepSize;
    }

    private Rarity GetRarity6d2(int diceRoll)
    {
        switch (diceRoll)
        {
            case 6:
                return Rarity.UltraRare;
            case 7:
                return Rarity.Rare;
            case 8:
                return Rarity.Uncommon;
            case 9:
                return Rarity.Common;
            case 10:
                return Rarity.Uncommon;
            case 11:
                return Rarity.Rare;
            case 12:
                return Rarity.UltraRare;
            default:
                Debug.LogError("Dice roll did not return an expected value. Stat rarity may be wrong.");
                return Rarity.Common;
        }
    }
}

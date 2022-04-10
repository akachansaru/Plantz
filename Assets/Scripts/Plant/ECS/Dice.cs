using System;
using UnityEngine;

public class Dice
{
    public int NumRolls { get; set; } // = 6;
    public int XSidedDie { get; set; } // = 2;
    public int MaxStepsFromMean { get; set; } // = 3; // 3 is for using a bell curve based on 6d2 - mean is 9, min is 6, max is 12
    public int CurveMean { get; set; } // = 9; // 9 is for 6d2
                                        // common: 9; uncommon: 8, 10; rare: 7, 11; ultra rare: 6, 12
                                        // https://www.redblobgames.com/articles/probability/damage-rolls.html -> not correct. Starts at 0
                                        // https://anydice.com/

    public Dice(int numRolls, int xSidedDie, int maxStepsFromMean, int curveMean)
    {
        NumRolls = numRolls;
        XSidedDie = xSidedDie;
        MaxStepsFromMean = maxStepsFromMean;
        CurveMean = curveMean;
    }

    /// <summary>
    /// Example: numRolls = 6, xSidedDie = 2 will roll a 2 sided die 6 times (6d2)
    /// </summary>
    /// <param name="numRolls"></param>
    /// <param name="xSidedDie"></param>
    /// <returns></returns>
    public int RollDice()
    {
        int value = 0;

        for (int i = 0; i < NumRolls; i++)
        {
            value += 1 + UnityEngine.Random.Range(0, XSidedDie);
        }
        return value;
    }

    //public float CalculateValue(float mean, float max, out Rarity newStatRarity)
    //{
    //    float amtFromMean = max - mean;
    //    float stepSize = amtFromMean / maxStepsFromMean;
    //    int diceRoll = RollDice();
    //    int amtFromMeanDice = diceRoll - curveMean;
    //    newStatRarity = GetRarity(diceRoll);
    //    return mean + amtFromMeanDice * stepSize;
    //}

    //public float CalculateValue(float mean, float max)
    //{
    //    float amtFromMean = max - mean;
    //    float stepSize = amtFromMean / maxStepsFromMean;
    //    int diceRoll = RollDice();
    //    int amtFromMeanDice = diceRoll - curveMean;
    //    return mean + amtFromMeanDice * stepSize;
    //}

    //public float CalculateValue(float mean, float max, Rarity minRarity, out Rarity newStatRarity)
    //{
    //    float amtFromMean = max - mean;
    //    float stepSize = amtFromMean / maxStepsFromMean;
    //    int diceRoll = GetMinRarity(minRarity, out newStatRarity);

    //    int amtFromMeanDice = diceRoll - curveMean;
    //    return mean + amtFromMeanDice * stepSize;
    //}

    //public float CalculateValueAboveAve(float mean, float max, Rarity minRarity, out Rarity newStatRarity)
    //{
    //    Debug.Log("calculate above ave");
    //    float amtFromMean = max - mean;
    //    float stepSize = amtFromMean / maxStepsFromMean;
    //    int diceRoll = GetMinRarity(minRarity, out newStatRarity);

    //    int amtFromMeanDice = Math.Abs(diceRoll - curveMean);
    //    return mean + amtFromMeanDice * stepSize;
    //}

    //private int GetMinRarity(Rarity minRarity, out Rarity newStatRarity)
    //{
    //    int diceRoll = RollDice();
    //    newStatRarity = GetRarity(diceRoll);
    //    // Keep rolling until the desired minimum rarity is met
    //    while (newStatRarity < minRarity)
    //    {
    //        diceRoll = RollDice();
    //        newStatRarity = GetRarity(diceRoll);
    //    }
    //    return diceRoll;
    //}

    //public float CalculateValueBelowAve(float mean, float max, Rarity minRarity, out Rarity newStatRarity)
    //{
    //    float amtFromMean = max - mean;
    //    float stepSize = amtFromMean / maxStepsFromMean;
    //    int diceRoll = GetMinRarity(minRarity, out newStatRarity);

    //    int amtFromMeanDice = -Math.Abs(diceRoll - curveMean);
    //    return mean + amtFromMeanDice * stepSize;
    //}

    //private Rarity GetRarity(int diceRoll)
    //{
    //    switch (diceRoll)
    //    {
    //        case 6:
    //            return Rarity.UltraRare;
    //        case 7:
    //            return Rarity.Rare;
    //        case 8:
    //            return Rarity.Uncommon;
    //        case 9:
    //            return Rarity.Common;
    //        case 10:
    //            return Rarity.Uncommon;
    //        case 11:
    //            return Rarity.Rare;
    //        case 12:
    //            return Rarity.UltraRare;
    //        default:
    //            Debug.LogError("Dice roll did not return an expected value. Stat rarity may be wrong.");
    //            return Rarity.Common;
    //    }
    //}
}

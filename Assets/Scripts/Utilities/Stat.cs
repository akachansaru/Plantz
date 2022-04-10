using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    [Serializable]
    public class Stat
    {
        [SerializeField] private int cultivarProbability = 95; // Probability of generating a cultivar when combining stats if both have ultraRare rarity
        [SerializeField] private float cultivarFactor = 5f; // Multiply (for getting larger) or divide (smaller) when creating a cultivar stat
        [Space]
        [SerializeField] private float value = 0f;
        [SerializeField] private float mean = 0f;
        [SerializeField] private float max = 0f;
        [SerializeField] private Rarity rarity = Rarity.Common;
        [SerializeField] private bool isCultivarStat = false;
        [SerializeField] private string iD = "";

        public float Value { get { return value; } set { this.value = value; } }
        public float Mean { get { return mean; } set { mean = value; } }
        public float Max { get { return max; } set { max = value; } }
        public Rarity Rarity { get { return rarity; } private set { rarity = value; } }
        public bool IsCultivarStat { get { return isCultivarStat; } private set { isCultivarStat = value; } }
        public string ID { get { return iD; } set { iD = value; } }

        public Stat(float mean, float max, string ID)
        {
            Value = CalculateValue(mean, max, out rarity);
            Mean = mean;
            Max = max;
            this.ID = ID;
        }

        public Stat(float mean, float max, Rarity minRarity, string ID)
        {
            Value = CalculateValue(mean, max, minRarity, out rarity);
            Mean = mean;
            Max = max;
            this.ID = ID;
        }

        private Stat(float value, float mean, float max, Rarity rarity, string ID)
        {
            Value = value;
            Mean = mean;
            Max = max;
            Rarity = rarity;
            this.ID = ID;
        }

        public Stat CombineSpecies(Stat other, List<Stat> cultivarStats)
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
            if (value > mean && other.value >= mean || value >= mean && other.value > mean) // resulting value should be above average
            {
                if (Rarity == Rarity.UltraRare && other.Rarity == Rarity.UltraRare && UnityEngine.Random.Range(0, 100) >= 100 - cultivarProbability)
                {
                    return CreateCultivarStat(mean * cultivarFactor, max * cultivarFactor, cultivarStats, ID + "A");
                }
                else if (Rarity == other.Rarity) // ensure the new stat is at least Rarity
                {
                    return new Stat(CalculateValueAboveAve(mean, max, Rarity, out Rarity newRarity), mean, max, newRarity, ID + "A");
                }
                else
                {
                    return new Stat(CalculateValueAboveAve(mean, max, Rarity.Common, out Rarity newRarity), mean, max, newRarity, ID + "A");
                }
            }
            else if (value < mean && other.value <= mean || value <= mean && other.value < mean) // resulting value should be below average
            {
                if (Rarity == Rarity.UltraRare && other.Rarity == Rarity.UltraRare && UnityEngine.Random.Range(0, 100) >= 100 - cultivarProbability)
                {
                    return CreateCultivarStat(mean / cultivarFactor, max / cultivarFactor, cultivarStats, ID + "B");
                }
                else if (Rarity == other.Rarity) // ensure the new stat is at least Rarity
                {
                    return new Stat(CalculateValueBelowAve(mean, max, Rarity, out Rarity newRarity), mean, max, newRarity, ID + "B");
                }
                else
                {
                    return new Stat(CalculateValueBelowAve(mean, max, Rarity.Common, out Rarity newRarity), mean, max, newRarity, ID + "B");
                }
            }
            else // If they're not both above or both below just calculate a new value
            {
                return new Stat(CalculateValue(mean, max, out Rarity newRarity), mean, max, newRarity, ID);
            }
            //}
        }

        private Stat CreateCultivarStat(float mean, float max, List<Stat> cultivarStats, string ID)
        {
            Stat cultivarStat = new Stat(CalculateValue(mean, max, out Rarity newRarity), mean, max, newRarity, ID);
            cultivarStat.IsCultivarStat = true;
            cultivarStats.Add(cultivarStat);
            return cultivarStat;
        }

        /// <summary>
        /// Creates a new stat using the average of the mean and the max
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Stat CombineHybrid(Stat other)
        {
            Debug.Log("hybrid combine");
            float meanAve = (mean + other.mean) / 2f;
            float maxAve = (max + other.max) / 2f;
            return new Stat(meanAve + UnityEngine.Random.Range(meanAve - maxAve, maxAve - meanAve), meanAve, maxAve, Rarity.Common, ID); // TODO: ID not necessarily right.change the rarity to something meaningful
        }

        public float GetValueWithVariance()
        {
            return CalculateValue(Mean, Max, 2, 12, 12, 12); // 2d12
        }

        private int numRolls = 6;
        private int xSidedDie = 2;
        private int maxStepsFromMean = 3; // 3 is for using a bell curve based on 6d2 - mean is 9, min is 6, max is 12
        private int curveMean = 9; // 9 is for 6d2
        // common: 9; uncommon: 8, 10; rare: 7, 11; ultra rare: 6, 12
        // https://www.redblobgames.com/articles/probability/damage-rolls.html

        private float CalculateValue(float mean, float max, out Rarity newStatRarity)
        {
            float amtFromMean = max - mean;
            float stepSize = amtFromMean / maxStepsFromMean;
            int diceRoll = RollDice(numRolls, xSidedDie);
            int amtFromMeanDice = diceRoll - curveMean;
            newStatRarity = GetRarity(diceRoll);
            return mean + amtFromMeanDice * stepSize;
        }

        private float CalculateValue(float mean, float max, int numRolls, int xSidedDie, int maxStepsFromMean, int curveMean)
        {
            float amtFromMean = max - mean;
            float stepSize = amtFromMean / maxStepsFromMean;
            int diceRoll = RollDice(numRolls, xSidedDie);
            int amtFromMeanDice = diceRoll - curveMean;
            return mean + amtFromMeanDice * stepSize;
        }

        private float CalculateValue(float mean, float max, Rarity minRarity, out Rarity newStatRarity)
        {
            float amtFromMean = max - mean;
            float stepSize = amtFromMean / maxStepsFromMean;
            int diceRoll = GetMinRarity(minRarity, out newStatRarity);

            int amtFromMeanDice = diceRoll - curveMean;
            return mean + amtFromMeanDice * stepSize;
        }

        private float CalculateValueAboveAve(float mean, float max, Rarity minRarity, out Rarity newStatRarity)
        {
            Debug.Log("calculate above ave");
            float amtFromMean = max - mean;
            float stepSize = amtFromMean / maxStepsFromMean;
            int diceRoll = GetMinRarity(minRarity, out newStatRarity);

            int amtFromMeanDice = Math.Abs(diceRoll - curveMean);
            return mean + amtFromMeanDice * stepSize;
        }

        private int GetMinRarity(Rarity minRarity, out Rarity newStatRarity)
        {
            int diceRoll = RollDice(numRolls, xSidedDie);
            newStatRarity = GetRarity(diceRoll);
            // Keep rolling until the desired minimum rarity is met
            while (newStatRarity < minRarity)
            {
                diceRoll = RollDice(numRolls, xSidedDie);
                newStatRarity = GetRarity(diceRoll);
            }
            return diceRoll;
        }

        private float CalculateValueBelowAve(float mean, float max, Rarity minRarity, out Rarity newStatRarity)
        {
            float amtFromMean = max - mean;
            float stepSize = amtFromMean / maxStepsFromMean;
            int diceRoll = GetMinRarity(minRarity, out newStatRarity);

            int amtFromMeanDice = -Math.Abs(diceRoll - curveMean);
            return mean + amtFromMeanDice * stepSize;
        }

        /// <summary>
        /// Example: numRolls = 6, xSidedDie = 2 will roll a 2 sided die 6 times (6d2)
        /// </summary>
        /// <param name="numRolls"></param>
        /// <param name="xSidedDie"></param>
        /// <returns></returns>
        private static int RollDice(int numRolls, int xSidedDie)
        {
            int value = 0;
            for (int i = 0; i < numRolls; i++)
            {
                value += 1 + UnityEngine.Random.Range(0, xSidedDie);
            }
            return value;
        }

        private Rarity GetRarity(int diceRoll)
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
}
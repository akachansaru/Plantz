using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private Text moneyText;

    public Text MoneyText { get { return moneyText; } set { moneyText = value; } }

    void Awake()
    {
        Assert.IsNotNull(moneyText);
    }

    void Update()
    {
        moneyText.text = ")( " + GlobalControl.Instance.savedValues.Inventory.Money;
    }

    public static float GetPriceOfPlant(PlantFE plantFE)
    {
        Taxonomy taxonomy = plantFE.Plant.Taxonomy;
        int rarityFactor =
            Taxonomy.SpeciesMultiplier * (int)taxonomy.Species.Rarity +
            Taxonomy.GenusMultiplier * (int)taxonomy.Species.Genus.Rarity +
            Taxonomy.FamilyMultiplier * (int)taxonomy.Species.Genus.Family.Rarity;

        decimal sizePortion = plantFE.GetPercentMaxSize() * rarityFactor;
        decimal caliperPortion = plantFE.GetPercentMaxCaliper() * rarityFactor;
        float healthFactor = plantFE.Plant.Health / 100f; // over 100 to normalize it to less than 1

        return (float)Math.Round((float)(sizePortion + caliperPortion) * healthFactor, 2);
    }
}
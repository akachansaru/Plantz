using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Fruit : IPlantComponent
{
    [SerializeField] private Taxonomy pollinatedTaxonomy;

    public Taxonomy PollinatedTaxonomy { get { return pollinatedTaxonomy; } private set { pollinatedTaxonomy = value; } }

    /// <summary>
    /// Creates a fruit with the parameters from the Flower
    /// </summary>
    /// <param name="flower"></param>
    public Fruit(Flower flower, Taxonomy combinedTaxonomy)
    {
        PollinatedTaxonomy = combinedTaxonomy;
        Parent = flower.Parent;
        CreateID(flower.GetID());
    }

    public override void Mature()
    {
        base.Mature(); // When IsMature == true it will have seeds

    }

    private string CreateID(string flowerName)
    {
        compID = "Fr" + flowerName.Remove(0, 1);
        return compID;
    }
}

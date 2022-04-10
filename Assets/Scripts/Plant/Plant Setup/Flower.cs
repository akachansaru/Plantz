using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Flower : IPlantComponent
{
    [SerializeField] private bool hasPollen = false;
    [SerializeField] private bool isPollinated = false;
    [SerializeField] private Taxonomy pollinatedTaxonomy;

    public bool HasPollen { get { return hasPollen; } set { hasPollen = value; } }
    public bool IsPollinated { get { return isPollinated; } private set { isPollinated = value; } } // Plant also has this. Should choose how the plant will be pollinated and get rid of one
    public Taxonomy PollinatedTaxonomy { get { return pollinatedTaxonomy; } private set { pollinatedTaxonomy = value; } }

    public Flower(string stem, int flowerNum)
    {
        CreateID(stem, flowerNum, "F");
    }

    public override void Mature()
    {
        base.Mature();
        HasPollen = true;
        // UNDONE: Add a new model in with the fully bloomed flower
    }

    public void Pollinate(Taxonomy combinedTaxonomy)
    {
        PollinatedTaxonomy = combinedTaxonomy;
        IsPollinated = true;
    }

    // This is if I just want to generate the back end. FlowerFE has a version that does front and back end
    public Fruit StartFruiting(Plant plant, Stem stem)
    {
        // add the fruit GO and start it growing
        Fruit newFruit = new Fruit(this, PollinatedTaxonomy);
        newFruit.LocalScale = plant.Taxonomy.Species.Genus.FruitInitialSize;
        newFruit.LocalPosition = newFruit.GetPositionForScaleChange(LocalScale, newFruit.LocalScale, LocalPosition);
        newFruit.LocalRotation = LocalRotation;

        plant.Fruit.Add(newFruit);
        stem.Fruit.Add(newFruit);
        return newFruit;
    }
}

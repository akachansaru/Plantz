using UnityEngine;

[RequireComponent(typeof(PlantFE))]
public class InitializeIndividualPlant : MonoBehaviour
{
    public enum SpeciesList { S1, S2, S3, S4, S5, SSmall, SFastGrowing }

    public SpeciesList species;

    public Taxonomy SetSpecies()
    {
        return GlobalControl.Instance.savedValues.AllSpecies[(int) species];
    }
}

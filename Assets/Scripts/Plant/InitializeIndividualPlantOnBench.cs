using UnityEngine;

/// <summary>
/// This should only go on placeholder plants on the workbenches. For "seeds" in the worktable scene InitializeIndividualPlant should be used
/// </summary>
[RequireComponent(typeof(PlantFE))]
public class InitializeIndividualPlantOnBench : InitializeIndividualPlant
{
    public int numDays;
    [Range(0, 1)] public float leafGrownPercent;
    [Range(0, 1)] public float flowerGrownPercent;

    public void Start()
    {
        Transform benchSegmentT = transform.parent;

        if (!benchSegmentT.GetComponent<BenchSegment>().IsOccupied)
        {
            Pot pot = GetComponent<PlantFE>().Plant.Pot;
            Plant newPlant = new Plant(SetSpecies(), pot, ConstantValues.SaveLists.Greenhouse);
            Vector3 posAboveBench = new Vector3(0, (benchSegmentT.localScale.y + transform.localScale.y) / 2, 0);
            newPlant.Pot.FillWithSoil(new Soil(Biomes.Forest));

            GameObject newPlantGO = PlantUtilities.LoadPlant(newPlant.InstaGrow(numDays, leafGrownPercent, flowerGrownPercent), 
                Resources.Load(ConstantValues.Prefabs.Plant) as GameObject, benchSegmentT, posAboveBench);
            PotFrontEnd.ApplyColor(newPlantGO, pot);

            benchSegmentT.GetComponent<BenchSegment>().SetPlantOnBench(newPlantGO);

        }

        Destroy(gameObject);
    }
}

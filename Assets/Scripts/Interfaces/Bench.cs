using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For placing plants in greenhouse grid.
/// </summary>
public class Bench : MonoBehaviour
{
    [SerializeField] private int benchNum = 0;
    [SerializeField] private List<GameObject> benchSegments = new List<GameObject>();
    public List<Plant[]> saveList;

    public int BenchNum { get { return benchNum; } private set { benchNum = value; } }
    public List<GameObject> BenchSegments { get { return benchSegments; } private set { benchSegments = value; } }

    public virtual void Start()
    {
        LoadPlantsOnBenchSegments();
    }

    private void LoadPlantsOnBenchSegments()
    {
        int numPlants = BenchSegments.Count;
        int i = 0;
        if (saveList.Count > 0)
        {
            foreach (Plant plant in saveList[BenchNum])
            {
                if (i >= numPlants)
                {
                    Debug.LogWarning("Too many plants for bench #" + BenchNum);
                }
                else
                {
                    LoadPlantOnBenchSegment(plant, i);
                    i++;
                }
            }
        }
    }

    private void LoadPlantOnBenchSegment(Plant plant, int benchSegment)
    {
        if (plant != null)
        {
            GameObject plantGO = PlantUtilities.LoadPlant(plant, Resources.Load(ConstantValues.Prefabs.Plant) as GameObject);
            BenchSegments[benchSegment].GetComponent<BenchSegment>().SetPlantOnBench(plantGO);
        }
    }
}

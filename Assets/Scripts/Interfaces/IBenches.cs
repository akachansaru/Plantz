using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class IBenches : MonoBehaviour
{
    [SerializeField] protected List<GameObject> benches = new List<GameObject>();
    protected List<Plant[]> saveList;

    public List<GameObject> Benches { get { return benches; } private set { benches = value;}}

    public virtual void Awake()
    {
        Assert.AreNotEqual(benches.Count, 0, "Need to assign benches");
    }

    public virtual void Start()
    {
        AddBenchesToList();
    }

    private void AddBenchesToList()
    {
        // If there's no saved game add all of the benches
        if (saveList.Count == 0)
        {
            foreach (GameObject bench in benches)
            {
                saveList.Add(new Plant[bench.GetComponent<Bench>().BenchSegments.Count]);
            }
        }
        else if (saveList.Count < benches.Count)
        { // Add one new one if there is a new bench
            // UNDONE: this hasn't been tested at all. Need to add initial count to list
            Debug.LogWarning("Don't think it should run here");
            saveList.Add(new Plant[benches[benches.Count - 1].GetComponent<Bench>().BenchSegments.Count]);
        }
    }
}
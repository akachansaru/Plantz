using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For placing plants in greenhouse grid.
/// </summary>
public class StoreBench : Bench
{
    public override void Start()
    {
        saveList = GlobalControl.Instance.savedValues.GreenhousePlants;
        base.Start();
    }
}

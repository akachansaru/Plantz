using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreBenches : IBenches
{
    public override void Start()
    {
        saveList = GlobalControl.Instance.savedValues.GreenhousePlants;
        base.Start();
    }
}
using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Leaf : IPlantComponent
{
    //[SerializeField] private Vector3Serializable originalPosition; // For regrowing the leaves

    //public Vector3Serializable OriginalPosition { get { return originalPosition; } set { originalPosition = value; } }

    public Leaf(string stem, int leafNum)
    {
        CreateID(stem, leafNum, "L");
    }

    public override void Mature()
    {
        base.Mature();
    }
}

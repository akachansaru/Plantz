using System;
using UnityEngine;

[Serializable]
public class Pot
{
    [SerializeField] private Vector3Serializable potSize = new Vector3Serializable(0.5f, 0.5f, 0.5f);
    [SerializeField] private Vector4Serializable potColor = new Vector4Serializable(195, 127, 95, 1);
    [SerializeField] private Soil soil;

    public Vector3Serializable PotSize { get { return potSize; } private set { potSize = value; } }
    public Vector4Serializable PotColor { get { return potColor; } set { potColor = value; } }
    public Soil Soil { get { return soil; } private set { soil = value; } }

    public Pot() { }

    public Pot(Vector3Serializable potSize, Vector4Serializable potColor)
    {
        this.potSize = potSize;
        this.potColor = potColor;
    }

    public void FillWithSoil(Soil soil)
    {
        Soil = soil;
    }
}

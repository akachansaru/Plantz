using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilFrontEnd : MonoBehaviour
{
    [SerializeField] private Soil soil = null;

    public Soil Soil { get { return soil; } set { soil = value; } }

    public void Start()
    {
        switch (soil.BiomeType)
        {
            case Biomes.Forest:
                GetComponent<MeshRenderer>().material = Resources.Load(ConstantValues.Materials.Grass) as Material;
                break;
            case Biomes.Desert:
                GetComponent<MeshRenderer>().material = Resources.Load(ConstantValues.Materials.Sand) as Material;
                break;
            case Biomes.Swamp:
                GetComponent<MeshRenderer>().material = Resources.Load(ConstantValues.Materials.Swamp) as Material;
                break;
            default:
                Debug.LogError("Biome type not found. Can't set material of soil.");
                break;
        }
    }

    public void OnMouseDown()
    {
        PlantingManager.Instance.SwitchToPottingCanvas(Soil);
    }
}

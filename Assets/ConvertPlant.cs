using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertPlant : MonoBehaviour
{
    private void Start()
    {
        string getRidOf = gameObject.name;
        string replaceWith = "SavedPlant:" + GetComponent<PlantFE>().Plant.Taxonomy.Species.SpeciesName;
        name = ReplaceName(transform, getRidOf, replaceWith);
        Transform root = transform;
        //FlattenHierarchy(transform, root);
    }

    private Transform FlattenHierarchy(Transform plantObject, Transform root)
    {
        foreach (Transform child in plantObject.transform)
        {
            child.parent = FlattenHierarchy(child, root);
        }
        return root;

    }

    private string ReplaceName(Transform plantObject, string getRidOf, string replaceWith)
    {
        foreach (Transform child in plantObject.transform)
        {
            child.name = ReplaceName(child, getRidOf, replaceWith);
        }
        return plantObject.name.Replace(getRidOf, replaceWith);
    }
}

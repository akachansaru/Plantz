using System;
using UnityEngine;

[Serializable]
public class PollenECS
{
    [SerializeField] private bool isLoaded = false;
    [SerializeField] private TaxonomyECS taxonomy = null;

    public bool IsLoaded { get { return isLoaded; } set { isLoaded = value; } } // Is it loaded in the inventory canvas?
    public TaxonomyECS Taxonomy { get { return taxonomy; } set { taxonomy = value; } }

    public PollenECS(TaxonomyECS taxonomy)
    {
        Taxonomy = taxonomy;
    }

}
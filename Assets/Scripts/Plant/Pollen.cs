using System;
using UnityEngine;

[Serializable]
public class Pollen
{
    [SerializeField] private bool isLoaded = false;
    [SerializeField] private Taxonomy taxonomy = null;

    public bool IsLoaded { get { return isLoaded; } set { isLoaded = value; } } // Is it loaded in the inventory canvas?
    public Taxonomy Taxonomy { get { return taxonomy; } set { taxonomy = value; } }

    public Pollen(Taxonomy taxonomy)
    {
        Taxonomy = taxonomy;
    }

}
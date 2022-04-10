using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    [SerializeField] private List<Pollen> inventoryPollen;
    [SerializeField] private List<PollenECS> inventoryPollenECS;
    [SerializeField] private List<Pollen> inventorySeeds;
    [SerializeField] private float money;

    public List<Pollen> InventoryPollen { get { return inventoryPollen; } private set { inventoryPollen = value; } }
    public List<PollenECS> InventoryPollenECS { get { return inventoryPollenECS; } private set { inventoryPollenECS = value; } }
    public List<Pollen> InventorySeeds { get { return inventorySeeds; } private set { inventorySeeds = value; } }
    public float Money { get { return money; } set { money = value; } }

    public Inventory(float startingMoney, List<Pollen> startingSeeds)
    {
        inventoryPollen = new List<Pollen>();
        inventoryPollenECS = new List<PollenECS>();
        inventorySeeds = startingSeeds;
        money = startingMoney;
    }
}
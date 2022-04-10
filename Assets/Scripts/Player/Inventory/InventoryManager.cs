using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject pollenPanel = null;
    [SerializeField] private GameObject seedPanel = null;
    [SerializeField] private Canvas inventoryCanvas = null;
    [SerializeField] private Text speciesText = null;

    [SerializeField] private Inventory inventory;
    //[SerializeField] private Inventory inventoryECS;

    public Inventory Inventory { get { return inventory; } }
    //public Inventory InventoryECS { get { return inventoryECS; } }

    public void Awake()
    {
        Assert.IsNotNull(pollenPanel);
        Assert.IsNotNull(seedPanel);
        Assert.IsNotNull(inventoryCanvas);
        Assert.IsNotNull(speciesText);
    }

    public void Start()
    {
        inventory = GlobalControl.Instance.savedValues.Inventory;
        //inventoryECS = GlobalControl.Instance.savedValues.Inventory;
    }

    public IEnumerator SelectPollen(PlantFE plantToPollinate)
    {
        Pollen pollen = null;
        OpenInventory();
        // UNDONE: Set a prompt to sellect pollen from inventory
        Debug.Log("Select pollen from inventory");
        while (pollen == null)
        {
            // Stop this coroutine if the inventory is closed without selecting a pollen
            if (!inventoryCanvas.gameObject.activeSelf)
            {
                yield break;
            }

            foreach (Transform pollenT in pollenPanel.transform)
            {
                if (pollenT.GetComponent<PollenFrontEnd>().SelectedItem != null)
                {
                    pollen = pollenT.GetComponent<PollenFrontEnd>().Pollen;
                }
            }
            yield return null;
        }

        plantToPollinate.Plant.Pollinate(pollen);
        inventory.InventoryPollen.Remove(pollen);
        RefreshInventory(inventory.InventoryPollen, pollenPanel.transform);
        CloseInventory();
        Player.PlayerInstance.CurrentObjectMenu.CloseMenu();
        yield return null;
    }

    public void OpenInventory()
    {
        LoadInventory();
        LoadSpecies();
        inventoryCanvas.gameObject.SetActive(true);
        Player.PlayerInstance.GivePlayerControl(false);
    }

    public void CloseInventory()
    {
        inventoryCanvas.gameObject.SetActive(false);
        Player.PlayerInstance.GivePlayerControl(true);
    }

    public void ToggleInventory()
    {
        if (!inventoryCanvas.gameObject.activeSelf)
        {
            OpenInventory();
        }
        else
        {
            CloseInventory();
        }
    }

    private void LoadSpecies()
    {
        speciesText.text = "Species unlocked: ";
        foreach (Taxonomy taxonomy in GlobalControl.Instance.savedValues.AllSpecies)
        {
            speciesText.text += taxonomy.ToString() + ", ";
        }
    }

    private void LoadInventory()
    {
        //LoadPanel(inventory.InventoryPollen, pollenPanel.transform);
        LoadPanel(inventory.InventoryPollenECS, pollenPanel.transform);
        LoadPanel(inventory.InventorySeeds, seedPanel.transform);
    }

    private void LoadPanel(List<PollenECS> inventoryList, Transform panelTransform)
    {
        foreach (PollenECS pollen in inventoryList)
        {
            if (!pollen.IsLoaded)
            {
                GameObject pollenGO = Instantiate(Resources.Load(ConstantValues.Prefabs.PollenUI), panelTransform) as GameObject;
                //pollenGO.GetComponent<PollenFrontEnd>().Pollen = pollen;
                pollen.IsLoaded = true;
            }
        }
    }

    private void LoadPanel(List<Pollen> inventoryList, Transform panelTransform)
    {
        foreach (Pollen pollen in inventoryList)
        {
            if (!pollen.IsLoaded)
            {
                GameObject pollenGO = Instantiate(Resources.Load(ConstantValues.Prefabs.PollenUI), panelTransform) as GameObject;
                pollenGO.GetComponent<PollenFrontEnd>().Pollen = pollen;
                pollen.IsLoaded = true;
            }
        }
    }

    private void RefreshInventory(List<Pollen> inventoryList, Transform panelTransform)
    {
        foreach (Transform child in panelTransform)
        {
            if (child.GetComponent<PollenFrontEnd>())
            {
                Destroy(child.gameObject);
            }
            else
            {
                Debug.LogWarning("Child of inventory panel does not have pollen comp.");
            }
        }

        foreach (Pollen pollen in inventoryList)
        {
            pollen.IsLoaded = false;
        }
    }
}

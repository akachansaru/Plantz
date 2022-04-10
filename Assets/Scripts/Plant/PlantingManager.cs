using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class PlantingManager : MonoBehaviour
{
    public static PlantingManager Instance;

    [SerializeField] private GameObject seedCanvas = null;
    [SerializeField] private GameObject seedPanel = null;
    [SerializeField] private GameObject potCanvas = null;
    [SerializeField] private GameObject potPanel = null;
    [SerializeField] private GameObject soilCanvas = null;
    [SerializeField] private GameObject soilPanel = null;
    [SerializeField] private GameObject pottingCanvas = null;
    [SerializeField] private GameObject pottingPanel = null;

    [SerializeField] private float potUIScale = 100f;

    private Dictionary<Taxonomy, int> taxonomies = new Dictionary<Taxonomy, int>();

    private static Taxonomy seedToPlant = null;
    private static Pot potToPlant = null;
    private static Soil soilToPot = null;

    private void Awake()
    {
        Assert.IsNotNull(seedCanvas);
        Assert.IsNotNull(seedPanel);
        Assert.IsNotNull(potCanvas);
        Assert.IsNotNull(potPanel);
        Assert.IsNotNull(soilCanvas);
        Assert.IsNotNull(soilPanel);
        Assert.IsNotNull(pottingCanvas);
        Assert.IsNotNull(pottingPanel);
    }

    public void Start()
    {
        Instance = this;

        foreach(Pollen seed in GlobalControl.Instance.savedValues.Inventory.InventorySeeds)
        {
            CountTaxa(seed.Taxonomy);
        }

        foreach(KeyValuePair<Taxonomy, int> taxa in taxonomies)
        {
            LoadSeed(taxa);
        }
    }

    public void OnStartGrowingButton()
    {
        GlobalControl.Instance.savedValues.WorktablePlant = new Plant(seedToPlant, potToPlant, ConstantValues.SaveLists.Greenhouse);
        SceneLoader.LoadSceneStatic(ConstantValues.Scenes.Outdoors);
    }

    public void SwitchToPotCanvas(Taxonomy seed)
    {
        seedToPlant = seed;
        potCanvas.SetActive(true);
        seedCanvas.SetActive(false);

        foreach(Pot pot in GlobalControl.Instance.savedValues.Pots)
        {
            LoadPot(pot);
        }
    }

    public void SwitchToSoilCanvas(Pot pot)
    {
        potToPlant = pot;
        soilCanvas.SetActive(true);
        potCanvas.SetActive(false);

        foreach (Soil soil in GlobalControl.Instance.savedValues.Soils)
        {
            LoadSoil(soil);
        }
    }

    public void SwitchToPottingCanvas(Soil soil)
    {
        soilToPot = soil;
        potToPlant.FillWithSoil(soil);

        pottingCanvas.SetActive(true);
        soilCanvas.SetActive(false);

        // bring in the pot graphic and add animation of soil pouring in then seed getting planted in the dirt
        GameObject potUI = Instantiate(Resources.Load(ConstantValues.Prefabs.PotUI), pottingCanvas.transform) as GameObject;
        potUI.GetComponent<PotFrontEnd>().Pot = potToPlant;

        potUI.GetComponent<RectTransform>().localScale = (potToPlant.PotSize * potUIScale).ToVector3();
        PotFrontEnd.ApplyColor(potUI, potToPlant);

    }

    private void LoadPot(Pot pot)
    {
        GameObject potUI = Instantiate(Resources.Load(ConstantValues.Prefabs.PotUI), potPanel.transform) as GameObject;
        potUI.GetComponent<PotFrontEnd>().Pot = pot;

        potUI.GetComponent<RectTransform>().localScale = (pot.PotSize * potUIScale).ToVector3();
        PotFrontEnd.ApplyColor(potUI, pot);

        // Expand the scroll content view so it always holds all the pots
        Vector2 sizeDelta = potPanel.GetComponent<RectTransform>().sizeDelta;
       // float contentIncrease = potUI.transform.localScale.x + potPanel.GetComponent<HorizontalLayoutGroup>().spacing;
        float contentIncrease = potPanel.GetComponent<HorizontalLayoutGroup>().spacing;

        potPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x + contentIncrease, sizeDelta.y);
        //ExpandContentPanel(potPanel, potUI.GetComponent<LayoutElement>(), new Vector2(1, 0));
    }

    private void LoadSoil(Soil soil)
    {
        GameObject soilUI = Instantiate(Resources.Load(ConstantValues.Prefabs.SoilUI), soilPanel.transform) as GameObject;
        soilUI.GetComponent<SoilFrontEnd>().Soil = soil;

        // Expand the scroll content view so it always holds all the soils
        Vector2 sizeDelta = soilPanel.GetComponent<RectTransform>().sizeDelta;
        float contentIncrease = soilPanel.GetComponent<HorizontalLayoutGroup>().spacing;

        soilPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x + contentIncrease, sizeDelta.y);
    }

    //private void ExpandContentPanel(GameObject panel, LayoutGroup layoutGroup, LayoutElement addedItem, Vector2 increaseDimensions)
    //{
    //    Vector2 sizeDelta = panel.GetComponent<RectTransform>().sizeDelta;
    //    float contentIncrease = addedItem.preferredWidth + layoutGroup.spacing;
    //    panel.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x + (contentIncrease * increaseDimensions.x), 
    //                                                                sizeDelta.y + (contentIncrease * increaseDimensions.y));
    //}

    private void CountTaxa(Taxonomy seed)
    {
        if (!taxonomies.ContainsKey(seed))
        {
            taxonomies.Add(seed, 1);
        }
        else
        {
            taxonomies[seed]++;
        }
    }

    private void LoadSeed(KeyValuePair<Taxonomy, int> seed)
    {
        GameObject seedUI = Instantiate(Resources.Load(ConstantValues.Prefabs.SeedButton), seedPanel.transform) as GameObject;
        seedUI.GetComponent<SeedUI>().Taxonomy = seed.Key;
        seedUI.GetComponent<SeedUI>().Amount = seed.Value;

        // Expand the scroll content view so it always holds all the seeds
        Vector2 sizeDelta = seedPanel.GetComponent<RectTransform>().sizeDelta;
        float contentIncrease = seedUI.GetComponent<LayoutElement>().preferredHeight + seedPanel.GetComponent<VerticalLayoutGroup>().spacing;
        seedPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y + contentIncrease);
        //ExpandContentPanel(seedPanel, seedUI.GetComponent<LayoutElement>(), new Vector2(0, 1));
    }
}

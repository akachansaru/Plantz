using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Assets.Scripts.Utilities;
using Unity.Entities;

[RequireComponent(typeof(PlantFE))]
public class PlantMenuECS : ObjectMenu
{
    [SerializeField] private GameObject plantPreviewPanel = null;

    [SerializeField] private Text sellButtonText = null;

    [SerializeField] private Text ageText = null;
    [SerializeField] private Text healthText = null;
    [SerializeField] private Text biomesText = null;
    [SerializeField] private Text growingSeasonsText = null;
    [SerializeField] private Text floweringSeasonsText = null;

    [SerializeField] private Text waterText = null;
    [SerializeField] private Text tempText = null;
    [SerializeField] private Text humidText = null;

    [SerializeField] private Text rarityText = null;
    [SerializeField] private Text taxonomyText = null;
    [SerializeField] private Text advancedStatsText = null;

    private float plantPrice;

    private Entity rootEntity;
    private EntityManager entityManager;

    public Text SellButtonText { get { return sellButtonText; } private set { sellButtonText = value; } }

    public override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(sellButtonText);
        Assert.IsNotNull(plantPreviewPanel);

        Assert.IsNotNull(ageText);
        Assert.IsNotNull(healthText);
        Assert.IsNotNull(biomesText);
        Assert.IsNotNull(growingSeasonsText);
        Assert.IsNotNull(floweringSeasonsText);
        Assert.IsNotNull(waterText);
        Assert.IsNotNull(tempText);
        Assert.IsNotNull(humidText);
        Assert.IsNotNull(rarityText);
        Assert.IsNotNull(taxonomyText);
        Assert.IsNotNull(advancedStatsText);
    }

    public override void Start()
    {
        base.Start();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public override void Update()
    {
        base.Update();
        if (Canvas.activeSelf)
        {
            DisplayTextUpdate();
        }
    }

    public override void OpenMenu()
    {
        base.OpenMenu();
        rootEntity = GetComponent<GameObjectEntityLink>().BaseEntity;
        DisplayText();
        SetUpPlantPreview();
    }

    // For the sell button on the menu canvas
    public void ButtonSellPlant()
    {
        AddMoney();
        DeletePlant();
        Player.PlayerInstance.GivePlayerControl(true);
        Destroy(Canvas);
    }

    private void DisplayText()
    {
        DisplayTaxonomyText();
        DisplayRarityText();
        DisplayAdvancedStatsText();
        DisplayBiomesText();
        DisplayGrowingSeasonsText();
        DisplayFloweringSeasonsText();
    }

    private void DisplayTextUpdate()
    {
        DisplaySellText();
        //ageText.text = "Age: " + plant.GetAge().ToString(); // FIXME: this should be based on my date/time
        //healthText.text = "Health: " + plant.Health;
        //waterText.text = "Water: " + plant.Pot.Soil.CurrentMoistureLevel;
        DisplayTemperatureText();
        DisplayHumidityText();
    }

    private void SetUpPlantPreview()
    {
        //GameObject previewPlant = Instantiate(originalParentPlant.gameObject) as GameObject;
        //Destroy(previewPlant.transform.Find("PlantCanvas").gameObject);
        //previewPlant.GetComponent<PlantFE>().enabled = false;
        //previewPlant.transform.parent = plantPreviewPanel.transform;
        // UNDONE: make the plant preview pane to rotate, zoom, pan etc so the player can visually inspect their plant
    }

    private void AddMoney()
    {
        GlobalControl.Instance.savedValues.Inventory.Money += plantPrice;
    }

    private void DeletePlant()
    {
        //BenchSegment benchSegment = originalParentPlant.parent.GetComponent<BenchSegment>();
        //int benchNum = benchSegment.transform.parent.GetComponent<Bench>().BenchNum;
        //int benchSegmentNum = benchSegment.GetBenchSegmentNum();
        //benchSegment.RemovePlantFromBench();
        //GlobalControl.Instance.savedValues.GreenhousePlants[benchNum][benchSegmentNum] = null;
       // Destroy(originalParentPlant.gameObject);
    }

    private void DisplaySellText()
    {
        //plantPrice = MoneyManager.GetPriceOfPlant(plantFE);
        //SellButtonText.text = "Sell for )(" + plantPrice;
    }

    private void DisplayBiomesText()
    {
        biomesText.text = "Biomes: ";
        DynamicBuffer<Biomes> biomes = entityManager.GetBuffer<BiomesBufferElement>(rootEntity).Reinterpret<Biomes>();
        for (int b = 0; b < biomes.Length; b++)
        {
            if (b < biomes.Length - 1)
            {
                biomesText.text += biomes[b] + ", ";
            }
            else
            {
                biomesText.text += biomes[b];
            }
        }
    }

    private void DisplayGrowingSeasonsText()
    {
        growingSeasonsText.text = "Growing seasons: ";
        //for (int s = 0; s < taxonomy.Species.GrowingSeasons.Count; s++)
        //{
        //    if (s < taxonomy.Species.GrowingSeasons.Count - 1)
        //    {
        //        growingSeasonsText.text += taxonomy.Species.GrowingSeasons[s] + ", ";
        //    }
        //    else
        //    {
        //        growingSeasonsText.text += taxonomy.Species.GrowingSeasons[s];
        //    }
        //}
    }

    private void DisplayFloweringSeasonsText()
    {
        floweringSeasonsText.text = "Flowering seasons: ";
        //for (int s = 0; s < taxonomy.Species.FloweringSeasons.Count; s++)
        //{
        //    if (s < taxonomy.Species.FloweringSeasons.Count - 1)
        //    {
        //        floweringSeasonsText.text += taxonomy.Species.FloweringSeasons[s] + ", ";
        //    }
        //    else
        //    {
        //        floweringSeasonsText.text += taxonomy.Species.FloweringSeasons[s];
        //    }
        //}
    }

    private void DisplayTemperatureText()
    {
        float currTemp = GetComponentInParent<GreenhouseClimate>().CurrentTemperature;
        //RequirementRange range = taxonomy.Species.GetTemperatureRange();

        //tempText.text = string.Format("Current temperature: " + currTemp + "\n" + "Temparature range: " + range.Low + " to " + range.High); 
    }

    private void DisplayHumidityText()
    {
        float currHumid = GetComponentInParent<GreenhouseClimate>().CurrentHumidity;
        //RequirementRange range = taxonomy.Species.GetHumidityRange();

        //humidText.text = string.Format("Current humidity: " + currHumid + "\n" + "Humidity range: " + range.Low + " to " + range.High);
    }

    private void DisplayTaxonomyText()
    {
        taxonomyText.text = string.Format("{0} {1} {2}", 
            entityManager.GetComponentData<FamilyName>(rootEntity).Value, // .ToUpper()
            entityManager.GetComponentData<GenusName>(rootEntity).Value, 
            entityManager.GetComponentData<SpeciesName>(rootEntity).Value); // .ToLower()
    }

    private void DisplayRarityText()
    {
        //rarityText.text = string.Format("Family: {0}, Genus: {1}, Species: {2}",
        //    taxonomy.Species.Genus.Family.Rarity, 
        //    taxonomy.Species.Genus.Rarity, 
        //    taxonomy.Species.Rarity);
        rarityText.text = string.Format("Species: {0}",
        entityManager.GetComponentData<RarityComponent>(rootEntity).Value);

    }

    private void DisplayAdvancedStatsText()
    {
        advancedStatsText.text = string.Format("PlantMaxSize: {0}, StemGrowthTime: {1}, leavesPerNode: {2}, \n" +
            "nodesBetweenLeaves: {3}, leafInitialSize: {4}, leafMaxSize: {5}, \n" +
            "leafGrowthRate: {6}, flowersPerNode: {7}, nodesBetweenFlowers: {8}, \n" +
            "flowerInitialSize: {9}, flowerMaxSize: {10}, flowerGrowthRate {11}, \n" +
            "stemGrowthRate: {12}, stemGrowthPercent: {13}, stemInitialSize: {14}, \n" +
            "stemMaxSize: {15}",
            entityManager.GetComponentData<MaxPlantSizeComponent>(rootEntity).Value.value,
            entityManager.GetComponentData<StemGrowthTimeComponent>(rootEntity).Value.value,
            entityManager.GetComponentData<LeavesPerNodeComp>(rootEntity).Value,
            entityManager.GetComponentData<NodesBetweenLeavesComp>(rootEntity).Value,
            entityManager.GetComponentData<LeafInitialSizeComp>(rootEntity).Value.value,
            entityManager.GetComponentData<LeafMaxSizeComp>(rootEntity).Value.value,
            entityManager.GetComponentData<WidenLeafTimeComponent>(rootEntity).Value.value,
            entityManager.GetComponentData<FlowersPerNodeComp>(rootEntity).Value,
            entityManager.GetComponentData<NodesBetweenFlowersComp>(rootEntity).Value,
            entityManager.GetComponentData<FlowerInitialSizeComp>(rootEntity).Value.value,
            entityManager.GetComponentData<FlowerMaxSizeComp>(rootEntity).Value.value,
            entityManager.GetComponentData<WidenFlowerTimeComponent>(rootEntity).Value.value,
            entityManager.GetComponentData<WidenStemTimeComponent>(rootEntity).Value.value,
            entityManager.GetComponentData<StemWidenPercentComp>(rootEntity).Value.value,
            entityManager.GetComponentData<InitialStemWidthComp>(rootEntity).Value.value,
            entityManager.GetComponentData<MaxStemWidthComp>(rootEntity).Value.value);
    }
}
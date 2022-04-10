using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;

public class PlantFE : MonoBehaviour
{
    [SerializeField] private Plant plant = null;
    [SerializeField] private GameObject firstStem = null;
    //[SerializeField] private List<GameObject> lastYearsFlowerGOs = new List<GameObject>();
    //[SerializeField] private List<GameObject> thisYearsFlowerGOs = new List<GameObject>();
    [SerializeField] private List<GameObject> flowerGOs = new List<GameObject>();
    [SerializeField] private List<GameObject> fruitGOs = new List<GameObject>();
    [SerializeField] private List<GameObject> matureFruitGOs = new List<GameObject>();

    private static float BarrierBuffer { get; } = 0.5f; // How far from the wall plants can grow. The stem global magnitude is added to this too

    // Plant/stems
    public Plant Plant { get { return plant; } set { plant = value; } }
    public GameObject FirstStemGO { get { return firstStem; } set { firstStem = value; } }
    // This is to iterate over for widening stems. Filled in PlantUtilities when the plant is loaded and added to when a stem is grown. 
    // Empties won't be added when created or loaded. 
    public List<GameObject> StemsGOs { get; set; } = new List<GameObject>();
    // This is filled in PlantUtilities when the plant is loaded and added to when a leaf is grown
    private List<GameObject> GrowingStemGOs { get; set; } = new List<GameObject>(); // PERFORMANCE: Can this be filled when the plant is loaded?
    private bool IsGrowing { get; set; } = false;

    // Leaves
    public List<GameObject> LeafGOs { get; } = new List<GameObject>();
    private float LeafDroppingRate { get; set; }
    private bool IsLeafDropping { get; set; } = false;

    // Flowers
    //public List<GameObject> LastYearsFlowerGOs { get { return lastYearsFlowerGOs; } }
    //public List<GameObject> ThisYearsFlowerGOs { get { return thisYearsFlowerGOs; } }
    public List<GameObject> FlowerGOs { get { return flowerGOs; } }
    private bool IsFlowering { get; set; } = false;
    private float FlowerDroppingRate { get; set; }
    private bool IsFlowerDropping { get; set; } = false;
    //private bool resetFlowerGOs = false;

    // Fruit
    public List<GameObject> FruitGOs { get { return fruitGOs; } }
    public List<GameObject> MatureFruitGOs { get { return matureFruitGOs; } }
    private float FruitDroppingRate { get; set; }
    private bool IsFruitDropping { get; set; } = false;


    private Coroutine growPlant;
    private Coroutine growLeaves;
    private Coroutine growFlowers;
    private Coroutine growFruit;
    private Coroutine dropLeaves;
    private Coroutine dropFruit;
    private Coroutine widenStems = null;

    public void Start()
    {
        PotFrontEnd.ApplyColor(gameObject, Plant.Pot);
        PotFrontEnd.ApplySize(gameObject, Plant.Pot);
        InitializeGrowingStemGOs(); // FIXME: do I need GrowingStemGOs anymore?
        IsLeafDropping = false;
        IsGrowing = false;

        // *** All this is to try to remove the branches growing though boundaries for InstaGrow plants
        //List<GameObject> stemsToDestroy = new List<GameObject>();
        //List<GameObject> leavesToDestroy = new List<GameObject>();
        //List<GameObject> flowersToDestroy = new List<GameObject>();
        //List<GameObject> stemsToDestroyTemp = new List<GameObject>();
        ////List<GameObject> leavesToDestroyTemp = new List<GameObject>();
        ////List<GameObject> flowersToDestroyTemp = new List<GameObject>();

        //List<GameObject> tempEmpties = new List<GameObject>(); // Empties holding flowers/leaves
        //List<GameObject> branchBases = new List<GameObject>(); // Empties at the start of each branch
        //List<GameObject> branchBasesTemp = new List<GameObject>(); // Empties at the start of each branch

        //foreach (GameObject stemGO in StemsGOs)
        //{ // check if IsBaseBranch is working correctly then use that instead of == 0
        //    if (stemGO.GetComponent<StemFE>().Stem.Branch.CurrStep == 0 && stemGO.GetComponent<StemFE>().Stem.IsEmpty) // Check each child of each basebranch to avoid checking comps that have already been removed
        //    {
        //        branchBasesTemp.Add(stemGO); // Will only add to branchBases if a stem in the branch intersects
        //        bool branchIntersects = false;
        //        foreach (Transform child in stemGO.transform)
        //        {
        //            if (child.TryGetComponent(out StemFE stemFE))
        //            {
        //                if (!stemFE.Stem.IsEmpty) // Skip the empties for now since they'll only contain leaves/flowers
        //                {
        //                    stemsToDestroyTemp.Add(child.gameObject);
        //                    if (HitBarrier(child.gameObject, child.gameObject))
        //                    {
        //                        branchIntersects = true; // If any segments in the branch intersect will remove them all
        //                    }
        //                }
        //                else
        //                {
        //                    // add to another list to loop through all children if branchIntersects is true to get the leaves/flowers to remove
        //                    tempEmpties.Add(child.gameObject);
        //                }
        //            }
        //        }

        //        if (branchIntersects) // Add all to the permanant list
        //        {
        //            branchBases.AddRange(branchBasesTemp);
        //            stemsToDestroy.AddRange(stemsToDestroyTemp);
        //            //leavesToDestroy.AddRange(leavesToDestroyTemp);
        //            //flowersToDestroy.AddRange(flowersToDestroyTemp);
        //            //Debug.Log("empties: " + tempEmpties.Count);
        //            //Debug.Log("branch intersects. Adding leaves/flowers for removal");
        //            //foreach (GameObject plantComp in tempEmpties)
        //            //{
        //            //    foreach (Transform child in plantComp.transform)
        //            //    {
        //            //        if (child.TryGetComponent(out LeafFE leafFE))
        //            //        {
        //            //            Debug.Log("found leaf ");
        //            //            leavesToDestroy.Add(child.gameObject);
        //            //        }
        //            //        else if (child.TryGetComponent(out FlowerFE flowerFE))
        //            //        {
        //            //            Debug.Log("found flower ");
        //            //            flowersToDestroy.Add(child.gameObject);
        //            //        }
        //            //    }
        //            //}
        //        }
        //        branchBasesTemp.Clear();
        //        stemsToDestroyTemp.Clear();
        //        tempEmpties.Clear();
        //        //leavesToDestroyTemp.Clear();
        //        //flowersToDestroyTemp.Clear();
        //    }
        //}

        //Debug.Log("leaves: " + leavesToDestroy.Count);
        //Debug.Log("flowers: " + flowersToDestroy.Count);
        //Debug.Log("stems: " + stemsToDestroy.Count);

        ////foreach (GameObject leafGO in leavesToDestroy)
        ////{
        ////    Debug.Log("removed leaf");
        ////    LeafGOs.Remove(leafGO);
        ////}

        ////foreach (GameObject flowerGO in flowersToDestroy)
        ////{
        ////    Debug.Log("removed flower");
        ////    FlowerGOs.Remove(flowerGO);
        ////}

        //foreach (GameObject stemGO in stemsToDestroy)
        //{
        //    Debug.Log("removed stem");
        //    StemsGOs.Remove(stemGO);
        //}

        //foreach (GameObject branchBase in branchBases)
        //{
        //    Debug.Log("branchBase children: " + branchBase.transform.childCount);
        //    foreach (Transform stem in branchBase.transform)
        //    {
        //        foreach (Transform child in stem)
        //        {
        //            if (child.TryGetComponent(out LeafFE leafFE))
        //            {
        //                Debug.Log("found leaf ");
        //                LeafGOs.Remove(child.gameObject);
        //            }
        //            else if (child.TryGetComponent(out FlowerFE flowerFE))
        //            {
        //                Debug.Log("found flower ");
        //                FlowerGOs.Remove(child.gameObject);
        //            }
        //        }
        //    }
        //    Debug.Log("destroyed branch " + branchBase.name);
        //    Destroy(branchBase); // Should be the empty at the base and hopefully all the leaves/flowers too
        //}
    }

    // Will need this if I add back in flowers growing on last year's growth
    private void ResetFlowers()
    {
        //if (LastYearsFlowerGOs.Count > 0)
        //{
        //    if ((Seasons)GlobalControl.Instance.savedValues.Season == Seasons.Spring && !resetFlowerGOs)
        //    {
        //        resetFlowerGOs = true;
        //        LastYearsFlowerGOs.Clear();
        //        LastYearsFlowerGOs.AddRange(ThisYearsFlowerGOs);
        //        ThisYearsFlowerGOs.Clear();
        //        Plant.LastYearsFlowers.Clear();
        //        Plant.LastYearsFlowers.AddRange(Plant.ThisYearsFlowers);
        //        Plant.ThisYearsFlowers.Clear();
        //    }
        //    else if ((Seasons)GlobalControl.Instance.savedValues.Season == Seasons.Summer)
        //    {
        //        resetFlowerGOs = false;
        //    }
        //}
    }

    public void Update()
    {
        if (!GameTime.GamePaused)
        {
            if (Plant.Taxonomy.Species != null) // might be able to get rid of .Species here
            {
                HealthCheck();
                DryOutSoil();
                //ResetFlowers();
                LeafDropCheck();
                FloweringCheck();
                GrowingSeasonCheck();
                FruitingCheck();
                DropFruitCheck();
                WidenStemsCheck();
                UpdateFruitCheck();
            }
        }
    }

    private void DryOutSoil()
    {
        if (GetComponentInParent<GreenhouseClimate>()) // Otherwise it will be an outdoor plant and the biome should be fine for it
        {
            if (Plant.Pot.Soil.CurrentMoistureLevel >= 0)
            {
                float currTemp = GetComponentInParent<GreenhouseClimate>().CurrentTemperature;

                float tempFactor = currTemp / Plant.Taxonomy.Species.GetTemperatureRange().Ave;
                float humidityFactor = currTemp / Plant.Taxonomy.Species.GetHumidityRange().Ave;

                float climateFactor = tempFactor + humidityFactor;
                Plant.Pot.Soil.AdjustMoistureLevel(-climateFactor / Plant.Pot.Soil.HoursToDryOut * Time.deltaTime); // FIXME: I think I need Time.deltaTime here?
            }
        }
    }

    private void HealthCheck()
    {
        if (Plant.Health <= 0)
        {
            Debug.Log(Plant.PlantID + " is dead.");
            // UNDONE: need to stop it from growing new leaves and flowers

            // These should stop it from growing anymore
            Plant.IsMaxCaliper = true;
            Plant.IsMaxSize = true;
        }
        // UNDONE: need to start dropping leaves and flowers and slow down stem growth when the health gets bad

        if (GetComponentInParent<GreenhouseClimate>()) // Otherwise it will be an outdoor plant and the biome should be fine for it
        {
            TempCheck();
            HumidityCheck();
        }
    }

    /// <summary>
    /// Returns true if the temp is in a good range, false if in bad range and adjusts the plant health
    /// </summary>
    private void TempCheck()
    {
        float currTemp = GetComponentInParent<GreenhouseClimate>().CurrentTemperature;
        RequirementRange range = Plant.Taxonomy.Species.GetTemperatureRange();

        if (currTemp < range.Low)
        {
            Plant.Health -= (range.Low - currTemp) / GameTime.SEASON; // TODO: change this to a Fragility factor - more fragile plants will die faster
        }
        if (currTemp > range.High)
        {
            Plant.Health -= (currTemp - range.High) / GameTime.SEASON;
        }
    }

    private void HumidityCheck()
    {
        float currHumidity = GetComponentInParent<GreenhouseClimate>().CurrentHumidity;
        RequirementRange range = Plant.Taxonomy.Species.GetHumidityRange();

        if (currHumidity < range.Low)
        {
            Plant.Health -= (range.Low - currHumidity) / GameTime.SEASON;
        }
        if (currHumidity > range.High)
        {
            Plant.Health -= (currHumidity - range.High) / GameTime.SEASON;
        }
    }

    private void LeafDropCheck()
    {
        if ((Seasons)GlobalControl.Instance.savedValues.Season == Seasons.Fall) // UNDONE: this needs to be set based on the plant
        {
            if (!IsLeafDropping)
            {
                Debug.Log("In leaf dropping season.");
                IsLeafDropping = true;
                dropLeaves = StartCoroutine(DropLeaves());
            }
        }
    }

    private void FloweringCheck()
    {
        if (IsFloweringSeason())
        {
            if (!IsFlowering && FirstStemGO) // Check if the plant is ready to start growing (it won't be if still being planted for example)
            {
                //Debug.Log("In flowering season.");
                IsFlowering = true;
                //if (Plant.Taxonomy.Species.OnNewGrowth)
                //{
                //    growFlowers = StartCoroutine(GrowFlowers(ThisYearsFlowerGOs));
                //}
                //else
                //{
                //    growFlowers = StartCoroutine(GrowFlowers(LastYearsFlowerGOs));
                //}
                growFlowers = StartCoroutine(GrowFlowers(FlowerGOs));
            }
        }
        else
        {
            // Stop the flowers from growing and start dropping them. TODO: the flowers shouldn't still be growing once the growing season is over. Add a check in Taxonomy so the growth time will always fit into the growing seasons?
            if (IsFlowering)
            {
                Debug.Log("Not in flowering season");
                IsFlowering = false;
                StopCoroutine(growFlowers);
            }

            if (!IsFlowerDropping)
            {
                IsFlowerDropping = true;
                //if (Plant.Taxonomy.Species.OnNewGrowth)
                //{
                //    StartCoroutine(DropFlowers(ThisYearsFlowerGOs, Plant.ThisYearsFlowers));
                //}
                //else
                //{
                //    StartCoroutine(DropFlowers(LastYearsFlowerGOs, Plant.LastYearsFlowers));
                //}
                StartCoroutine(DropFlowers());
            }
        }

        if (FlowerGOs.Count == 0 && IsFlowering)
        {
            StopCoroutine(growFlowers);
            IsFlowering = false;
        }
    }

    private void GrowingSeasonCheck()
    {
        if (IsGrowingSeason())
        {
            if (!IsGrowing && FirstStemGO) // Check if the plant is ready to start growing (it won't be if still being planted for example)
            {
                Debug.Log("In growing season");
                IsGrowing = true;
                AddLastYearsLeaves();
                growPlant = StartCoroutine(GrowPlant());
                growLeaves = StartCoroutine(GrowLeaves());
                widenStems = StartCoroutine(WidenStems());
            }
        }
        else
        {
            if (IsGrowing)
            {
                Debug.Log("Not in season");
                IsGrowing = false;
                StopCoroutine(growPlant);
                StopCoroutine(growLeaves);
                StopCoroutine(widenStems);
            }
        }
    }

    private void FruitingCheck()
    {
        if (FruitGOs.Count > 0)
        {
            growFruit = StartCoroutine(GrowFruit());
        }
    }

    // could also have Drop methods in the CompFE and have a variance for the time that they wait to drop after mature
    private void DropFruitCheck()
    {
        // start dropping fruit once it's two seasons after flowering season
        List<Seasons> floweringSeasons = Plant.Taxonomy.Species.FloweringSeasons;
        if ((Seasons)GlobalControl.Instance.savedValues.Season == floweringSeasons[floweringSeasons.Count - 1] + 2)
        {
            if (!IsFruitDropping)
            {
                Debug.Log("In fruit dropping season.");
                IsFruitDropping = true;
                dropFruit = StartCoroutine(DropFruit());
            }
        }
    }

    private void WidenStemsCheck()
    {
        if (widenStems != null)
        {
            if (FirstStemGO.transform.lossyScale.x >= Plant.Taxonomy.Species.StemMaxSize.Value)
            {
                StopCoroutine(widenStems);
                Plant.IsMaxCaliper = true;
                Debug.Log(Plant.PlantID + " reached max caliper.");
            }
            if (FirstStemGO.transform.lossyScale.x >= Plant.Pot.PotSize.x || FirstStemGO.transform.lossyScale.x >= Plant.Pot.PotSize.z)
            {
                // TODO: Add in a buffer so the plant doesn't grow out of the pot
                StopCoroutine(widenStems);
                Debug.Log(Plant.PlantID + " needs larger pot.");
            }
        }
    }

    private void UpdateFruitCheck()
    {
        if (Plant.FruitNeedsUpdating)
        {
            Plant.FruitNeedsUpdating = false;
            foreach (GameObject flowerGO in FlowerGOs)
            {
                Destroy(flowerGO);
            }
            FlowerGOs.Clear();
            foreach (Fruit fruit in Plant.Fruit)
            {
                GameObject gameObject = Instantiate(Resources.Load(ConstantValues.Prefabs.Fruit)) as GameObject;
                gameObject.GetComponent<FruitFE>().Load(fruit, false, FruitGOs);
            }
        }
    }

    /// <summary>
    /// For use with the UI button in the plant menu
    /// </summary>
    public void ButtonWater()
    {
        Plant.Pot.Soil.Water();
    }

    /// <summary>
    /// For use with the UI button in the plant menu
    /// </summary>
    public void ButtonRepot()
    {
        Debug.LogWarning("Not yet implemented");
        // UNDONE: Implement repotting
        //Worktable.SelectedPlantGO = gameObject;
        //SceneLoader.LoadSceneStatic(ConstantValues.Scenes.Worktable);
    }

    /// <summary>
    /// For use with the UI button in the plant menu
    /// </summary>
    public void ButtonPollinate()
    {
        if (IsFlowering && !Plant.IsPollinated)
        {
            StartCoroutine(Player.PlayerInstance.InventoryManager.SelectPollen(this));
        }
        else if (!IsFlowering)
        {
            Debug.LogWarning("Can't pollinate when not flowering.");
        }
        else if (Plant.IsPollinated)
        {
            Debug.LogWarning("Plant is already pollinated");
        }
    }

    // Pick all the fruit at once currently
    public void ButtonHarvestFruit()
    {
        Debug.Log("harvest");
        int numMatureFruit = 0;
        List<GameObject> tempList = new List<GameObject>();
        bool isCultivar = false;
        bool isNewCultivar = false;
        List<Stat> cultivarStats = new List<Stat>();
        Taxonomy cultivarTaxonomy = null;
        foreach (GameObject fruitGO in FruitGOs)
        {
            Fruit fruit = fruitGO.GetComponent<FruitFE>().Comp;
            if (fruit.IsMature)
            {
                Player.PlayerInstance.InventoryManager.Inventory.InventorySeeds.Add(new Pollen(fruit.PollinatedTaxonomy));
                tempList.Add(fruitGO);
                Plant.Fruit.Remove(fruit);
                Destroy(fruitGO);
                numMatureFruit++;
                isNewCultivar = IsNewCultivar(fruit.PollinatedTaxonomy);
                if (fruit.PollinatedTaxonomy.Species.CultivarStats.Count > 0)
                {
                    isCultivar = true;
                    cultivarStats = fruit.PollinatedTaxonomy.Species.CultivarStats;
                    cultivarTaxonomy = fruit.PollinatedTaxonomy;
                }
            }
        }
        tempList.ForEach(g => fruitGOs.Remove(g));
        //FruitGOs.Remove(fruitGO);

        if (isCultivar && isNewCultivar)
        {
            Debug.Log("Discovered new cultivar!");
            // bring up a window to name the new cultivar
            StartCoroutine(GameManager.Instance.OpenCultivarCanvas(cultivarStats, cultivarTaxonomy));
        }
        else if (!isNewCultivar)
        {
            Debug.Log("already discovered " + cultivarTaxonomy.ToString());
        }

        if (numMatureFruit == 0)
        {
            Debug.LogWarning(Plant.PlantID + " has no mature fruit to harvest.");
        }
    }

    /// <summary>
    /// Analyze the seeds to see if they are new cultivars
    /// </summary>
    /// <returns></returns>
    private bool IsNewCultivar(Taxonomy taxonomy)
    {
        foreach (Taxonomy tax in GlobalControl.Instance.savedValues.AllSpecies)
        {
            if (taxonomy.StatID == tax.StatID)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// For use with the UI button in the plant menu
    /// </summary>
    public void ButtonCollectPollen()
    {
        bool collectedPollen = false; // Will be true if any flower in the plant has pollen and it was collected
        //foreach(Flower flower in Plant.ThisYearsFlowers)
        foreach (Flower flower in Plant.Flowers)
        {
            if (flower.HasPollen)
            {
                Debug.Log("Collected pollen from " + Plant.PlantID);
                flower.HasPollen = false;
                collectedPollen = true;
                Player.PlayerInstance.InventoryManager.Inventory.InventoryPollen.Add(new Pollen(Plant.Taxonomy));
                break;
            }
        }
        if (!collectedPollen)
        {
            Debug.Log(Plant.PlantID + " has no more pollen.");
        }
    }

    public GameObject GetBenchSegment(int segmentNum)
    {
        return GetComponentInParent<GreenhouseBench>().BenchSegments[segmentNum];
    }

    public Plant[] GetSaveBench()
    {
        return GetSaveList()[GetComponentInParent<GreenhouseBench>().BenchNum];
    }

    private List<Plant[]> GetSaveList()
    {
        return GetComponentInParent<GreenhouseBench>().saveList;
    }

    public decimal GetPercentMaxSize()
    {
        return (decimal)StemsGOs.Count / Plant.Taxonomy.Species.Genus.PlantMaxSize;
    }

    public decimal GetPercentMaxCaliper()
    {
        return (decimal)((Plant.Stems[0][0].GlobalScale.x - Plant.Taxonomy.Species.StemInitialSize.Value) / Plant.Taxonomy.Species.StemMaxSize.Value);
    }

    private void AddLastYearsLeaves()
    {
        foreach (Leaf leaf in Plant.Leaves)
        {
            if (leaf.IsDropped)
            {
                //PlantUtilities.LoadDroppedLeaf(leaf, gameObject);
                GameObject newLeafGO = Instantiate(Resources.Load(ConstantValues.Prefabs.Leaf)) as GameObject;
                newLeafGO.GetComponent<LeafFE>().LoadDroppedLeaf(leaf, gameObject);
                leaf.IsDropped = false;
            }
        }
    }

    /// <summary>
    /// Removes a random flower from the plant so it can fall and waits up to a second before dropping another
    /// </summary>
    /// <returns></returns>
    //private IEnumerator DropFlowers(List<GameObject> flowerGOs, List<Flower> flowers)
    private IEnumerator DropFlowers()
    {
        for (; ; )
        {
            if (FlowerGOs.Count > 0)
            {
                int rand = Random.Range(0, FlowerGOs.Count);
                GameObject randFlower = FlowerGOs[rand];
                FlowerFE randFlowerFE = randFlower.GetComponent<FlowerFE>();
                FlowerGOs.RemoveAt(rand);

                Plant.Flowers.RemoveAt(rand); // Removing all of them means there will be no flowers growing on last year's growth
                if (randFlowerFE.Comp.IsPollinated)
                {
                    randFlowerFE.StartFruitingFE(this, randFlower.transform.parent.GetComponent<StemFE>());
                }
                randFlowerFE.Drop(randFlower.transform.parent.GetComponent<StemFE>().Stem.Flowers);

                FlowerDroppingRate = Random.Range(0.5f, 2f);
                yield return new WaitForSeconds(FlowerDroppingRate);
            }
            else
            {
                // Should be done growing flowers here
                //IsFlowerDropping = false;
                yield break;
            }
        }
    }

    /// <summary>
    /// Removes a random leaf from the plant so it can fall and waits up to a second before dropping another
    /// </summary>
    /// <returns></returns>
    private IEnumerator DropLeaves()
    {
        for (; ; )
        {
            if (LeafGOs.Count > 0)
            {
                int rand = Random.Range(0, LeafGOs.Count);
                GameObject randLeaf = LeafGOs[rand];
                LeafGOs.RemoveAt(rand);

                if (randLeaf.transform.parent.GetComponent<StemFE>().Stem.Branch.YearGrown != GlobalControl.Instance.savedValues.Year)
                {
                    Plant.Leaves.RemoveAt(rand);
                }

                randLeaf.GetComponent<LeafFE>().Drop(randLeaf.transform.parent.GetComponent<StemFE>().Stem.Leaves);

                //leafDroppingRate = GameTime.timeScale > 0 ? UnityEngine.Random.Range(0.5f, 2f) * GameTime.timeScale : UnityEngine.Random.Range(0.5f, 2f); // TODO could make this scale with how late into fall it is
                LeafDroppingRate = Random.Range(0.5f, 2f);
                yield return new WaitForSeconds(LeafDroppingRate);
            }
            else
            {
                // Should be done growing leaves here
                //IsLeafDropping = false;
                yield break;
            }
        }
    }

    private IEnumerator DropFruit()
    {
        for (; ; )
        {
            if (FruitGOs.Count > 0)
            {
                int rand = Random.Range(0, FruitGOs.Count);
                GameObject randFruit = FruitGOs[rand];
                FruitGOs.RemoveAt(rand);
                Plant.Fruit.RemoveAt(rand);

                randFruit.GetComponent<FruitFE>().Drop(randFruit.transform.parent.GetComponent<StemFE>().Stem.Fruit);

                //fruitDroppingRate = GameTime.timeScale > 0 ? UnityEngine.Random.Range(0.5f, 2f) * GameTime.timeScale : UnityEngine.Random.Range(0.5f, 2f); // TODO could make this scale with how late into fall it is
                FruitDroppingRate = Random.Range(0.5f, 2f);
                yield return new WaitForSeconds(FruitDroppingRate);
            }
            else
            {
                // Should be done growing fruit here
                //IsFruitDropping = false;
                yield break;
            }
        }
    }

    private bool IsGrowingSeason()
    {
        bool isGrowingSeason = false;
        foreach (Seasons season in Plant.Taxonomy.Species.GrowingSeasons)
        {
            if ((Seasons)GlobalControl.Instance.savedValues.Season == season)
            {
                isGrowingSeason = true;
            }
        }
        return isGrowingSeason;
    }

    private bool IsFloweringSeason()
    {
        bool isFloweringSeason = false;
        foreach (Seasons season in Plant.Taxonomy.Species.FloweringSeasons)
        {
            if ((Seasons)GlobalControl.Instance.savedValues.Season == season)
            {
                isFloweringSeason = true;
                break;
            }
        }
        return isFloweringSeason;
    }

    /// <summary>
    /// Initiate growingStemGOs so the plant can grow
    /// </summary>
    private void InitializeGrowingStemGOs()
    {
        GrowingStemGOs.Clear();
        //foreach (string stem in Plant.GrowingStems)
        foreach (Stem stem in Plant.GrowingStems)
        {
            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>(true))
            {
                if (t.gameObject.name == stem.StemID)
                {
                    GrowingStemGOs.Add(t.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Grows the existing flowers based on the growthRate
    /// </summary>
    private IEnumerator GrowFlowers(List<GameObject> flowerGOs)
    {
        yield return new WaitForSeconds(GlobalControl.Instance.savedValues.TimeToFlowerGrowth); // For when the game is loaded. 
        for (; ; )
        {
            foreach (GameObject flowerGO in flowerGOs)
            {
                flowerGO.GetComponent<FlowerFE>().Grow(Plant.Taxonomy.Species.Genus.FlowerMaxSize, ConstantValues.PlantConsts.FlowerPrefabScale.ToVector3());
            }
            float timeToFlowerGrowth;
            for (timeToFlowerGrowth = Plant.Taxonomy.Species.Genus.FlowerGrowthRate; timeToFlowerGrowth > 0; timeToFlowerGrowth -= Time.deltaTime)
            {
                GlobalControl.Instance.savedValues.TimeToFlowerGrowth = timeToFlowerGrowth;
                yield return null;
            }
        }
    }

    /// <summary>
    /// Grows the existing leaves based on the growthRate
    /// </summary>
    private IEnumerator GrowLeaves()
    {
        yield return new WaitForSeconds(GlobalControl.Instance.savedValues.TimeToLeafGrowth); // For when the game is loaded. 
        for (; ; )
        {
            foreach (GameObject leafGO in LeafGOs)
            {
                leafGO.GetComponent<LeafFE>().Grow(Plant.Taxonomy.Species.Genus.LeafMaxSize.Value, ConstantValues.PlantConsts.LeafPrefabScale.ToVector3());
            }
            float timeToLeafGrowth;
            for (timeToLeafGrowth = Plant.Taxonomy.Species.Genus.LeafGrowthRate.Value; timeToLeafGrowth > 0; timeToLeafGrowth -= Time.deltaTime)
            {
                GlobalControl.Instance.savedValues.TimeToLeafGrowth = timeToLeafGrowth;
                yield return null;
            }
        }
    }

    /// <summary>
    /// Grows the existing fruit based on the growthRate
    /// </summary>
    private IEnumerator GrowFruit()
    {
        yield return new WaitForSeconds(GlobalControl.Instance.savedValues.TimeToFruitGrowth); // For when the game is loaded. 
        for (; ; )
        {
            foreach (GameObject fruitGO in FruitGOs)
            {
                fruitGO.GetComponent<FruitFE>().Grow(Plant.Taxonomy.Species.Genus.FruitMaxSize, ConstantValues.PlantConsts.FruitPrefabScale.ToVector3());
                // Will need to add this to GrowFlower and leaf if it works
                if (fruitGO.transform.localScale.x >= Plant.Taxonomy.Species.Genus.FruitMaxSize)
                {
                    fruitGO.GetComponent<FruitFE>().Comp.Mature();
                }
            }
            float timeToFruitGrowth;
            for (timeToFruitGrowth = Plant.Taxonomy.Species.Genus.FruitGrowthRate; timeToFruitGrowth > 0; timeToFruitGrowth -= Time.deltaTime)
            {
                GlobalControl.Instance.savedValues.TimeToFruitGrowth = timeToFruitGrowth;
                yield return null;
            }
        }
    }

    /// <summary>
    /// Widens the existing stems based on their growthRate
    /// </summary>
    private IEnumerator WidenStems()
    {
        float stemGrowthAmt = Plant.Taxonomy.Species.StemGrowthAmt;
        float firstStemGlobalScale = FirstStemGO.GetComponent<StemFE>().Stem.GlobalScale.x;

        yield return new WaitForSeconds(Plant.TimeUntilStemGrowth); // For when the game is loaded. 
        for (; ; )
        {
            if (firstStemGlobalScale + stemGrowthAmt < Plant.Taxonomy.Species.StemMaxSize.Value)
            {
                if (firstStemGlobalScale + stemGrowthAmt < Plant.Pot.PotSize.x &&
                    firstStemGlobalScale + stemGrowthAmt < Plant.Pot.PotSize.z)
                {
                    WidenAllStems(stemGrowthAmt);
                }
                else if (firstStemGlobalScale < Plant.Pot.PotSize.x && firstStemGlobalScale < Plant.Pot.PotSize.z) // Add the extra on to get it to exactly the max size
                {
                    Debug.Log("widening for pot size: " + (Plant.Pot.PotSize.x - firstStemGlobalScale));
                    WidenAllStems(Plant.Pot.PotSize.x - firstStemGlobalScale); // TODO: do a separate one for potSize.z
                }
            }
            else if (firstStemGlobalScale < Plant.Taxonomy.Species.StemMaxSize.Value) // Add the extra on to get it to exactly the max size
            {
                Debug.Log("widening for stem size: " + (Plant.Taxonomy.Species.StemMaxSize.Value - firstStemGlobalScale));
                WidenAllStems(Plant.Taxonomy.Species.StemMaxSize.Value - firstStemGlobalScale);
            }
            float timeUntilStemGrowth;
            for (timeUntilStemGrowth = Plant.Taxonomy.Species.StemGrowthRate.Value; timeUntilStemGrowth > 0; timeUntilStemGrowth -= Time.deltaTime)
            {
                Plant.TimeUntilStemGrowth = timeUntilStemGrowth;
                yield return null;
            }
        }
    }

    private void AdjustLeafAndFlowerPositions(List<GameObject> empties)
    {
        float growthAmt = ConstantValues.PlantConsts.StemGrowthPercent / 2; // Divide by 2 because it's growing evenly on both sides

        foreach (GameObject empty in empties)
        {
            foreach (Transform child in empty.transform)
            {
                if (child.GetComponent<LeafFE>() || child.GetComponent<FlowerFE>())
                {
                    Vector3 moveAmt = new Vector3(
                        growthAmt * child.localPosition.normalized.x,
                        growthAmt * child.localPosition.normalized.y,
                        growthAmt * child.localPosition.normalized.z);
                    if (child.GetComponent<LeafFE>())
                    {
                        child.GetComponent<LeafFE>().SetPosition(child.localPosition + moveAmt);
                    }
                    else
                    {
                        child.GetComponent<FlowerFE>().SetPosition(child.localPosition + moveAmt);
                    }
                }
            }
        }
    }

    private void WidenAllStems(float growthAmt)
    {
        List<GameObject> empties = new List<GameObject>();
        foreach (GameObject stemGO in StemsGOs)
        {
            StemFE stemFE = stemGO.GetComponent<StemFE>();
            if (!stemFE.Stem.IsEmpty)
            {
                WidenStem(stemFE, growthAmt);
            }
            else
            {
                empties.Add(stemGO);
            }
        }
        // Move the leaves and flowers so they look like they're staying with the stem as it grows
        AdjustLeafAndFlowerPositions(empties);
    }

    private void WidenStem(StemFE stemFE, float growthAmt)
    {
        Stem stem = stemFE.Stem;
        stemFE.SetScaleGlobal(new Vector3(stem.GlobalScale.x + growthAmt, stem.GlobalScale.y, stem.GlobalScale.z + growthAmt));
    }

    private GameObject GetSiblingOfEmpty(GameObject emptyGO)
    {
        string emptyName = emptyGO.name.Substring(2); // Strip the "E " off the front
        return GameObject.Find(emptyName);
    }

    /// <summary>
    /// Adds new stem(s) to the plant according to the branchPattern
    /// </summary>
    private IEnumerator GrowPlant()
    {
        yield return new WaitForSeconds(Plant.TimeUntilGrowth); // For when the game is loaded. 
        for (; ; )
        {
            if (StemsGOs.Count < Plant.Taxonomy.Species.Genus.PlantMaxSize)
            {
                GrowNext();
            }
            else
            {
                Plant.IsMaxSize = true;
                Debug.Log(Plant.PlantID + " reached max size.");
                // PERFORMANCE: Make it so it doesn't keep checking this
            }

            foreach (GameObject stemGO in GrowingStemGOs)
            { // PERFORMANCE: Can I add these while iterating for GrowNext?
                Plant.GrowingStems.Add(stemGO.GetComponent<StemFE>().Stem);
            }

            // How it was 11/24/20
            //// This will need to change once I change to real growth rate
            float timeUntilGrowth; // In minutes but Time.deltaTime in seconds
            for (timeUntilGrowth = Plant.Taxonomy.Species.Genus.TimeBetweenPlantGrowth; timeUntilGrowth > 0; timeUntilGrowth -= Time.deltaTime)
            {
                // Gets "stuck" in here until timeUntilGrowth reaches 0 then leaves for loop and grows again
                Plant.TimeUntilGrowth = timeUntilGrowth;
                yield return null;
            }

            //float startTime = GlobalControl.Instance.savedValues.GameTime;
            //while (GameTime.GetMinutesElapsed(startTime, GlobalControl.Instance.savedValues.GameTime) < Plant.Taxonomy.Species.Genus.PlantGrowthRate)
            //{
            //    // Need to save something in Plant.TimeUntilGrowth
            //    yield return null;
            //}
        }
    }

    // This will need to be in Plant and reference the back end stuff. Have a method in here that calls on the Plant methods.
    // The Plant methods will be the ones called in GrowPlantsInOtherScenes and the Game Objects will be created when the scene is loaded
    private void GrowNext()
    {
        Plant.GrowingStems.Clear();
        List<GameObject> temp = new List<GameObject>();
        foreach (GameObject stemGO in GrowingStemGOs)
        {
            Branch branch = stemGO.GetComponent<StemFE>().Stem.Branch;
            if (branch.CurrStep != Plant.Taxonomy.Species.Internodes)
            {   // When not branching. Create an empty GO to act as the parent to avoid scale skewing
                GameObject emptyGO = null;
                if (branch.CurrStep == 0)
                {
                    emptyGO = CreateEmptyStem(stemGO);
                }
                GrowNormal(stemGO, emptyGO, branch, temp);
            }
            else
            {
                // Create the branch(s) and the trunk/main branch
                DoBranchingPattern(branch, Plant.Taxonomy.Species.Genus.Family.BranchPattern, stemGO, temp);
            }
        }
        GrowingStemGOs.Clear();
        GrowingStemGOs.AddRange(temp);
    }

    private void DoBranchingPattern(Branch branch, BranchPatterns branchPattern, GameObject stemGO, List<GameObject> temp)
    {
        switch (branchPattern)
        {
            case BranchPatterns.Alternate:
                AlternateBranch(branch, stemGO, temp);
                break;
            case BranchPatterns.Opposite:
                OppositeBranch(branch, stemGO, temp);
                break;
            case BranchPatterns.Whorled:
                WhorledBranch(branch, stemGO, temp);
                break;
            default:
                Debug.LogError("Missing BranchPattern.");
                break;
        }
    }

    public GameObject CreateEmptyStem(GameObject stemGO)
    {
        GameObject emptyGO = new GameObject();
        StemFE emptyFE = emptyGO.AddComponent<StemFE>();
        emptyFE.Stem = stemGO.GetComponent<StemFE>().Stem.Clone();
        emptyFE.Stem.IsEmpty = true;
        emptyFE.SetParent(stemGO.transform.parent);
        emptyFE.SetPosition(stemGO.transform.localPosition);
        emptyFE.SetScaleGlobal(Plant.Taxonomy.Species.StemInitialSize.Value);
        emptyFE.SetRotation(stemGO.transform.localRotation);
        emptyFE.SetName("E " + stemGO.name);
        Plant.Stems[stemGO.GetComponent<StemFE>().Stem.BranchNum].Add(emptyGO.GetComponent<StemFE>().Stem);
        StemsGOs.Add(emptyGO); // FIXME: Should I add the empty? Need to adjust leaf positions
        return emptyGO;
    }

    private void AlternateBranch(Branch branch, GameObject stemGO, List<GameObject> temp)
    {
        GrowBranch(stemGO, branch, temp);
        branch.IncrementBranchSide(360 / Plant.Taxonomy.Species.BranchesPerCycle);
        GrowContinue(stemGO, branch, temp);
    }

    private void OppositeBranch(Branch branch, GameObject stemGO, List<GameObject> temp)
    {
        GrowBranch(stemGO, branch, temp);
        branch.IncrementBranchSide(180);
        GrowBranch(stemGO, branch, temp);
        branch.IncrementBranchSide(360 / Plant.Taxonomy.Species.BranchesPerCycle);
        GrowContinue(stemGO, branch, temp);
    }

    private void WhorledBranch(Branch branch, GameObject stemGO, List<GameObject> temp)
    {
        for (int b = 0; b < Plant.Taxonomy.Species.BranchesPerCycle; b++)
        {
            GrowBranch(stemGO, branch, temp);
            branch.IncrementBranchSide(360 / Plant.Taxonomy.Species.BranchesPerCycle);

        }
        GrowContinue(stemGO, branch, temp);
    }

    private GameObject GrowNormal(GameObject stemGO, GameObject emptyGO, Branch branch, List<GameObject> growingTemp)
    {
        int branchNum = stemGO.GetComponent<StemFE>().Stem.BranchNum;
        int currStep = branch.CurrStep + 1;//seems like this will never be 0 

        GameObject newStemGO = InstantiateStem(Plant.CreateStem(branchNum, currStep, branch, false), branchNum, currStep, branch, false);

        // Set the parent. If it's the first comp in the branch (besides the branchBase), set it to the emptyGO sibling of branchBase
        if (currStep == 1)
        {
            newStemGO.GetComponent<StemFE>().SetParent(emptyGO.transform);
        }
        else
        {
            newStemGO.GetComponent<StemFE>().SetParent(stemGO.transform.parent);
        }

        // Set the rotation
        newStemGO.GetComponent<StemFE>().SetRotation(Vector3.zero);

        // Set the position
        if (HitBarrier(stemGO, newStemGO))
        {
            Debug.Log(stemGO.name + " stopped growing in normal");
            StemsGOs.Remove(newStemGO);
            Destroy(newStemGO);

            return null;
        }
        else
        {
            newStemGO.GetComponent<StemFE>().SetPosition(new Vector3(0, currStep * 2, 0));
            growingTemp.Add(newStemGO);
        }

        // Set the scale. Needs to be globally so it comes in at the original scale
        newStemGO.GetComponent<StemFE>().SetScaleGlobal(Plant.Taxonomy.Species.StemInitialSize.Value);

        newStemGO.GetComponent<StemFE>().AddLeaves(this, LeafGOs);
        //newStemGO.GetComponent<StemFE>().AddFlowers(this, ThisYearsFlowerGOs, Plant.ThisYearsFlowers);
        newStemGO.GetComponent<StemFE>().AddFlowers(this, FlowerGOs, Plant.Flowers);

        return newStemGO;
    }

    private GameObject GrowBranch(GameObject stemGO, Branch branch, List<GameObject> growingTemp)
    {
        // Start the new branch
        Plant.Stems.Add(new List<Stem>());
        int branchNum = Plant.Stems.Count - 1;

        GameObject newStemGO = InstantiateStem(Plant.CreateStem(branchNum, 0, branch, true), branchNum, 0, branch, true);

        // Set the parent to stemGO's parent
        newStemGO.GetComponent<StemFE>().SetParent(stemGO.transform.parent);

        // Set the rotation. This sets the branch's direction. Random every time
        Vector3Serializable rotation = branch.GetRotationFromSide(Plant.Taxonomy.Species.GetVariedBranchRotation(false));
        newStemGO.GetComponent<StemFE>().SetRotation(rotation.ToVector3());

        // Set the position
        if (HitBarrier(stemGO, newStemGO))
        {
            Debug.Log(stemGO.name + " stopped growing in branch");
            StemsGOs.Remove(newStemGO);
            Destroy(newStemGO);

            return null;
        }
        else
        {
            newStemGO.GetComponent<StemFE>().SetPosition(branch.GetPositionFromSide(newStemGO.GetComponent<StemFE>().Stem).ToVector3());
            growingTemp.Add(newStemGO);
        }

        // Set the scale. Needs to be globally so it comes in at the original scale (not grown during game time)
        newStemGO.GetComponent<StemFE>().SetScaleGlobal(Plant.Taxonomy.Species.StemInitialSize.Value);

        newStemGO.GetComponent<StemFE>().AddLeaves(this, LeafGOs);
        //newStemGO.GetComponent<StemFE>().AddFlowers(this, ThisYearsFlowerGOs, Plant.ThisYearsFlowers);
        newStemGO.GetComponent<StemFE>().AddFlowers(this, FlowerGOs, Plant.Flowers);
        return newStemGO;
    }

    // Continues the current pattern after creating a branch. This is like a new branch but it continues the main trunk/branch
    private GameObject GrowContinue(GameObject stemGO, Branch branch, List<GameObject> growingTemp)
    {
        Plant.Stems.Add(new List<Stem>());
        int branchNum = Plant.Stems.Count - 1;
        branch.CurrStep++;

        GameObject newStemGO = InstantiateStem(Plant.CreateStem(branchNum, 0, branch, true), branchNum, 0, branch, true);

        // Set the parent
        newStemGO.GetComponent<StemFE>().SetParent(stemGO.transform.parent);

        // Set the rotation. This sets the main branch/trunk's direction. Getting it directly from taxonomy so it's random everytime
        newStemGO.GetComponent<StemFE>().SetRotation(Plant.Taxonomy.Species.GetVariedTrunkRotation(true).ToVector3());

        // Set the position
        if (HitBarrier(stemGO, newStemGO))
        {
            Debug.Log(stemGO.name + " stopped growing in continue");
            StemsGOs.Remove(newStemGO);
            Destroy(newStemGO);

            return null;
        }
        else
        {
            newStemGO.GetComponent<StemFE>().SetPosition(branch.GetPositionFromSide(newStemGO.GetComponent<StemFE>().Stem).ToVector3());
            growingTemp.Add(newStemGO);
        }

        // Set the scale. Needs to be globally so it comes in at the original scale (not grown during game time)
        newStemGO.GetComponent<StemFE>().SetScaleGlobal(Plant.Taxonomy.Species.StemInitialSize.Value);

        newStemGO.GetComponent<StemFE>().AddLeaves(this, LeafGOs);
        //newStemGO.GetComponent<StemFE>().AddFlowers(this, ThisYearsFlowerGOs, Plant.ThisYearsFlowers);
        newStemGO.GetComponent<StemFE>().AddFlowers(this, FlowerGOs, Plant.Flowers);
        return newStemGO;
    }

    // When things are moved to Plant and plants are growing in other scenes this might have to be checked when the game is loaded
    // and parts removed then
    private bool HitBarrier(GameObject stemGO, GameObject newStemGO)
    {
        // FIXME: Might need to increase the distance that it checks
        float distance = stemGO.transform.lossyScale.magnitude + BarrierBuffer;
        if (Physics.Raycast(stemGO.transform.position, stemGO.transform.TransformDirection(stemGO.transform.forward), out _, distance, ConstantValues.LayerMasks.Barrier))
        {
            Debug.DrawRay(stemGO.transform.position, distance * stemGO.transform.TransformDirection(stemGO.transform.forward).normalized, Color.red, 5f);
            //StemsGOs.Remove(newStemGO);
            //Destroy(newStemGO);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a GameObject from stem prefab and assigns newStem to its stem slot.
    /// </summary>
    /// <param name="newStem"></param>
    /// <returns></returns>
    private GameObject InstantiateStem(Stem newStem, int branchNum, int currStep, Branch branch, bool isBranchBase)
    {
        newStem.BranchNum = branchNum;
        newStem.Branch = new Branch(currStep, branch)
        {
            IsBranchBase = isBranchBase,
            YearGrown = GlobalControl.Instance.savedValues.Year
        };
        GameObject newStemGO = Instantiate(Resources.Load(ConstantValues.Prefabs.Stem)) as GameObject;
        newStemGO.GetComponent<StemFE>().Stem = newStem;
        newStemGO.name = newStem.StemID;
        newStemGO.GetComponent<MeshRenderer>().enabled = FirstStemGO.GetComponent<MeshRenderer>().enabled; // For when the plant grows while being held
        StemsGOs.Add(newStemGO);
        return newStemGO;
    }
}
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlantFE : MonoBehaviour
//{

//    [SerializeField] private Plant plant = null;
//    [SerializeField] private GameObject firstStem = null;
//    [SerializeField] private List<GameObject> lastYearsFlowerGOs = new List<GameObject>();
//    [SerializeField] private List<GameObject> thisYearsFlowerGOs = new List<GameObject>();

//    private static float BarrierBuffer { get; } = 0.5f; // How far from the wall plants can grow. The stem global magnitude is added to this too

//    // Plant/stems
//    public Plant Plant { get { return plant; } set { plant = value; } }
//    public GameObject FirstStemGO { get { return firstStem; } set { firstStem = value; } }
//    // This is to iterate over for widening stems. Filled in PlantUtilities when the plant is loaded and added to when a stem is grown. 
//    // Empties won't be added when created or loaded. 
//    public List<GameObject> StemsGOs { get; set; } = new List<GameObject>();
//    // This is filled in PlantUtilities when the plant is loaded and added to when a leaf is grown
//    private List<GameObject> GrowingStemGOs { get; set; } = new List<GameObject>(); // PERFORMANCE: Can this be filled when the plant is loaded?
//    private bool IsGrowing { get; set; } = false;

//    // Leaves
//    public List<GameObject> LeafGOs { get; } = new List<GameObject>();
//    private float LeafDroppingRate { get; set; } = 1f;
//    private bool IsLeafDropping { get; set; } = false;

//    // Flowers
//    public List<GameObject> LastYearsFlowerGOs { get { return lastYearsFlowerGOs; } }
//    public List<GameObject> ThisYearsFlowerGOs { get { return thisYearsFlowerGOs; } }
//    private bool IsFlowering { get; set; } = false;
//    private float FlowerDroppingRate { get; set; } = 1f;
//    private bool IsFlowerDropping { get; set; } = false;
//    private bool resetFlowerGOs = false;

//    private Coroutine growPlant;
//    private Coroutine growLeaves;
//    private Coroutine growFlowers;
//    private Coroutine widenStems = null;

//    public void Start()
//    {
//        PotFrontEnd.ApplyColor(gameObject, Plant.Pot);
//        PotFrontEnd.ApplySize(gameObject, Plant.Pot);
//        InitializeGrowingStemGOs(); // FIXME: do I need GrowingStemGOs anymore?
//        IsLeafDropping = false;
//        IsGrowing = false;
//    }

//    private void ResetFlowers()
//    {
//        if ((Seasons)GlobalControl.Instance.savedValues.Season == Seasons.Spring && !resetFlowerGOs)
//        {
//            resetFlowerGOs = true;
//            LastYearsFlowerGOs.Clear();
//            LastYearsFlowerGOs.AddRange(ThisYearsFlowerGOs);
//            ThisYearsFlowerGOs.Clear();
//            Plant.LastYearsFlowers.Clear();
//            Plant.LastYearsFlowers.AddRange(Plant.ThisYearsFlowers);
//            Plant.ThisYearsFlowers.Clear();
//        }
//        else if ((Seasons)GlobalControl.Instance.savedValues.Season == Seasons.Summer)
//        {
//            resetFlowerGOs = false;
//        }
//    }

//    public void Update()
//    {
//        if (Plant.Taxonomy.Species != null) // might be able to get rid of .Species here
//        {
//            ResetFlowers();
//            if ((Seasons)GlobalControl.Instance.savedValues.Season == Seasons.Fall) // UNDONE: this needs to be set based on the plant
//            {
//                if (!IsLeafDropping)
//                {
//                    Debug.Log("In leaf dropping season.");
//                    IsLeafDropping = true;
//                    StartCoroutine(DropLeaves());
//                }
//            }
//            if (IsFloweringSeason())
//            {
//                if (!IsFlowering && FirstStemGO) // Check if the plant is ready to start growing (it won't be if still being planted for example)
//                {
//                    Debug.Log("In flowering season.");
//                    IsFlowering = true;
//                    if (Plant.Taxonomy.Species.OnNewGrowth)
//                    {
//                        growFlowers = StartCoroutine(GrowFlowers(ThisYearsFlowerGOs));
//                    }
//                    else
//                    {
//                        growFlowers = StartCoroutine(GrowFlowers(LastYearsFlowerGOs));
//                    }
//                }
//            }
//            else
//            {
//                if (IsFlowering)
//                {
//                    Debug.Log("Not in flowering season");
//                    IsFlowering = false;
//                    StopCoroutine(growFlowers);
//                }

//                if (!IsFlowerDropping)
//                {
//                    IsFlowerDropping = true;
//                    if (Plant.Taxonomy.Species.OnNewGrowth)
//                    {
//                        StartCoroutine(DropFlowers(ThisYearsFlowerGOs, Plant.ThisYearsFlowers));
//                    }
//                    else
//                    {
//                        StartCoroutine(DropFlowers(LastYearsFlowerGOs, Plant.LastYearsFlowers));
//                    }
//                }
//            }

//            if (IsGrowingSeason())
//            {
//                if (!IsGrowing && FirstStemGO) // Check if the plant is ready to start growing (it won't be if still being planted for example)
//                {
//                    Debug.Log("In season");
//                    IsGrowing = true;
//                    AddLastYearsLeaves();
//                    growPlant = StartCoroutine(GrowPlant());
//                    growLeaves = StartCoroutine(GrowLeaves());
//                    widenStems = StartCoroutine(WidenStems());
//                }
//            }

//            if (widenStems != null)
//            {
//                if (FirstStemGO.transform.lossyScale.x >= Plant.Taxonomy.Species.StemMaxSize)
//                {
//                    StopCoroutine(widenStems);
//                    Plant.IsMaxCaliper = true;
//                    Debug.Log(Plant.PlantID + " reached max caliper.");
//                }
//                if (FirstStemGO.transform.lossyScale.x >= Plant.Pot.PotSize.x || FirstStemGO.transform.lossyScale.x >= Plant.Pot.PotSize.z)
//                { // TODO: Add in a buffer so the plant doesn't grow out of the pot
//                    StopCoroutine(widenStems);
//                    Debug.Log(Plant.PlantID + " needs larger pot.");
//                }
//            }

//            if (!IsGrowingSeason())
//            {
//                if (IsGrowing)
//                {
//                    Debug.Log("Not in season");
//                    IsGrowing = false;
//                    StopCoroutine(growPlant);
//                    StopCoroutine(growLeaves);
//                    StopCoroutine(widenStems);
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// For use with the UI button in the plant menu
//    /// </summary>
//    public void Repot()
//    {
//        Debug.LogWarning("Not yet implemented");
//        //Worktable.SelectedPlantGO = gameObject;
//        //SceneLoader.LoadSceneStatic(ConstantValues.Scenes.Worktable);
//    }

//    /// <summary>
//    /// For use with the UI button in the plant menu
//    /// </summary>
//    public void Pollinate()
//    {
//        //Plant pollen = Player.PlayerInstance.InventoryManager.SelectPollen();
//        StartCoroutine(Player.PlayerInstance.InventoryManager.SelectPollen(this));

//    }

//    /// <summary>
//    /// For use with the UI button in the plant menu
//    /// </summary>
//    public void CollectPollen()
//    {
//        Debug.Log("Collected pollen");
//        // Plant.RemovePollen();
//        Player.PlayerInstance.InventoryManager.Inventory.InventoryPollen.Add(Plant);
//    }

//    public GameObject GetBenchSegment(int segmentNum)
//    {
//        return GetComponentInParent<GreenhouseBench>().BenchSegments[segmentNum];
//    }

//    public Plant[] GetSaveBench()
//    {
//        return GetSaveList()[GetComponentInParent<GreenhouseBench>().BenchNum];
//    }

//    private List<Plant[]> GetSaveList()
//    {
//        return GetComponentInParent<GreenhouseBench>().saveList;
//    }

//    public decimal GetPercentMaxSize()
//    {
//        return (decimal)StemsGOs.Count / Plant.Taxonomy.Species.Genus.PlantMaxSize;
//    }

//    public decimal GetPercentMaxCaliper()
//    {
//        return (decimal)((Plant.Stems[0][0].GlobalScale.x - Plant.Taxonomy.Species.StemInitialSize.x) / Plant.Taxonomy.Species.StemMaxSize);
//    }

//    private void AddLastYearsLeaves()
//    {
//        foreach (Leaf leaf in Plant.Leaves)
//        {
//            if (leaf.IsDropped)
//            {
//                PlantUtilities.LoadDroppedLeaf(leaf, gameObject);
//                leaf.IsDropped = false;
//            }
//        }
//    }

//    /// <summary>
//    /// Removes a random flower from the plant so it can fall and waits up to a second before dropping another
//    /// </summary>
//    /// <returns></returns>
//    private IEnumerator DropFlowers(List<GameObject> flowerGOs, List<Flower> flowers)
//    {
//        for (; ; )
//        {
//            if (flowerGOs.Count > 0)
//            {
//                int rand = Random.Range(0, flowerGOs.Count);
//                GameObject randFlower = flowerGOs[rand];
//                flowerGOs.RemoveAt(rand);
//                flowers.RemoveAt(rand);

//                randFlower.GetComponent<FlowerFE>().DropFlower();

//                FlowerDroppingRate = Random.Range(0.5f, 2f);
//                yield return new WaitForSeconds(FlowerDroppingRate);
//            }
//            else
//            { // Should be done growing flowers here
//                IsFlowerDropping = false;
//                yield break;
//            }
//        }
//    }

//    /// <summary>
//    /// Removes a random leaf from the plant so it can fall and waits up to a second before dropping another
//    /// </summary>
//    /// <returns></returns>
//    private IEnumerator DropLeaves()
//    {
//        for (; ; )
//        {
//            if (LeafGOs.Count > 0)
//            {
//                int rand = Random.Range(0, LeafGOs.Count);
//                GameObject randLeaf = LeafGOs[rand];
//                LeafGOs.RemoveAt(rand);
//                if (randLeaf.transform.parent.GetComponent<StemFE>().Stem.Branch.YearGrown != GlobalControl.Instance.savedValues.Year)
//                {
//                    Plant.Leaves.RemoveAt(rand);
//                }
//                randLeaf.GetComponent<LeafFE>().DropLeaf();

//                //leafDroppingRate = GameTime.timeScale > 0 ? UnityEngine.Random.Range(0.5f, 2f) * GameTime.timeScale : UnityEngine.Random.Range(0.5f, 2f); // TODO could make this scale with how late into fall it is
//                LeafDroppingRate = Random.Range(0.5f, 2f);
//                yield return new WaitForSeconds(LeafDroppingRate);
//            }
//            else
//            { // Should be done growing leaves here
//                IsLeafDropping = false;
//                yield break;
//            }
//        }
//    }

//    private bool IsGrowingSeason()
//    {
//        bool isGrowingSeason = false;
//        foreach (Seasons season in Plant.Taxonomy.Species.GrowingSeasons)
//        {
//            if ((Seasons)GlobalControl.Instance.savedValues.Season == season)
//            {
//                isGrowingSeason = true;
//            }
//        }
//        return isGrowingSeason;
//    }

//    private bool IsFloweringSeason()
//    {
//        bool isFloweringSeason = false;
//        foreach (Seasons season in Plant.Taxonomy.Species.FloweringSeasons)
//        {
//            if ((Seasons)GlobalControl.Instance.savedValues.Season == season)
//            {
//                isFloweringSeason = true;
//            }
//        }
//        return isFloweringSeason;
//    }

//    /// <summary>
//    /// Initiate growingStemGOs so the plant can grow
//    /// </summary>
//    private void InitializeGrowingStemGOs()
//    {
//        GrowingStemGOs.Clear();
//        //foreach (string stem in Plant.GrowingStems)
//        foreach (Stem stem in Plant.GrowingStems)
//        {
//            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>(true))
//            {
//                if (t.gameObject.name == stem.StemID)
//                {
//                    GrowingStemGOs.Add(t.gameObject);
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// Grows the existing flowers based on the growthRate
//    /// </summary>
//    private IEnumerator GrowFlowers(List<GameObject> flowerGOs)
//    {
//        yield return new WaitForSeconds(GlobalControl.Instance.savedValues.TimeToFlowerGrowth); // For when the game is loaded. 
//        for (; ; )
//        {
//            foreach (GameObject flowerGO in flowerGOs)
//            {
//                GrowFlower(flowerGO);
//            }
//            float timeToFlowerGrowth;
//            for (timeToFlowerGrowth = Plant.Taxonomy.Species.Genus.FlowerGrowthRate; timeToFlowerGrowth > 0; timeToFlowerGrowth -= Time.deltaTime)
//            {
//                GlobalControl.Instance.savedValues.TimeToFlowerGrowth = timeToFlowerGrowth;
//                yield return null;
//            }
//        }
//    }

//    private void GrowFlower(GameObject flowerGO)
//    {
//        if (flowerGO.transform.localScale.x + ConstantValues.PlantConsts.GrowthAmt < Plant.Taxonomy.Species.Genus.FlowerMaxSize)
//        {
//            GrowFlowerHelper(flowerGO, ConstantValues.PlantConsts.GrowthAmt);
//        }
//        else // Add the extra on to get it to exactly the max size
//        {
//            GrowFlowerHelper(flowerGO, Plant.Taxonomy.Species.Genus.FlowerMaxSize - flowerGO.transform.localScale.x);
//        }
//    }

//    private void GrowFlowerHelper(GameObject flowerGO, float growthAmt)
//    {
//        Vector3 initialScale = flowerGO.transform.localScale;

//        flowerGO.GetComponent<FlowerFE>().SetScale(flowerGO.transform.localScale +
//            growthAmt * ConstantValues.PlantConsts.FlowerPrefabScale.ToVector3() /
//            ConstantValues.PlantConsts.FlowerPrefabScale.x); // divided by x to keep growthAmt.x = ConstantValues.PlantConsts.GrowthAmt

//        Vector3 finalScale = flowerGO.transform.localScale;

//        // Move the flowers so they are still attached to the plant 
//        Vector3 sign = flowerGO.GetComponent<FlowerFE>().Flower.Position.ToVector3().normalized;
//        Vector3 deltaScale = finalScale - initialScale;

//        // Only using the x comp of the deltaScale since that's the only length that matters with current flower model
//        flowerGO.GetComponent<FlowerFE>().SetPosition(flowerGO.transform.localPosition + sign * (deltaScale.x / 2));
//    }

//    /// <summary>
//    /// Grows the existing leaves based on the growthRate
//    /// </summary>
//    private IEnumerator GrowLeaves()
//    {
//        yield return new WaitForSeconds(GlobalControl.Instance.savedValues.TimeToLeafGrowth); // For when the game is loaded. 
//        for (; ; )
//        {
//            foreach (GameObject leafGO in LeafGOs)
//            {
//                GrowLeaf(leafGO);
//            }
//            float timeToLeafGrowth;
//            for (timeToLeafGrowth = Plant.Taxonomy.Species.Genus.LeafGrowthRate; timeToLeafGrowth > 0; timeToLeafGrowth -= Time.deltaTime)
//            {
//                GlobalControl.Instance.savedValues.TimeToLeafGrowth = timeToLeafGrowth;
//                yield return null;
//            }
//        }
//    }

//    private void GrowLeaf(GameObject leafGO)
//    {
//        if (leafGO.transform.localScale.x + ConstantValues.PlantConsts.GrowthAmt < Plant.Taxonomy.Species.Genus.LeafMaxSize)
//        {
//            GrowLeafHelper(leafGO, ConstantValues.PlantConsts.GrowthAmt);
//        }
//        else // Add the extra on to get it to exactly the max size
//        {
//            GrowLeafHelper(leafGO, Plant.Taxonomy.Species.Genus.LeafMaxSize - leafGO.transform.localScale.x);
//        }
//    }

//    private void GrowLeafHelper(GameObject leafGO, float growthAmt)
//    {
//        Vector3 initialScale = leafGO.transform.localScale;

//        leafGO.GetComponent<LeafFE>().SetScale(leafGO.transform.localScale +
//            growthAmt * ConstantValues.PlantConsts.LeafPrefabScale.ToVector3() /
//            ConstantValues.PlantConsts.LeafPrefabScale.x); // divided by x to keep growthAmt.x = ConstantValues.PlantConsts.GrowthAmt

//        Vector3 finalScale = leafGO.transform.localScale;

//        // Move the leaves so they are still attached to the plant
//        Vector3 sign = leafGO.GetComponent<LeafFE>().Leaf.position.ToVector3().normalized;
//        Vector3 deltaScale = finalScale - initialScale;

//        // Only using the x comp of the deltaScale since that's the only length that matters with current leaf model
//        leafGO.GetComponent<LeafFE>().SetPosition(leafGO.transform.localPosition + sign * (deltaScale.x / 2));
//    }

//    /// <summary>
//    /// Widens the existing stems based on their growthRate
//    /// </summary>
//    private IEnumerator WidenStems()
//    {
//        float stemGrowthAmt = Plant.Taxonomy.Species.StemGrowthAmt;
//        float firstStemGlobalScale = FirstStemGO.GetComponent<StemFE>().Stem.GlobalScale.x;

//        yield return new WaitForSeconds(Plant.TimeUntilStemGrowth); // For when the game is loaded. 
//        for (; ; )
//        {
//            if (firstStemGlobalScale + stemGrowthAmt < Plant.Taxonomy.Species.StemMaxSize)
//            {
//                if (firstStemGlobalScale + stemGrowthAmt < Plant.Pot.PotSize.x &&
//                    firstStemGlobalScale + stemGrowthAmt < Plant.Pot.PotSize.z)
//                {
//                    WidenAllStems(stemGrowthAmt);
//                }
//                else // Add the extra on to get it to exactly the max size
//                {
//                    Debug.Log("widening for pot size: " + (Plant.Pot.PotSize.x - firstStemGlobalScale));
//                    WidenAllStems(Plant.Pot.PotSize.x - firstStemGlobalScale); // TODO: do a separate one for potSize.z
//                }
//            }
//            else // Add the extra on to get it to exactly the max size
//            {
//                Debug.Log("widening for stem size: " + (Plant.Taxonomy.Species.StemMaxSize - firstStemGlobalScale));
//                WidenAllStems(Plant.Taxonomy.Species.StemMaxSize - firstStemGlobalScale);
//            }
//            float timeUntilStemGrowth;
//            for (timeUntilStemGrowth = Plant.Taxonomy.Species.StemGrowthRate; timeUntilStemGrowth > 0; timeUntilStemGrowth -= Time.deltaTime)
//            {
//                Plant.TimeUntilStemGrowth = timeUntilStemGrowth;
//                yield return null;
//            }
//        }
//    }

//    private void AdjustLeafAndFlowerPositions(List<GameObject> empties)
//    {
//        foreach (GameObject empty in empties)
//        {
//            foreach (Transform child in empty.transform)
//            {
//                if (child.GetComponent<LeafFE>() || child.GetComponent<FlowerFE>())
//                {
//                    float growthAmt = ConstantValues.PlantConsts.StemGrowthPercent / 2; // Divide by 2 because it's growing evenly on both sides
//                    Vector3 moveAmt = new Vector3(
//                        growthAmt * child.localPosition.normalized.x,
//                        growthAmt * child.localPosition.normalized.y,
//                        growthAmt * child.localPosition.normalized.z);
//                    if (child.GetComponent<LeafFE>())
//                    {
//                        child.GetComponent<LeafFE>().SetPosition(child.localPosition + moveAmt);
//                    }
//                    else
//                    {
//                        child.GetComponent<FlowerFE>().SetPosition(child.localPosition + moveAmt);
//                    }
//                }
//            }
//        }
//    }

//    private void WidenAllStems(float growthAmt)
//    {
//        List<GameObject> empties = new List<GameObject>();
//        foreach (GameObject stemGO in StemsGOs)
//        {
//            StemFE stemFE = stemGO.GetComponent<StemFE>();
//            if (!stemFE.Stem.IsEmpty)
//            {
//                WidenStem(stemFE, growthAmt);
//            }
//            else
//            {
//                empties.Add(stemGO);
//            }
//        }
//        // Move the leaves and flowers so they look like they're staying with the stem as it grows
//        AdjustLeafAndFlowerPositions(empties);
//    }

//    private void WidenStem(StemFE stemFE, float growthAmt)
//    {
//        Stem stem = stemFE.Stem;
//        stemFE.SetScaleGlobal(new Vector3(stem.GlobalScale.x + growthAmt, stem.GlobalScale.y, stem.GlobalScale.z + growthAmt));
//    }

//    private GameObject GetSiblingOfEmpty(GameObject emptyGO)
//    {
//        string emptyName = emptyGO.name.Substring(2); // Strip the "E " off the front
//        return GameObject.Find(emptyName);
//    }

//    /// <summary>
//    /// Adds new stem(s) to the plant according to the branchPattern
//    /// </summary>
//    private IEnumerator GrowPlant()
//    {
//        yield return new WaitForSeconds(Plant.TimeUntilGrowth); // For when the game is loaded. 
//        for (; ; )
//        {
//            List<Stem> newStems = new List<Stem>();
//            if (StemsGOs.Count < Plant.Taxonomy.Species.Genus.PlantMaxSize)
//            {
//                newStems = Plant.GrowAllBranches();
//            }
//            else
//            {
//                Plant.IsMaxSize = true;
//                Debug.Log(Plant.PlantID + " reached max size.");
//            }

//            //foreach (GameObject stemGO in GrowingStemGOs)
//            //{ // PERFORMANCE: Can I add these while iterating for GrowNext?
//            //    Plant.GrowingStems.Add(stemGO.GetComponent<Stem>());
//            //}

//            foreach (Stem stem in Plant.GrowingStems)
//            { // PERFORMANCE: Can I add these while iterating for GrowNext?
//                InstantiateStem(stem);
//            }

//            float timeUntilGrowth;
//            for (timeUntilGrowth = Plant.Taxonomy.Species.Genus.PlantGrowthRate; timeUntilGrowth > 0; timeUntilGrowth -= Time.deltaTime)
//            {
//                Plant.TimeUntilGrowth = timeUntilGrowth;
//                yield return null;
//            }
//            GlobalControl.Instance.Save();
//        }
//    }

//    //// This will need to be in Plant and reference the back end stuff. Have a method in here that calls on the Plant methods.
//    //// The Plant methods will be the ones called in GrowPlantsInOtherScenes and the Game Objects will be created when the scene is loaded
//    //private void GrowNext()
//    //{
//    //    Plant.GrowingStems.Clear();
//    //    List<GameObject> temp = new List<GameObject>();
//    //    foreach (GameObject stemGO in GrowingStemGOs)
//    //    {
//    //        Branch branch = stemGO.GetComponent<StemFE>().Stem.Branch;
//    //        if (branch.CurrStep != Plant.Taxonomy.Species.Genus.Internodes)
//    //        {   // When not branching. Create an empty GO to act as the parent to avoid scale skewing
//    //            GameObject emptyGO = null;
//    //            if (branch.CurrStep == 0)
//    //            {
//    //                emptyGO = CreateEmptyStem(stemGO);
//    //            }
//    //            GrowNormal(stemGO, emptyGO, branch, temp);
//    //        }
//    //        else
//    //        {
//    //            // Create the branch(s) and the trunk/main branch
//    //            DoBranchingPattern(branch, Plant.Taxonomy.Species.Genus.Family.BranchPattern, stemGO, temp);
//    //        }
//    //    }
//    //    GrowingStemGOs.Clear();
//    //    GrowingStemGOs.AddRange(temp);
//    //}

//    //private void DoBranchingPattern(Branch branch, BranchPattern branchPattern, GameObject stemGO, List<GameObject> temp)
//    //{
//    //    switch (branchPattern)
//    //    {
//    //        case BranchPattern.Alternate:
//    //            AlternateBranch(branch, stemGO, temp);
//    //            break;
//    //        case BranchPattern.Opposite:
//    //            OppositeBranch(branch, stemGO, temp);
//    //            break;
//    //        case BranchPattern.Whorled:
//    //            WhorledBranch(branch, stemGO, temp);
//    //            break;
//    //        default:
//    //            Debug.LogError("Missing BranchPattern.");
//    //            break;
//    //    }
//    //}

//    //public GameObject CreateEmptyStem(GameObject stemGO)
//    //{
//    //    GameObject emptyGO = new GameObject();
//    //    StemFE emptyFE = emptyGO.AddComponent<StemFE>();
//    //    emptyFE.Stem = stemGO.GetComponent<StemFE>().Stem.Clone();
//    //    emptyFE.Stem.IsEmpty = true;
//    //    emptyFE.SetParent(stemGO.transform.parent);
//    //    emptyFE.SetPosition(stemGO.transform.localPosition);
//    //    emptyFE.SetScaleGlobal(Plant.Taxonomy.Species.StemInitialSize.ToVector3());
//    //    emptyFE.SetRotation(stemGO.transform.localRotation);
//    //    emptyFE.SetName("E " + stemGO.name);
//    //    Plant.Stems[stemGO.GetComponent<StemFE>().Stem.BranchNum].Add(emptyGO.GetComponent<StemFE>().Stem);
//    //    StemsGOs.Add(emptyGO); // FIXME: Should I add the empty? Need to adjust leaf positions
//    //    return emptyGO;
//    //}

//    //private void AlternateBranch(Branch branch, GameObject stemGO, List<GameObject> temp)
//    //{
//    //    GrowBranch(stemGO, branch, temp);
//    //    branch.IncrementBranchSide();
//    //    GrowContinue(stemGO, branch, temp);
//    //}

//    //private void OppositeBranch(Branch branch, GameObject stemGO, List<GameObject> temp)
//    //{
//    //    GrowBranch(stemGO, branch, temp);
//    //    branch.IncrementBranchSide();
//    //    branch.IncrementBranchSide();
//    //    GrowBranch(stemGO, branch, temp);
//    //    branch.IncrementBranchSide();
//    //    GrowContinue(stemGO, branch, temp);
//    //}

//    //private void WhorledBranch(Branch branch, GameObject stemGO, List<GameObject> temp)
//    //{
//    //    GrowBranch(stemGO, branch, temp);
//    //    branch.IncrementBranchSide();
//    //    GrowBranch(stemGO, branch, temp);
//    //    branch.IncrementBranchSide();
//    //    GrowBranch(stemGO, branch, temp);
//    //    branch.IncrementBranchSide();
//    //    GrowBranch(stemGO, branch, temp);
//    //    branch.IncrementBranchSide();
//    //    GrowContinue(stemGO, branch, temp);
//    //}

//    //private GameObject GrowNormal(GameObject stemGO, GameObject emptyGO, Branch branch, List<GameObject> growingTemp)
//    //{
//    //    int branchNum = stemGO.GetComponent<StemFE>().Stem.BranchNum;
//    //    int currStep = branch.CurrStep + 1;

//    //    GameObject newStemGO = InstantiateStem(Plant.CreateStem(branchNum), branchNum, currStep, branch, false, false);

//    //    // Set the parent. If it's the first comp in the branch (besides the branchBase), set it to the emptyGO sibling of branchBase
//    //    if (currStep == 1)
//    //    {
//    //        newStemGO.GetComponent<StemFE>().SetParent(emptyGO.transform);
//    //    }
//    //    else
//    //    {
//    //        newStemGO.GetComponent<StemFE>().SetParent(stemGO.transform.parent);
//    //    }

//    //    // Set the rotation
//    //    newStemGO.GetComponent<StemFE>().SetRotation(Vector3.zero);

//    //    // Set the position
//    //    if (HitBarrier(stemGO, newStemGO))
//    //    {
//    //        Debug.Log(stemGO.name + " stopped growing in normal");
//    //        return null;
//    //    }
//    //    else
//    //    {
//    //        newStemGO.GetComponent<StemFE>().SetPosition(new Vector3(0, currStep * 2, 0));
//    //        growingTemp.Add(newStemGO);
//    //    }

//    //    // Set the scale. Needs to be globally so it comes in at the original scale
//    //    newStemGO.GetComponent<StemFE>().SetScaleGlobal(Plant.Taxonomy.Species.StemInitialSize.ToVector3());

//    //    newStemGO.GetComponent<StemFE>().AddLeaves(this, LeafGOs); // TODO: move this so it's only here once for each type of growing
//    //    newStemGO.GetComponent<StemFE>().AddFlowers(this, ThisYearsFlowerGOs, Plant.ThisYearsFlowers);

//    //    return newStemGO;
//    //}

//    //private GameObject GrowBranch(GameObject stemGO, Branch branch, List<GameObject> growingTemp)
//    //{
//    //    // Start the new branch
//    //    Plant.Stems.Add(new List<Stem>());
//    //    int branchNum = Plant.Stems.Count - 1;

//    //    GameObject newStemGO = InstantiateStem(Plant.CreateStem(branchNum), branchNum, 0, branch, true, false);

//    //    // Set the parent to stemGO's parent
//    //    newStemGO.GetComponent<StemFE>().SetParent(stemGO.transform.parent);

//    //    // Set the rotation. This sets the branch's direction. Getting it directly from taxonomy so it's random everytime
//    //    Vector3 rotation = branch.GetRotationFromSide(Plant.Taxonomy.Species.Genus.GetVariedBranchRotation(false).ToVector3());
//    //    newStemGO.GetComponent<StemFE>().SetRotation(rotation);

//    //    // Set the position
//    //    if (HitBarrier(stemGO, newStemGO))
//    //    {
//    //        Debug.Log(stemGO.name + " stopped growing in branch");
//    //        return null;
//    //    }
//    //    else
//    //    {
//    //        newStemGO.GetComponent<StemFE>().SetPosition(branch.GetPositionFromSide(newStemGO));
//    //        growingTemp.Add(newStemGO);
//    //    }

//    //    // Set the scale. Needs to be globally so it comes in at the original scale (not grown during game time)
//    //    newStemGO.GetComponent<StemFE>().SetScaleGlobal(Plant.Taxonomy.Species.StemInitialSize.ToVector3());

//    //    newStemGO.GetComponent<StemFE>().AddLeaves(this, LeafGOs);
//    //    newStemGO.GetComponent<StemFE>().AddFlowers(this, ThisYearsFlowerGOs, Plant.ThisYearsFlowers);
//    //    return newStemGO;
//    //}

//    //// Continues the current pattern after creating a branch. This is like a new branch but it continues the main trunk/branch
//    //private GameObject GrowContinue(GameObject stemGO, Branch branch, List<GameObject> growingTemp)
//    //{
//    //    Plant.Stems.Add(new List<Stem>());
//    //    int branchNum = Plant.Stems.Count - 1;
//    //    branch.CurrStep++;

//    //    GameObject newStemGO = InstantiateStem(Plant.CreateStem(branchNum), branchNum, 0, branch, true, false);

//    //    // Set the parent
//    //    newStemGO.GetComponent<StemFE>().SetParent(stemGO.transform.parent);

//    //    // Set the rotation. This sets the main branch/trunk's direction. Getting it directly from taxonomy so it's random everytime
//    //    newStemGO.GetComponent<StemFE>().SetRotation(Plant.Taxonomy.Species.Genus.GetVariedTrunkRotation(true).ToVector3());

//    //    // Set the position
//    //    if (HitBarrier(stemGO, newStemGO))
//    //    {
//    //        Debug.Log(stemGO.name + " stopped growing in continue");
//    //        return null;
//    //    }
//    //    else
//    //    {
//    //        newStemGO.GetComponent<StemFE>().SetPosition(branch.GetPositionFromSide(newStemGO));
//    //        growingTemp.Add(newStemGO);
//    //    }

//    //    // Set the scale. Needs to be globally so it comes in at the original scale (not grown during game time)
//    //    newStemGO.GetComponent<StemFE>().SetScaleGlobal(Plant.Taxonomy.Species.StemInitialSize.ToVector3());

//    //    newStemGO.GetComponent<StemFE>().AddLeaves(this, LeafGOs);
//    //    newStemGO.GetComponent<StemFE>().AddFlowers(this, ThisYearsFlowerGOs, Plant.ThisYearsFlowers);
//    //    return newStemGO;
//    //}

//    //// When things are moved to Plant and plants are growing in other scenes this might have to be checked when the game is loaded
//    //// and parts removed then
//    //private bool HitBarrier(GameObject stemGO, GameObject newStemGO)
//    //{
//    //    float distance = stemGO.transform.lossyScale.magnitude + BarrierBuffer;
//    //    if (Physics.Raycast(stemGO.transform.position, stemGO.transform.TransformDirection(stemGO.transform.forward), out _, distance, ConstantValues.LayerMasks.Barrier))
//    //    {
//    //        Debug.DrawRay(stemGO.transform.position, distance * stemGO.transform.TransformDirection(stemGO.transform.forward).normalized, Color.red, 5f);
//    //        StemsGOs.Remove(newStemGO);
//    //        Destroy(newStemGO);
//    //        return true;
//    //    }
//    //    else
//    //    {
//    //        return false;
//    //    }
//    //}

//    /// <summary>
//    /// Creates a GameObject from stem prefab and assigns newStem to its stem slot.
//    /// </summary>
//    /// <param name="newStem"></param>
//    /// <returns></returns>
//    private GameObject InstantiateStem(Stem newStem)
//    {
//        GameObject newStemGO = Instantiate(Resources.Load(ConstantValues.Prefabs.Stem)) as GameObject;
//        newStemGO.GetComponent<StemFE>().Stem = newStem;
//        //newStemGO.transform.SetParent(GameObject.Find(newStem.Parent).transform); // PERFORMANCE: need to find a better way to do this
//        newStemGO.name = newStem.StemID;
//        newStemGO.GetComponent<MeshRenderer>().enabled = FirstStemGO.GetComponent<MeshRenderer>().enabled; // For when the plant grows while being held
//        StemsGOs.Add(newStemGO);
//        return newStemGO;
//    }
//}
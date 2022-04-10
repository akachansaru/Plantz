using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Plant
{
    [SerializeField] private Pot pot = new Pot();
    [SerializeField] private string plantID;
    [SerializeField] private float health = 100f;
    [SerializeField] private DateTime dateCreated;

    [SerializeField] private Taxonomy taxonomy;
    [SerializeField] private bool isMaxSize; // True if the plant contains the maximum amount of stems. Will not add more // FIXME: this counts the empties and maybe the leaves
    [SerializeField] private bool isMaxCaliper; // True if the first stem is fully widened. The other stems will stop growing wider
    [SerializeField] private List<List<Stem>> stems = new List<List<Stem>>(); // Each List<Stem> is a branch
    [SerializeField] private List<Stem> growingStems = new List<Stem>();
    // UNDONE: make a list to keep track of the stems that stopped growing due to obstacles. Add them to growing Stems once the plant is moved
    [SerializeField] private List<Leaf> leaves = new List<Leaf>();
    //[SerializeField] private List<Flower> lastYearsFlowers = new List<Flower>();
    //[SerializeField] private List<Flower> thisYearsFlowers = new List<Flower>();
    [SerializeField] private List<Flower> flowers = new List<Flower>();
    [SerializeField] private List<Fruit> fruit = new List<Fruit>();
    [SerializeField] private bool isPollinated = false;

    public Pot Pot { get { return pot; } set { pot = value; } }
    public string PlantID { get { return plantID; } private set { plantID = value; } }
    public float Health { get { return health; } set { health = value; } }
    public Taxonomy Taxonomy { get { return taxonomy; } }
    public bool IsMaxSize { get { return isMaxSize; } set { isMaxSize = value; } }
    public bool IsMaxCaliper { get { return isMaxCaliper; } set { isMaxCaliper = value; } }
    public List<List<Stem>> Stems { get { return stems; } private set { stems = value; } }
    public List<Stem> GrowingStems { get { return growingStems; } } // Changed from string to Stem 5/3/20
    public List<Leaf> Leaves { get { return leaves; } }
    //public List<Flower> LastYearsFlowers { get { return lastYearsFlowers; } }
    //public List<Flower> ThisYearsFlowers { get { return thisYearsFlowers; } }
    public List<Flower> Flowers { get { return flowers; } }
    public List<Fruit> Fruit { get { return fruit; } }
    public bool IsPollinated { get { return isPollinated; } private set { isPollinated = value; } } // Flower also has this. Should choose how the plant will be pollinated and get rid of one

    public bool FruitNeedsUpdating { get; set; } // PlantFE will check for this and swap out the flower models with fruit models

    public Stem FirstStem { get; set; }
    public float TimeUntilGrowth { get; set; }
    public float TimeUntilStemGrowth { get; set; }
    public Vector3Serializable Position { get; set; } // For plants growing in the wild. Greenhouse plants won't use this. TODO: make a new script for plants growing in the wild

    /// <summary>
    /// Creates an ID and initializes the plant with the given taxonomy
    /// </summary>
    /// <param name="taxonomy"></param>
    /// <param name="num"></param>
    /// <param name="saveListName"></param>
    public Plant(Taxonomy taxonomy, List<Plant> saveList, string saveListName)
    {
        dateCreated = DateTime.Now;
        Stems.Add(new List<Stem>());
        if (GlobalControl.Instance)
        {
            CreateID(saveList.Count, saveListName);
        }
        else
        {
            CreateID(0, saveListName);
        }
        Pot = new Pot(new Vector3Serializable(0.5f, 0.5f, 0.5f), new Vector4Serializable(1, 1, 1, 1));
        InitializePlant(taxonomy);
    }

    /// <summary>
    /// For the worktable plants
    /// </summary>
    /// <param name="taxonomy"></param>
    /// <param name="saveListName"></param>
    public Plant(Taxonomy taxonomy, Pot pot, string saveListName)
    {
        dateCreated = DateTime.Now;
        if (saveListName != ConstantValues.SaveLists.Greenhouse)
        {
            Debug.LogError("This should only be used for Greenhouse plants");
        }
        Stems.Add(new List<Stem>());
        if (GlobalControl.Instance)
        {
            CreateID(GlobalControl.Instance.savedValues.PlantIDNum++, saveListName);
        }
        else
        {
            CreateID(0, saveListName);
        }
        Pot = pot;
        InitializePlant(taxonomy);
    }

    public TimeSpan GetAge()
    {
        return DateTime.Now - dateCreated;
    }

    /// <summary>
    /// Starts fruit in each flower that has seeds that are a cross of this plant and pollen
    /// </summary>
    /// <param name="pollen"></param>
    /// <returns></returns>
    public void Pollinate(Pollen pollen)
    {
        Taxonomy newTaxonomy = new Taxonomy(this, pollen);

        if (newTaxonomy.Species != null)
        {
            IsPollinated = true;
            foreach (Flower flower in Flowers)
            {
                // add the pollinated taxonomy so the fruit can eventually take it once it matures
                flower.Pollinate(newTaxonomy);
            }
        }
    }

    private void CreateID(int num, string saveListName)
    {
        PlantID = "P" + num + " " + saveListName;
    }

    private void InitializePlant(Taxonomy taxonomy)
    {
        this.taxonomy = taxonomy;
        TimeUntilGrowth = Taxonomy.Species.Genus.TimeBetweenPlantGrowth; // So the plant doesn't immediately grow a stem
        TimeUntilStemGrowth = Taxonomy.Species.StemGrowthRate.Value; // So the plant doesn't immediately widen

        FirstStem = new Stem(this, 0)
        {
            BranchNum = 0,
            GlobalScale = new Vector3Serializable(Taxonomy.Species.StemInitialSize.Value, Taxonomy.Species.StemInitialSize.Value, Taxonomy.Species.StemInitialSize.Value)
        };
        FirstStem.Branch = new Branch();
        FirstStem.Parent = PlantID;
        Stems[0].Add(FirstStem);
        FirstStem.IsFirstStem = true;
        growingStems.Add(FirstStem); // Start off the stem that will grow
    }

    /// <summary>
    /// Returns a new stem fromt stemTemplate with ID and adds it to the list of all stems on the plant as part of the branch at branchNum.
    /// </summary>
    /// <param name="growingNode"></param>
    /// <param name="branchNum"></param>
    /// <returns></returns>
    public Stem CreateStem(int branchNum, int currStep, Branch branch, bool isBranchBase)
    {
        Stem newStem = new Stem(this, Stems.SelectMany(list => list).Count());
        Stems[branchNum].Add(newStem);
        newStem.BranchNum = branchNum;
        newStem.Branch = new Branch(currStep, branch)
        {
            IsBranchBase = isBranchBase,
            YearGrown = GlobalControl.Instance.savedValues.Year
        };
        return newStem;
    }

    // *** start of where I added stuff (besides most of CreateStem)

    /// <summary>
    /// For pre-growing plants. Should load in at the age indicated
    /// </summary>
    /// <param name="growNum"></param>
    public Plant InstaGrow(int numDays, float leafGrownPercent, float flowerGrownPercent)
    { // This is from GrowPlant()
        List<Leaf> leavesToRemove = new List<Leaf>();
        List<Flower> flowersToRemove = new List<Flower>();
        
        float timePassedForStemWiden = 0f; // So the stems can widen at the appropriate rate
        float timePassedForLeafDrop = 0f;
        float timePassedTotal = 0f;

        while (timePassedTotal < GameTime.ToMinutes(numDays, TimeUnits.Day))
        {
            if (timePassedForStemWiden >= Taxonomy.Species.StemGrowthRate.Value)
            {
                WidenStems();
                timePassedForStemWiden = 0f;
            }

            if (Stems.Count < Taxonomy.Species.Genus.PlantMaxSize)
            {
                GrowAllBranches();
            }
            else
            {
                IsMaxSize = true;
            }

            timePassedForStemWiden += Taxonomy.Species.Genus.TimeBetweenPlantGrowth; // The amount of time that would have passed had the plant actually been growing
            timePassedForLeafDrop += timePassedForStemWiden;
            timePassedTotal += timePassedForStemWiden;

            // Remove leaves and flowers if the plant has been growing for over a year
            if (timePassedForLeafDrop >= GameTime.YEAR)
            {
                timePassedForLeafDrop = 0f;

                foreach(Leaf leaf in Leaves)
                {
                    if (leaf.IsDropped)
                    {
                        leavesToRemove.Add(leaf); // If it's already been dropped that means it was grown in the year before and it shouldn't grow again
                    }
                    else
                    {
                        leaf.IsDropped = true;
                    }
                }

                // Not sure if this is right
                foreach (Flower flower in Flowers)
                {
                    if (flower.IsDropped)
                    {
                        flowersToRemove.Add(flower);
                    }
                    else
                    {
                        flower.IsDropped = true;
                    }
                }
            }
        }

        leavesToRemove.ForEach(l => Leaves.Remove(l));
        Leaves.ForEach(l => l.IsDropped = false); // Change them all to not IsDropped so they load in at full size

        flowersToRemove.ForEach(f => Flowers.Remove(f));
        Flowers.ForEach(f => f.IsDropped = false); // Change them all to not IsDropped so they load in at full size

        foreach (Flower flower in Flowers)
        {
            flower.GrowComp(Taxonomy.Species.Genus.FlowerMaxSize, flowerGrownPercent, ConstantValues.PlantConsts.FlowerPrefabScale);
        }

        foreach (Leaf leaf in Leaves)
        {
            leaf.GrowComp(Taxonomy.Species.Genus.LeafMaxSize.Value, leafGrownPercent, ConstantValues.PlantConsts.LeafPrefabScale);
        }
        return this;
    }

    // Taken from GrowFlowerHelper in PlantFE
    //private void GrowComp(List<IPlantComponent> list, float maxSize, float grownPercent, Vector3Serializable prefabScale)
    //{
    //    float targetSize = maxSize * grownPercent;

    //    Debug.Log(PlantID + " target size: " + targetSize + " " + list.Count);
    //    bool temp = false;

    //    foreach (IPlantComponent comp in list)
    //    {
    //        Vector3Serializable initialScale = comp.LocalScale;

    //        comp.LocalScale = targetSize * prefabScale / prefabScale.x; // divided by x to keep growthAmt.x = ConstantValues.PlantConsts.GrowthAmt

    //        Vector3Serializable finalScale = comp.LocalScale;

    //        // Move the flowers so they are still attached to the plant 
    //        Vector3Serializable sign = new Vector3Serializable(comp.LocalPosition.ToVector3().normalized);
    //        Vector3Serializable deltaScale = finalScale - initialScale;

    //        // Only using the x comp of the deltaScale since that's the only length that matters with current flower model
    //        comp.LocalPosition += sign * (deltaScale.x / 2);
    //        if (!temp)
    //        {
    //            Debug.Log(PlantID + " made it here. Size should be " + comp.LocalScale);
    //            temp = true;
    //        }
    //        if (grownPercent == 1)
    //        {
    //            comp.Mature();
    //        }
    //    }
    //}

    //private void GrowLeaves(float grownPercent)
    //{
    //    float targetSize = Taxonomy.Species.Genus.LeafMaxSize * grownPercent;
    //    foreach (Leaf leaf in Leaves)
    //    {
    //        Vector3Serializable initialScale = leaf.LocalScale;

    //        leaf.LocalScale = targetSize * ConstantValues.PlantConsts.LeafPrefabScale /
    //            ConstantValues.PlantConsts.LeafPrefabScale.x; // divided by x to keep growthAmt.x = ConstantValues.PlantConsts.GrowthAmt

    //        Vector3Serializable finalScale = leaf.LocalScale;

    //        // Move the leaves so they are still attached to the plant 
    //        Vector3Serializable sign = new Vector3Serializable(leaf.LocalPosition.ToVector3().normalized);
    //        Vector3Serializable deltaScale = finalScale - initialScale;

    //        // Only using the x comp of the deltaScale since that's the only length that matters with current leaf model
    //        leaf.LocalPosition += sign * (deltaScale.x / 2);
    //    }
    //}

    /// <summary>
    /// Adds new stem(s) to the plant according to the branchPattern
    /// </summary>
    //public IEnumerator GrowPlant()
    //{
    //    yield return new WaitForSeconds(TimeUntilGrowth); // For when the game is loaded. 
    //    for (; ; )
    //    {
    //        if (Stems.Count < Taxonomy.Species.Genus.PlantMaxSize)
    //        {
    //            GrowAllBranches();
    //        }
    //        else
    //        {
    //            IsMaxSize = true;
    //            Debug.Log(PlantID + " reached max size.");
    //        }

    //        //foreach (GameObject stemGO in GrowingStemGOs)
    //        //{ // PERFORMANCE: Can I add these while iterating for GrowNext?
    //        //    Plant.GrowingStems.Add(stemGO.name);
    //        //}

    //        float timeUntilGrowth;
    //        for (timeUntilGrowth = Taxonomy.Species.Genus.PlantGrowthRate; timeUntilGrowth > 0; timeUntilGrowth -= Time.deltaTime)
    //        {
    //            TimeUntilGrowth = timeUntilGrowth;
    //            yield return null;
    //        }
    //        GlobalControl.Instance.Save();
    //    }
    //}

    // This might just be for GrowPlantInsta
    private List<Stem> GrowAllBranches()
    {
        List<Stem> newStems = new List<Stem>();
        List<Stem> temp = new List<Stem>();
        foreach (Stem stem in GrowingStems)
        {
            newStems.AddRange(GrowNext(stem, temp));
        }
        GrowingStems.Clear();
        GrowingStems.AddRange(temp);
        return newStems;
    }

    private List<Stem> GrowNext(Stem stem, List<Stem> temp)
    {
        List<Stem> newStems;
        Branch branch = stem.Branch;
        if (branch.CurrStep != Taxonomy.Species.Internodes)
        {   
            // When not branching. Create an empty stem to act as the parent to avoid scale skewing
            Stem emptyStem = null;
            if (branch.CurrStep == 0)
            {
                emptyStem = CreateEmptyStem(stem);
            }
            newStems = GrowNormal(stem, emptyStem, branch, temp);
        }
        else
        {
            // Create the branch(s) and the trunk/main branch
            newStems = DoBranchingPattern(branch, Taxonomy.Species.Genus.Family.BranchPattern, stem, temp);
        }
        return newStems;
    }

    private List<Stem> DoBranchingPattern(Branch branch, BranchPatterns branchPattern, Stem stem, List<Stem> temp)
    {
        List<Stem> newStems;
        switch (branchPattern)
        {
            case BranchPatterns.Alternate:
                newStems = AlternateBranch(branch, stem, temp);
                break;
            case BranchPatterns.Opposite:
                newStems = OppositeBranch(branch, stem, temp);
                break;
            case BranchPatterns.Whorled:
                newStems = WhorledBranch(branch, stem, temp);
                break;
            default:
                Debug.LogError("Missing BranchPattern.");
                newStems = new List<Stem>();
                break;
        }
        return newStems;
    }

    /// <summary>
    /// Returns an existing empty if one was already made in Leaf or Flower. Otherwise, creates a new empty.
    /// </summary>
    /// <param name="stem"></param>
    /// <returns></returns>
    public Stem CreateEmptyStem(Stem stem)
    {
        Stem emptyStem = stem.Clone();
        emptyStem.IsEmpty = true;
        emptyStem.GlobalScale = new Vector3Serializable(Taxonomy.Species.StemInitialSize.Value,
                                                        Taxonomy.Species.StemInitialSize.Value,
                                                        Taxonomy.Species.StemInitialSize.Value);
        emptyStem.CreateID("E " + stem.StemID);

        // PERFORMANCE: find a way to identify if an empty already exists without looking through all stems
        // Have to look through all the stems incase the empty was already added when leaves/flowers were added so the same empty isn't added twice
        foreach(List<Stem> branch in Stems)
        {
            if (branch.Contains(emptyStem)) // Overwrote the Stem.Equals so it only looks at the ID
            {
                return branch[branch.IndexOf(emptyStem)];
            }
        }

        Stems[stem.BranchNum].Add(emptyStem);
        return emptyStem;
    }

    private List<Stem> AlternateBranch(Branch branch, Stem stem, List<Stem> temp)
    {
        List<Stem> newStems = new List<Stem>();

        newStems.Add(GrowBranch(stem, branch, temp));
        branch.IncrementBranchSide(360 / Taxonomy.Species.BranchesPerCycle);
        newStems.Add(GrowContinue(stem, branch, temp));

        return newStems;
    }

    private List<Stem> OppositeBranch(Branch branch, Stem stem, List<Stem> temp)
    {
        List<Stem> newStems = new List<Stem>();

        newStems.Add(GrowBranch(stem, branch, temp));
        branch.IncrementBranchSide(180);
        newStems.Add(GrowBranch(stem, branch, temp));
        branch.IncrementBranchSide(360 / Taxonomy.Species.BranchesPerCycle);
        newStems.Add(GrowContinue(stem, branch, temp));
        return newStems;
    }

    private List<Stem> WhorledBranch(Branch branch, Stem stem, List<Stem> temp)
    {
        List<Stem> newStems = new List<Stem>();

        for (int b = 0; b < Taxonomy.Species.BranchesPerCycle; b++)
        {
            newStems.Add(GrowBranch(stem, branch, temp));
            branch.IncrementBranchSide(360 / Taxonomy.Species.BranchesPerCycle);
        }
        newStems.Add(GrowContinue(stem, branch, temp));
        return newStems;
    }

    private List<Stem> GrowNormal(Stem stem, Stem emptyStem, Branch branch, List<Stem> growingTemp)
    {
        int branchNum = stem.BranchNum;
        int currStep = branch.CurrStep + 1;

        Stem newStem = CreateStem(branchNum, currStep, branch, false);

        if (currStep == 1)
        {
            newStem.Parent = emptyStem.StemID;
        }
        else
        {
            newStem.Parent = stem.Parent;
        }

        newStem.Rotation = new Vector3Serializable(Vector3.zero);
        newStem.Position = new Vector3Serializable(0, currStep * 2, 0);
        growingTemp.Add(newStem);

        // Set the scale. Needs to be globally so it comes in at the original scale
        newStem.GlobalScale = new Vector3Serializable(Taxonomy.Species.StemInitialSize.Value,
                                                      Taxonomy.Species.StemInitialSize.Value,
                                                      Taxonomy.Species.StemInitialSize.Value);

        newStem.AddLeaves(this, Leaves); // TODO: move this so it's only here once for each type of growing
        //newStem.AddFlowers(this, ThisYearsFlowers);
        newStem.AddFlowers(this, Flowers);

        return new List<Stem> { newStem };
    }

    private Stem GrowBranch(Stem stem, Branch branch, List<Stem> growingTemp)
    {
        // Start the new branch
        Stems.Add(new List<Stem>());
        int branchNum = Stems.Count - 1;

        Stem newStem = CreateStem(branchNum, 0, branch, true);

        // Set the parent to stem's parent
        newStem.Parent = stem.Parent;

        // Set the rotation. This sets the branch's direction. Getting it directly from taxonomy so it's random everytime
        newStem.Rotation = branch.GetRotationFromSide(Taxonomy.Species.GetVariedBranchRotation(false));

        newStem.Position = branch.GetPositionFromSide(newStem);
        growingTemp.Add(newStem);

        // Set the scale. Needs to be globally so it comes in at the original scale (not grown during game time)
        newStem.GlobalScale = new Vector3Serializable(Taxonomy.Species.StemInitialSize.Value,
                                                      Taxonomy.Species.StemInitialSize.Value,
                                                      Taxonomy.Species.StemInitialSize.Value);

        newStem.AddLeaves(this, Leaves);
        //newStem.AddFlowers(this, ThisYearsFlowers);
        newStem.AddFlowers(this, Flowers);
        return newStem;
    }

    // Continues the current pattern after creating a branch. This is like a new branch but it continues the main trunk/branch
    private Stem GrowContinue(Stem stem, Branch branch, List<Stem> growingTemp)
    {
        Stems.Add(new List<Stem>());
        int branchNum = Stems.Count - 1;
        branch.CurrStep++;

        Stem newStem = CreateStem(branchNum, 0, branch, true);

        // Set the parent
        newStem.Parent = stem.Parent;

        // Set the rotation. This sets the main branch/trunk's direction. Getting it directly from taxonomy so it's random everytime
        newStem.Rotation = Taxonomy.Species.GetVariedTrunkRotation(true);

        newStem.Position = branch.GetPositionFromSide(newStem);
        growingTemp.Add(newStem);

        // Set the scale. Needs to be globally so it comes in at the original scale (not grown during game time)
        newStem.GlobalScale = new Vector3Serializable(Taxonomy.Species.StemInitialSize.Value,
                                                      Taxonomy.Species.StemInitialSize.Value,
                                                      Taxonomy.Species.StemInitialSize.Value);

        newStem.AddLeaves(this, leaves);
        //newStem.AddFlowers(this, ThisYearsFlowers);
        newStem.AddFlowers(this, Flowers);
        return newStem;
    }

    // This is the coroutine in PlantFE stripped of its coroutineness. Right now just for insta grow
    private void WidenStems()
    {
        float stemGrowthAmt = Taxonomy.Species.StemGrowthAmt;
        //float firstStemGlobalScale = FirstStemGO.GetComponent<StemFE>().Stem.GlobalScale.x;
        float firstStemGlobalScale = FirstStem.GlobalScale.x;
        if (firstStemGlobalScale + stemGrowthAmt < Taxonomy.Species.StemMaxSize.Value)
        {
            if (firstStemGlobalScale + stemGrowthAmt < Pot.PotSize.x &&
                firstStemGlobalScale + stemGrowthAmt < Pot.PotSize.z)
            {
                WidenAllStems(stemGrowthAmt);
            }
            else if(firstStemGlobalScale < Pot.PotSize.x && firstStemGlobalScale < Pot.PotSize.z) // Add the extra on to get it to exactly the max size
            {
                Debug.Log("widening for pot size: " + (Pot.PotSize.x - firstStemGlobalScale));
                WidenAllStems(Pot.PotSize.x - firstStemGlobalScale); // TODO: do a separate one for potSize.z
            }
        }
        else if (firstStemGlobalScale < Taxonomy.Species.StemMaxSize.Value) // Add the extra on to get it to exactly the max size
        {
            Debug.Log("widening for stem size: " + (Taxonomy.Species.StemMaxSize.Value - firstStemGlobalScale));
            WidenAllStems(Taxonomy.Species.StemMaxSize.Value - firstStemGlobalScale);
        }
    }

    private void WidenAllStems(float growthAmt)
    {
        List<Stem> empties = new List<Stem>();
        foreach (List<Stem> branch in Stems)
        { 
            foreach(Stem stem in branch)
            {
                if (!stem.IsEmpty)
                {
                    WidenStem(stem, growthAmt);
                }
                else
                {
                    empties.Add(stem);
                }
            }
        }
        // Move the leaves and flowers so they look like they're staying with the stem as it grows
        AdjustLeafAndFlowerPositions(empties);
    }

    private void WidenStem(Stem stem, float growthAmt)
    {
        stem.GlobalScale = new Vector3Serializable(stem.GlobalScale.x + growthAmt, stem.GlobalScale.y, stem.GlobalScale.z + growthAmt);
    }

    // This is from PlantFE but changed
    private void AdjustLeafAndFlowerPositions(List<Stem> empties)
    {
        float growthAmt = ConstantValues.PlantConsts.StemGrowthPercent / 2; // Divide by 2 because it's growing evenly on both sides
        foreach (Stem empty in empties)
        {
            foreach (Leaf leaf in empty.Leaves)
            {
                Vector3Serializable moveAmt = new Vector3Serializable(
                    growthAmt * leaf.LocalPosition.ToVector3().normalized.x,
                    growthAmt * leaf.LocalPosition.ToVector3().normalized.y,
                    growthAmt * leaf.LocalPosition.ToVector3().normalized.z);
                leaf.LocalPosition += moveAmt;
            }

            foreach (Flower flower in empty.Flowers)
            {
                Vector3Serializable moveAmt = new Vector3Serializable(
                    growthAmt * flower.LocalPosition.ToVector3().normalized.x,
                    growthAmt * flower.LocalPosition.ToVector3().normalized.y,
                    growthAmt * flower.LocalPosition.ToVector3().normalized.z);
                flower.LocalPosition += moveAmt;
            }
        }
    }
}
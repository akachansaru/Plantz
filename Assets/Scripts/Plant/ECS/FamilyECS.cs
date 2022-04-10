using Unity.Mathematics;
using UnityEngine;

public class FamilyECS
{
    public int FamilyNum { get; private set; }
    public Rarity Rarity { get; private set; }

    public BranchPatterns BranchPattern { get; private set; }


    public FamilyECS(int genusNum)
    {
        FamilyNum = SetFamily(genusNum);
        Rarity = SetRarity(FamilyNum);

        BranchPattern = SetBranchPattern(FamilyNum);

    }

    // Family is determined by the genus
    private int SetFamily(int genus)
    {
        switch (genus)
        {
            case 0:
                return 0;
            case 1:
                return 1;
            case 2:
                return 2;
            default:
                Debug.LogError("Invalid genus num");
                return 0;
        }
    }
    private BranchPatterns SetBranchPattern(int family)
    {
        switch (family)
        {
            case 0:
                return BranchPatterns.Alternate;
            case 1:
                return BranchPatterns.Opposite;
            case 2:
                return BranchPatterns.Whorled;
            default:
                Debug.LogError("Invalid family num");
                return BranchPatterns.Alternate;
        }
    }

    private Rarity SetRarity(int family)
    {
        switch (family)
        {
            case 0:
                return Rarity.Common;
            case 1:
                return Rarity.Uncommon;
            case 2:
                return Rarity.Rare;
            default:
                Debug.LogError("Invalid genus num");
                return Rarity.Common;
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The family dictates the branch pattern type (opposite, alternate) (x), branch/trunk rotation
/// </summary>
[Serializable]
public class Family
{
    [SerializeField] private string familyName;
    [SerializeField] private BranchPatterns branchPattern;
    [SerializeField] private Rarity rarity;

    public string FamilyName { get { return familyName; } protected set { familyName = value; } }
    public BranchPatterns BranchPattern { get { return branchPattern; } protected set { branchPattern = value; } }
    public Rarity Rarity { get { return rarity; } protected set { rarity = value; } }

    public override string ToString()
    {
        return GetType().Name;
    }
}

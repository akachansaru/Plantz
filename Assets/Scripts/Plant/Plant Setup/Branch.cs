using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Branch
{
    [SerializeField] private int currStep = 0;
    [SerializeField] private int yearGrown = 0;
    //PERFORMANCE: could keep track of the stems in here rather than in Plant
    
    public bool IsBranchBase { get; set; }
    public int YearGrown { get { return yearGrown; } set { if (value < 0) { Debug.LogError("YearGrown cannot be negative."); } yearGrown = value; } }
    public int CurrStep { get { return currStep; } set { if (value < 0) { Debug.LogError("CurrStep cannot be negative."); } currStep = value; } }
    private float BranchSide { get; set; } = 0f;

    public Branch() { }

    public Branch(int currStep, Branch cloneBranch)
    {
        this.currStep = currStep;
        //BranchSide = cloneBranch.BranchSide;
        BranchSide = UnityEngine.Random.Range(0, 360);
    }

    public void IncrementBranchSide(float angle)
    {
        BranchSide += angle;
    }

    // This currently only sets the y position. Will set the next stem right above the last. Might be here that I need to fix the overlapping stems
    public Vector3Serializable GetPositionFromSide(Stem newStem)
    {
        // If the angle is over 180 degrees have to adjust the rotation to an equivalent < 180 so the position comes out right
        //float rotationX = newStem.Rotation.x;
        //if (newStem.Rotation.x > 180)
        //{
        //    rotationX -= 360;
        //}
        //rotationX = Mathf.Abs(rotationX);

        return new Vector3Serializable(0, CurrStep * 2, 0);
    }

    public Vector3Serializable GetRotationFromSide(Vector3Serializable currRotation)
    {
        return new Vector3Serializable(currRotation.x, currRotation.y + BranchSide, currRotation.z); ;
    }
}
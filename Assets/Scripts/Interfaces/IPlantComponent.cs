using UnityEngine;
using System.Collections;
using System;

[Serializable]
public abstract class IPlantComponent
{
    [SerializeField] protected string compID;
    [SerializeField] private string parent = "";
    [SerializeField] private Vector3Serializable localPosition;
    [SerializeField] private Vector3Serializable localRotation;
    [SerializeField] private Vector3Serializable localScale;

    [SerializeField] private bool isDropped = false;
    [SerializeField] private bool isMature = false;

    public string Parent { get { return parent; } set { if (value == "") { Debug.LogError("Stem name is blank."); } parent = value; } }
    public Vector3Serializable LocalPosition { get { return localPosition; } set { localPosition = value; } }
    public Vector3Serializable LocalRotation { get { return localRotation; } set { localRotation = value; } }
    public Vector3Serializable LocalScale { get { return localScale; } set { localScale = value; } }
    public bool IsDropped { get { return isDropped; } set { isDropped = value; } }
    public bool IsMature { get { return isMature; } set { isMature = value; } }

    public float TimeUntilGrowth { get; set; }

    public string CreateID(string stem, int num, string typeCode)
    {
        compID = typeCode + num + " " + stem;
        return compID;
    }

    public string GetID()
    {
        return compID;
    }

    // For InstaGrow. Taken from GrowFlowerHelper in PlantFE
    public void GrowComp(float maxSize, float grownPercent, Vector3Serializable prefabScale)
    {
        float targetSize = maxSize * grownPercent;

        Vector3Serializable initialScale = LocalScale;
        LocalScale = targetSize * prefabScale / prefabScale.x; // divided by x to keep growthAmt.x = ConstantValues.PlantConsts.GrowthAmt
        Vector3Serializable finalScale = LocalScale;

        //// Move the flowers so they are still attached to the plant 
        //Vector3Serializable sign = new Vector3Serializable(LocalPosition.ToVector3().normalized);
        //Vector3Serializable deltaScale = finalScale - initialScale;

        //// Only using the x comp of the deltaScale since that's the only length that matters with current flower model
        //LocalPosition += sign * (deltaScale.x / 2);
        LocalPosition += GetPositionForScaleChange(initialScale, finalScale, LocalPosition);
        if (grownPercent == 1)
        {
            Mature();
        }
    }

    public Vector3Serializable GetPositionForScaleChange(Vector3Serializable initialScale, Vector3Serializable finalScale, Vector3Serializable pos)
    {
        // Move the flowers so they are still attached to the plant 
        Vector3Serializable sign = finalScale.x > initialScale.x ? new Vector3Serializable(pos.ToVector3().normalized) : new Vector3Serializable(-pos.ToVector3().normalized);
        Vector3Serializable deltaScale = finalScale - initialScale;
        //this isn't working quite right for the fruit. Slightly in the branch
        // Only using the x comp of the deltaScale since that's the only length that matters with current models
        return sign * (deltaScale.x / 2);
    }

    public virtual void Mature()
    {
        IsMature = true;
    }
}
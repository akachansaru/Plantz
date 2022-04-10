using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotFrontEnd : MonoBehaviour
{
    [SerializeField] private Pot pot;
    [SerializeField] private Color potColor = new Color(195, 127, 95, 1);

    public Pot Pot { get { return pot; } set { pot = value; } }

    public void Awake()
    {
        SetColorFromFrontEnd();
        ApplyColor(gameObject, Pot);
        ApplySize(gameObject, Pot);
    }

    public static void ApplyColor(GameObject applyToPotGO, Pot applyFromPot) 
    {
        applyToPotGO.GetComponent<MeshRenderer>().material.color = applyFromPot.PotColor.ToVector4();
    }

    public static void ApplySize(GameObject applyToPotGO, Pot applyFromPot)
    {
        applyToPotGO.transform.localScale = applyFromPot.PotSize.ToVector3();
    }

    private void SetColorFromFrontEnd()
    {
        Pot.PotColor = new Vector4Serializable(potColor);
    }
}

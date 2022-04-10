using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Attached to the seed buttons for planting seeds
/// </summary>
public class SeedUI : MonoBehaviour
{
    [SerializeField] private Text buttonText = null;
    [SerializeField] private Text amtText = null;

    [SerializeField] private Taxonomy taxonomy;

    public Taxonomy Taxonomy { get { return taxonomy; } set { taxonomy = value; } }
    public int Amount { get; set; }

    private void Awake()
    {
        Assert.IsNotNull(buttonText);
        Assert.IsNotNull(amtText);
    }

    public void Start()
    {
        buttonText.text = GetComponent<SeedUI>().Taxonomy.ToString();
        amtText.text = "x" + Amount;
    }

    public void OnButtonClick()
    {
        PlantingManager.Instance.SwitchToPotCanvas(Taxonomy);
    }
}

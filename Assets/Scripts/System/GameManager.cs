using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject cultivarCanvas;
    public Text xCultivarStatsText;
    public InputField cultivarNameInput;

    [SerializeField] private string newCultivarName = "";
    [SerializeField] private bool isAcceptPressed = false;

    public void Awake()
    {
        Assert.IsNotNull(cultivarCanvas);
        Assert.IsNotNull(xCultivarStatsText);
        Assert.IsNotNull(cultivarNameInput);
    }

    public void Start()
    {
        Instance = this;
    }

    public IEnumerator OpenCultivarCanvas(List<Stat> cultivarStats, Taxonomy newTaxonomy)
    {
        GameTime.PauseGame();
        cultivarCanvas.SetActive(true);
        xCultivarStatsText.text = cultivarStats.Count + " cultivar stats were created.";

        for (; ; )
        {
            if (isAcceptPressed && newCultivarName != "")
            {
                string tempName = newTaxonomy.Species.SpeciesName;
                newTaxonomy.Species.SpeciesName += " '" + newCultivarName + "'";
                if (!GlobalControl.Instance.savedValues.AllSpecies.Contains(newTaxonomy))
                {
                    GlobalControl.Instance.savedValues.AllSpecies.Add(newTaxonomy);
                    cultivarCanvas.SetActive(false);
                    ResetAll();
                    GameTime.UnpauseGame();
                    yield break;
                }
                else
                {
                    newTaxonomy.Species.SpeciesName = tempName; // Reset the name if it's already been used
                    newCultivarName = "";
                    isAcceptPressed = false;
                    Debug.Log("Name already in use. Choose another.");
                }
            }
            yield return null;
        }
    }

    public void OnAcceptPressed()
    {
        isAcceptPressed = true;
    }

    public void OnEndCultivarNameEdit()
    {
        newCultivarName = cultivarNameInput.text;
    }

    private void ResetAll()
    {
        xCultivarStatsText.text = "";
        cultivarNameInput.text = "";
        newCultivarName = "";
        isAcceptPressed = false;
    }
}

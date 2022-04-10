using UnityEngine;
using UnityEngine.UI;

public class Worktable : MonoBehaviour
{
    // UNDONE: Add functionality to prune plant here
    [SerializeField] private Transform plantLocation = null;

    public static int GreenhouseNum { get; set; } // Used in EnterWorktable to know which greenhouse was used to enter the worktable. Don't need this yet, but will when there are multiple greenhouses

    private Plant SelectedPlant { get; set; }
    private GameObject SelectedPotGO { get; set; }
    private Taxonomy Taxonomy { get; set; }


    private void SetAddedGameObject(GameObject gameObject)
    {
        // FIXME: This is all a mess.
        switch (gameObject.GetComponent<MovePlant>().MoveType)
        {
            case MoveType.Pot:
                SelectedPotGO = gameObject;
                break;
            case MoveType.Plant:
                if (!SelectedPotGO)
                {
                    Debug.LogError("Need to choose a pot first.");
                } 
                else
                {
                    SelectedPlant = gameObject.GetComponent<PlantFE>().Plant;
                    SelectedPlant.Pot = SelectedPotGO.GetComponent<PotFrontEnd>().Pot;
                    Taxonomy = gameObject.GetComponent<InitializeIndividualPlant>().SetSpecies();
                }
                break;
            case MoveType.Soil:
                if (!SelectedPotGO || SelectedPlant == null)
                {
                    Debug.LogError("Need to choose a pot and plant first.");
                }
                else
                {
                    SelectedPlant.Pot.FillWithSoil(new Soil(gameObject.GetComponent<SoilFrontEnd>().Soil.BiomeType));
                }
                break;
            default:
                Debug.LogError("Invalid type.");
                break;
        }
    }

    /// <summary>
    /// Called by MovePlant which is on each plant in the worktable
    /// </summary>
    /// <param name="plantComponent"></param>
    /// <param name="originalPosition"></param>
    /// <returns></returns>
    public Vector3 SnapToWorktable(GameObject selectedGO, Vector3 originalPosition)
    {
        SetAddedGameObject(selectedGO);

        Vector3 location;
        if (selectedGO != null)
        {
            location = plantLocation.position;
        }
        else
        {
            location = originalPosition;
            // UNDONE: replace the previously selected plant with the new one
        }
        return location;
    }

    /// <summary>
    /// For use with UI button
    /// </summary>
    public void ButtonStartGrowing()
    {
        if (SelectedPlant != null)
        {
            // UNDONE: make this save it to the greenhouse of the workbench that was used to create it
            GlobalControl.Instance.savedValues.WorktablePlant = new Plant(Taxonomy, SelectedPlant.Pot, ConstantValues.SaveLists.Greenhouse);
            GlobalControl.Instance.savedValues.WorktablePlant.InstaGrow(10, 1, 1); // why am I insta growing it here?
        }
    }
}
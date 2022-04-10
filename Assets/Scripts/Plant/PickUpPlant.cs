using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlantFE))]
public class PickUpPlant : Interactable
{
    [SerializeField] private Vector3 distInFrontOfPlayer = new Vector3(0, 0.5f, 0.5f);
    [SerializeField] private float shrinkSize = 0.125f; // For picking up the plant. 1/8th the scale of the player

    private float ShrinkSize { get { return shrinkSize; } set { ShrinkSize = value; } }

    private const float BUFFERTIME = 0.01f; // So that the plant doesn't get picked up immediately after being dropped

    private int oldBenchNum;
    private int oldSegmentNum;
    private GameObject duplicatePlant;
    private float timeAtDrop = 0f;

    // BUG: make the colliders triggers for all comps while holding
    public override void DoInteraction()
    {
        if (Player.PlayerInstance.CarriedObject == null && Time.unscaledTime - timeAtDrop > BUFFERTIME)
        {
            GetOriginalValues();
            gameObject.layer = ConstantValues.Layers.Default;
            DuplicatePlant();
            GetComponentInParent<BenchSegment>().RemovePlantFromBench();
            HoverInFrontOfPlayer(duplicatePlant);
            ShrinkPlant(duplicatePlant.transform);
            Player.PlayerInstance.CarriedObject = gameObject;
        }
        else
        {
            Debug.Log("Already holding a plant.");
        }
    }

    private void SetRendererAllChildren(GameObject root, bool isEnabled)
    {
        if (root.GetComponent<MeshRenderer>())
        {
            if (root.GetComponent<StemFE>()) // If it's a stem need to check if it's not empty before changing the meshRenderer so it doesn't show the empties
            {
                if (!root.GetComponent<StemFE>().Stem.IsEmpty)
                {
                    root.GetComponent<MeshRenderer>().enabled = isEnabled;
                }
            } 
            else
            {
                root.GetComponent<MeshRenderer>().enabled = isEnabled;
            }
        }
        // Recursion will stop when there are no more children
        foreach (Transform child in root.transform)
        {
            SetRendererAllChildren(child.gameObject, isEnabled);
        }
    }

    private void GetOriginalValues()
    {
        if (GetComponentInParent<Workbench>())
        {
            oldBenchNum = -1; // Using this for the workbench
            oldSegmentNum = -1;
        }
        else
        {
            oldBenchNum = GetComponentInParent<Bench>().BenchNum;
            oldSegmentNum = GetComponentInParent<BenchSegment>().GetBenchSegmentNum();
        }
    }

    private void DuplicatePlant()
    {
        duplicatePlant = Instantiate(gameObject);
        duplicatePlant.GetComponent<PickUpPlant>().enabled = false;
        duplicatePlant.GetComponent<PlantFE>().enabled = false;
        DestroyCollidersAllChildren(duplicatePlant);
        SetRendererAllChildren(gameObject, false); // Make the original plant invisible on the bench
    }

    // For the duplicate plant because the FPS went down like crazy for plants with lots of parts
    private void DestroyCollidersAllChildren(GameObject root)
    {
        if (root.GetComponent<Collider>())
        {
            Destroy(root.GetComponent<Collider>());
        }
        // Recursion will stop when there are no more children
        foreach (Transform child in root.transform)
        {
            DestroyCollidersAllChildren(child.gameObject);
        }
    }

    private void ShrinkPlant(Transform transform)
    {
        transform.localScale *= ShrinkSize;
    }

    private void HoverInFrontOfPlayer(GameObject gameObject)
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.layer = ConstantValues.Layers.Default; // So it won't be interacted with again until dropped
        gameObject.transform.parent = Player.PlayerInstance.transform;
        gameObject.transform.localPosition = distInFrontOfPlayer;
    }

    public void DropPlant(GameObject benchSegment)
    {
        SetPlantValuesToNormal();
        benchSegment.GetComponent<BenchSegment>().SetPlantOnBench(gameObject);
        UpdatePlantSaveLocation(benchSegment);
        Player.PlayerInstance.CarriedObject = null;
        timeAtDrop = Time.unscaledTime;
    }

    private void SetPlantValuesToNormal()
    {
        Destroy(duplicatePlant);
        GetComponent<Rigidbody>().isKinematic = false;
        gameObject.layer = ConstantValues.Layers.Interactable;
        transform.localRotation = Quaternion.identity; // FIXME: This rotates the plant the opposite way when placed sometimes
        SetRendererAllChildren(gameObject, true);
    }

    private void UpdatePlantSaveLocation(GameObject benchSegment)
    {
        int newBenchNum = benchSegment.transform.GetComponentInParent<GreenhouseBench>().BenchNum;
        int newSegmentNum = benchSegment.GetComponent<BenchSegment>().GetBenchSegmentNum();
        if (oldBenchNum == -1) // If it was picked up from the workbench
        {
            GlobalControl.Instance.savedValues.WorktablePlant = null;
        }
        else
        {
            GlobalControl.Instance.savedValues.GreenhousePlants[oldBenchNum][oldSegmentNum] = null;
        }
        GlobalControl.Instance.savedValues.GreenhousePlants[newBenchNum][newSegmentNum] = GetComponent<PlantFE>().Plant;
    }
}

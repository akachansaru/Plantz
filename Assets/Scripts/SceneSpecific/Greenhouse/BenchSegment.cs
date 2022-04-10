using UnityEngine;

[RequireComponent(typeof(HighlightOnMouseEnter))]
public class BenchSegment : MonoBehaviour
{
    //[SerializeField] private Color highlightColor = new Color(195, 127, 95);
    [SerializeField] private bool isOccupied = false;

    public bool IsOccupied { get { return isOccupied; } private set { isOccupied = value; } }

    //private MeshRenderer mRenderer;
    //private Color originalColor;

    //public virtual void Start()
    //{
    //    mRenderer = GetComponent<MeshRenderer>();
    //    originalColor = mRenderer.material.color;
    //}

    public int GetBenchSegmentNum()
    {
        return GetComponentInParent<Bench>().BenchSegments.FindIndex(gameObject.Equals);
    }

    public void RemovePlantFromBench()
    {
        IsOccupied = false;
    }

    public void SetPlantOnBench(GameObject plantGO)
    {
        plantGO.transform.parent = transform;
        plantGO.transform.localPosition = GetPositionOnBench(transform, plantGO.GetComponent<PlantFE>().Plant);
        IsOccupied = true;
    }

    private Vector3 GetPositionOnBench(Transform benchT, Plant plant)
    {
        return new Vector3(0, (benchT.localScale.y + plant.Pot.PotSize.y) / 2, 0);
    }

    //public void OnMouseEnter()
    //{
    //    if (Vector3.Distance(Player.PlayerInstance.transform.position, transform.position) < Player.PlayerInstance.interactionSettings.InteractDist)
    //    {
    //        mRenderer.material.color = highlightColor;
    //    }
    //}

    // Put the carried plant onto the bench if the player hits the right key while looking at the bench segment
    public void OnMouseOver()
    {
        if (Input.GetKeyDown(Player.PlayerInstance.interactionSettings.InteractKey))
        {
            if (Player.PlayerInstance.CarriedObject != null && !GetComponent<EnterWorktable>() && !IsOccupied)
            {
                Player.PlayerInstance.CarriedObject.GetComponent<PickUpPlantECS>().DropPlant(gameObject);
            }
        }
    }

    //public void OnMouseExit()
    //{
    //    mRenderer.material.color = originalColor;
    //}
}

using UnityEngine;
using UnityEngine.EventSystems;

// Attach to object to drag it. Dropping behavior is handled by derived scripts
public class MoveObjectOrUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
{
    [SerializeField] protected Color highlightColor;
    [Tooltip("If this is true the object will stay where it was dropped, otherwise it will go back to it's start position.")]
    [SerializeField] private bool leaveOnDrop = false;

    protected static bool dragging;
    protected bool selected;
    protected Vector3 originalPosition;

    protected Color originalColor;
    Vector3 screenPoint;
    Vector3 offset;


    public virtual void Start()
    {
        originalPosition = transform.position;
        dragging = false;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        //if (!dragging)
        //{
        //    mRenderer.material.color = highlightColor;
        //}
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //if (!dragging)
        //{
        //    mRenderer.material.color = originalColor;
        //}
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        selected = true;
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
        transform.position = cursorPosition;
    }

    // Doesn't seem to work well. Using OnEndDrag instead
    public virtual void OnDrop(PointerEventData eventData)
    {
        //dragging = false;
        //selected = false;
        //RaycastHit hitInfo;
        //if (Physics.Raycast(transform.position, Vector3.forward, out hitInfo, 5, 1<<8)) {
        //    Debug.Log("hit worktable");
        //    SnapToWorktable(hitInfo);
        //}
        //if (touching) {
        // Debug.Log("dropped on " + touching);
        //GetComponent<PlantComponent>().GraftTo(touching.GetComponent<PlantComponent>());
        //}
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (leaveOnDrop)
        {
            originalPosition = transform.position;
        }
    }
}
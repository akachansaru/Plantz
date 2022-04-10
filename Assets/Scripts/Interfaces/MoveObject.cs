using UnityEngine;
using UnityEngine.EventSystems;

// Attach to object to drag it. Dropping behavior is handled by derived scripts
public class MoveObject : MoveObjectOrUI
{
    MeshRenderer mRenderer;

    public override void Start()
    {
        base.Start();
        mRenderer = GetComponent<MeshRenderer>();
        originalColor = mRenderer.material.color;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (!dragging)
        {
            mRenderer.material.color = highlightColor;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (!dragging)
        {
            mRenderer.material.color = originalColor;
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
    }

    // Doesn't seem to work well. Using OnEndDrag instead
    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
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

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MoveType { Pot, Plant, Soil }

/// <summary>
/// Attached to plants or pots in the worktable. Snaps it into place when dropped over the worktable
/// </summary>
public class MovePlant : MoveObject
{
    [SerializeField] private MoveType moveType = MoveType.Pot;

    public MoveType MoveType { get { return moveType; } }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        dragging = false;
        selected = false;
        DragToWorktable();
    }

    private void DragToWorktable()
    {
        if (Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hitInfo, Mathf.Infinity, ConstantValues.LayerMasks.Worktable))
        {
            transform.position = hitInfo.collider.GetComponent<Worktable>().SnapToWorktable(gameObject, originalPosition);
        }
        else
        {
            transform.position = originalPosition;
        }
    }
}

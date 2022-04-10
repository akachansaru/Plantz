using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public GameObject SelectedItem { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectedItem = gameObject;
    }
}

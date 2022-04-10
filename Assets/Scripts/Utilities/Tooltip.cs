using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This needs to go on the GameObject that you want the tooltip to display for
public class Tooltip : MonoBehaviour
{
    private Text tooltipText;

    private void OnEnable()
    {
        tooltipText = GetComponent<Text>();
    }

    // Use OnPointerEnter on the object you want the tooltip to appear on
    public void ShowTooltip(string tooltip)
    {
        gameObject.SetActive(true);
        tooltipText.text = tooltip;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}

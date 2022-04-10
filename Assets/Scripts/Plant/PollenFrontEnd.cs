using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PollenFrontEnd : InventoryItem, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Pollen pollen;
    [SerializeField] private Tooltip tooltip = null;

    public Pollen Pollen { get { return pollen; } set { pollen = value; } }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Pollen.IsLoaded = false;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        Assert.IsNotNull(tooltip);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.ShowTooltip(Pollen.Taxonomy.ToString());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
    }
}

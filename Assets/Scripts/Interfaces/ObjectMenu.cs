using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class ObjectMenu : MonoBehaviour
{
    [SerializeField] private GameObject canvas;

    protected GameObject Canvas { get { return canvas; } private set { canvas = value; } }

    public virtual void Awake()
    {
        Assert.IsNotNull(canvas);
    }

    public virtual void Start()
    {
        if (gameObject.layer != ConstantValues.Layers.Interactable)
        {
            gameObject.layer = ConstantValues.Layers.Interactable;
            Debug.LogWarning("Changed layer to Interactable on " + gameObject.name);
        }
    }

    public virtual void Update()
    {
        if (Input.GetKeyDown(Player.PlayerInstance.interactionSettings.ObjectCloseMenuKey))
        {
            CloseMenu();
        }
    }

    public virtual void OpenMenu()
    {
        SetUpCanvas();
        Player.PlayerInstance.GivePlayerControl(false);
    }

    private void SetUpCanvas()
    {
        canvas.GetComponent<Canvas>().worldCamera = Camera.main;
        canvas.SetActive(true);
    }

    public virtual void CloseMenu()
    {
        ReturnCanvas();
        Player.PlayerInstance.CurrentObjectMenu = null;
        Player.PlayerInstance.GivePlayerControl(true);
    }

    private void ReturnCanvas()
    {
        canvas.SetActive(false);
    }
}

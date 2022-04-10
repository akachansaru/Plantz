using System;
using UnityEngine;

public class Player : UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController
{
    public static Player PlayerInstance;
    public InteractionSettings interactionSettings = new InteractionSettings();

    [SerializeField] private GameObject carriedObject = null;
    private ObjectMenu currentObjectMenu; // Will not be null if the player is currently looking at a menu of a plant etc.

    public InventoryManager InventoryManager { get; private set; }

    public GameObject CarriedObject { get { return carriedObject; } set { carriedObject = value; } }
    public ObjectMenu CurrentObjectMenu { get { return currentObjectMenu; } set { currentObjectMenu = value; } }

    [Serializable]
    public class InteractionSettings
    {
        [SerializeField] private KeyCode inventoryKey = KeyCode.I;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private KeyCode objectOpenMenuKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode objectCloseMenuKey = KeyCode.B;
        [SerializeField] private KeyCode mainMenuKey = KeyCode.Escape;

        [SerializeField] private float interactDist = 3f;

        public KeyCode InventoryKey { get { return inventoryKey; } }
        public KeyCode InteractKey { get { return interactKey; } }
        public KeyCode ObjectOpenMenuKey { get { return objectOpenMenuKey; } }
        public KeyCode ObjectCloseMenuKey { get { return objectCloseMenuKey; } }
        public KeyCode MainMenuKey { get { return mainMenuKey; } }

        public float InteractDist { get { return interactDist; } }
    }

    public override void Start()
    {
        base.Start();
        InventoryManager = GetComponent<InventoryManager>();
        PlayerInstance = this;
    }

    public override void Update()
    {
        base.Update();

        if (!GameTime.GamePaused)
        {
            if (Input.GetKeyDown(interactionSettings.InventoryKey))
            {
                InventoryManager.ToggleInventory();
            }

            if (Input.GetKeyDown(interactionSettings.InteractKey))
            {
                Interact();
            }

            if (Input.GetKeyDown(interactionSettings.ObjectOpenMenuKey))
            {
                OpenObjectMenu();
            }

        }
        if (Input.GetKeyDown(interactionSettings.MainMenuKey))
        {
            Settings.instance.ToggleSettings();
        }
    }

    public void GivePlayerControl(bool hasControl)
    {
        HasPlayerControl = hasControl;
        ShowCursor(!hasControl);
    }

    private void Interact()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, interactionSettings.InteractDist, ConstantValues.LayerMasks.Interactable))
        {
            Interactable interactable = hit.collider.gameObject.GetComponentInParent<Interactable>(); // FIXME: This could be sketchy using GetCompInParent. Will need to make sure that only one interactiable script is on each hierarchy
            if (interactable)
            {
                Debug.Log("interacting with " + hit.collider.name);
                interactable.DoInteraction(); // Could feed in gameObject and see if it's something that should be able to call the interactable to avoid issues with GetCompInParent
            }
            else
            {
                Debug.LogWarning("No Interactable component on " + hit.collider.name);
            }
        }
    }

    private void OpenObjectMenu()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, interactionSettings.InteractDist, ConstantValues.LayerMasks.Interactable))
        {
            CurrentObjectMenu = hit.collider.gameObject.GetComponent<ObjectMenu>();
            if (CurrentObjectMenu)
            {
                CurrentObjectMenu.OpenMenu();
            }
            else
            {
                Debug.LogWarning("No ObjectMenu component on " + hit.collider.name);
            }
        }
    }

    private void ShowCursor(bool isVisible)
    {
        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = isVisible;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
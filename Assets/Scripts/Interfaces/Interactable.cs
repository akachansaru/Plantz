using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Interactable : MonoBehaviour
{
    public virtual void Start()
    {
        if (gameObject.layer != ConstantValues.Layers.Interactable)
        {
            gameObject.layer = ConstantValues.Layers.Interactable;
            Debug.LogWarning("Changed layer to Interactable on " + gameObject.name);
        }
    }

    public abstract void DoInteraction();
}

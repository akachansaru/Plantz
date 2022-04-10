using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : Interactable
{
    public override void DoInteraction()
    {
        Destroy(gameObject);
    }
}

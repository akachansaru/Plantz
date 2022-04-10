using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WindArea : MonoBehaviour
{
    [SerializeField] Vector3 direction = Vector3.zero;
    [SerializeField] float strength = 1f;

    public Vector3 Direction { get { return direction; } set { direction = value; } }
    public float Strength { get { return strength; } set { strength = value; } }

    void Start()
    {
        // The layer needs to be Ground like the terrain so the plants won't detect it when generating
        if (gameObject.layer != ConstantValues.Layers.Ground)
        {
            gameObject.layer = ConstantValues.Layers.Ground;
            Debug.LogWarning("Changed windArea layer to Ground.");
        }

        Vector3 biomeEdges = GetComponentInParent<Terrain>().terrainData.size;
        GetComponent<BoxCollider>().size = biomeEdges;
        transform.localPosition = biomeEdges / 2;
    }

}

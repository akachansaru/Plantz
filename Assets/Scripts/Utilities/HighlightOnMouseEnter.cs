using UnityEngine;
using System.Collections;

public class HighlightOnMouseEnter : MonoBehaviour
{
    [SerializeField] private Color highlightColor = new Color(195, 127, 95);

    private MeshRenderer mRenderer;
    private Color originalColor;

    public virtual void Start()
    {
        mRenderer = GetComponent<MeshRenderer>();
        originalColor = mRenderer.material.color;
    }

    public void OnMouseEnter()
    {
        if (Vector3.Distance(Camera.main.transform.position, transform.position) < Player.PlayerInstance.interactionSettings.InteractDist)
        {
            mRenderer.material.color = highlightColor;
        }
    }

    public void OnMouseExit()
    {
        mRenderer.material.color = originalColor;
    }
}

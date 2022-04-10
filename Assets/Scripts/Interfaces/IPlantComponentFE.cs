using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IPlantComponentFE<T> : MonoBehaviour where T : IPlantComponent
{
    [SerializeField] protected T comp;

    public virtual T Comp { get { return comp; } set { comp = value; } }

    public void Grow(float maxSize, Vector3 prefabScale)
    {
        float xScale = transform.localScale.x;
        if (xScale < maxSize)
        {
            if (xScale + ConstantValues.PlantConsts.GrowthAmt < maxSize)
            {
                ChangeSize(gameObject, ConstantValues.PlantConsts.GrowthAmt, prefabScale, this);
            }
            else // Add the extra on to get it to exactly the max size
            {
                ChangeSize(gameObject, maxSize - xScale, prefabScale, this);
                Comp.Mature();
            }
        }
    }

    public virtual void Drop(List<T> list)
    {
        if (transform.parent.GetComponent<StemFE>().Stem.Branch.YearGrown != GlobalControl.Instance.savedValues.Year)
        {
            list.Remove(Comp);
        }
        Comp.IsDropped = true;
        transform.parent = null;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().isTrigger = false;
    }

    // TODO: This is ugly. See if I can consolidate it with something like StartFruitingFE in FlowerFE
    public virtual GameObject SetupCompGO(Transform emptyT, Stem stem, int num, string typeCode, List<T> listInStem, Vector3 initialSize, Vector3Serializable[] positions)
    {
        gameObject.name = Comp.CreateID(stem.StemID, num, typeCode);

        SetParent(emptyT);
        SetScale(initialSize);
        SetPosition(CalculatePosition(positions[num], gameObject.transform.localScale));
        // FIXME (potential): Only using the x comp of the localScale since that's the only length that matters with current models

        SetRotation(CalculateRotation(positions[num]));

        listInStem.Add(Comp);
        return gameObject;
    }

    protected Vector3 CalculatePosition(Vector3Serializable position, Vector3 scale)
    {
        return new Vector3(position.x + (-.5f + (scale.x / 2)) * position.ToVector3().normalized.x,
            position.y + (-.5f + (scale.x / 2)) * position.ToVector3().normalized.y,
            position.z + (-.5f + (scale.x / 2)) * position.ToVector3().normalized.z);
    }

    // Returns euler rotations
    private Vector3 CalculateRotation(Vector3Serializable position)
    {
        // No rotation for x, around y and z for y, around y for z
        Vector3 rotation = Vector3.zero;
        if (position.x < 0)
        {
            rotation = new Vector3(0, 180, 0);
        }
        else if (position.y < 0)
        {
            rotation = new Vector3(0, 0, -90);
        }
        else if (position.y > 0)
        {
            rotation = new Vector3(0, 0, 90);
        }
        else if (position.z < 0)
        {
            rotation = new Vector3(0, 90, 0);
        }
        else if (position.z > 0)
        {
            rotation = new Vector3(0, -90, 0);
        }
        return rotation;
    }

    // For loading dropped or undropped comps
    public GameObject Load(T comp, bool loadDropped, List<GameObject> gameObjects)
    {
        if (!loadDropped)
        {
            if (!comp.IsDropped)
            {
                return LoadHelper(comp, gameObjects);
            }
            else
            {
                //Debug.LogWarning("Comp " + comp.GetID() + " is dropped. Did not load.");
                return null;
            }
        }
        else
        {
            if (comp.IsDropped)
            {
                return LoadHelper(comp, gameObjects);
            }
            else
            {
                // Debug.LogWarning("Comp " + comp.GetID() + " is not dropped. Did not load.");
                return null;
            }
        }
    }

    private GameObject LoadHelper(T comp, List<GameObject> gameObjects)
    {
        Comp = comp;
        gameObject.name = Comp.GetID();
        gameObject.transform.SetParent(GameObject.Find(Comp.Parent).transform);
        gameObject.transform.localPosition = Comp.LocalPosition.ToVector3();
        gameObject.transform.localScale = Comp.LocalScale.ToVector3();
        Quaternion rotation = Quaternion.identity;
        rotation.eulerAngles = Comp.LocalRotation.ToVector3();
        gameObject.transform.localRotation = rotation;

        gameObjects.Add(gameObject);

        return gameObject;
    }


    /// <summary>
    /// Adjust the size and move it the appropriate amount to stay on the edge of the branch.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="growthAmt"></param>
    /// <param name="prefabScale"></param>
    /// <param name="plantComponentFE"></param>
    public static void ChangeSize(GameObject gameObject, float growthAmt, Vector3 prefabScale, IPlantComponentFE<T> plantComponentFE)
    {
        Vector3 initialScale = gameObject.transform.localScale;

        // divided by x to keep growthAmt.x = ConstantValues.PlantConsts.GrowthAmt
        plantComponentFE.SetScale(gameObject.transform.localScale + growthAmt * prefabScale / prefabScale.x);

        Vector3 finalScale = gameObject.transform.localScale;

        // Move the comp so it are still attached to the plant
        Vector3 sign = plantComponentFE.Comp.LocalPosition.ToVector3().normalized;
        Vector3 deltaScale = finalScale - initialScale;

        // Only using the x comp of the deltaScale since that's the only length that matters with current leaf model
        plantComponentFE.SetPosition(gameObject.transform.localPosition + sign * (deltaScale.x / 2));
    }

    public void SetParent(Transform parent)
    {
        transform.parent = parent;
        Comp.Parent = parent.name;
    }

    public void SetPosition(Vector3 localPosition)
    {
        transform.localPosition = localPosition;
        Comp.LocalPosition = new Vector3Serializable(localPosition);
    }

    public void SetScale(Vector3 localScale)
    {
        transform.localScale = localScale;
        Comp.LocalScale = new Vector3Serializable(localScale);
    }

    public void SetRotation(Vector3 localRotation)
    {
        Quaternion quat = Quaternion.identity;
        quat.eulerAngles = localRotation;
        transform.localRotation = quat;
        Comp.LocalRotation = new Vector3Serializable(transform.localRotation.eulerAngles);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(ConstantValues.Tags.Boundary)) // Boundary underneath the terrain so the comp doesn't fall forever
        {
            Destroy(gameObject);
        }
    }
}

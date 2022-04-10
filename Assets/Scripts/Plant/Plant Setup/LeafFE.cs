using System.Collections.Generic;
using UnityEngine;

public class LeafFE : IPlantComponentFE<Leaf>
{
    //public override GameObject SetupCompGO(Transform emptyT, Stem stem, int num, string typeCode, List<Leaf> listInStem, Vector3 initialSize, Vector3Serializable[] positions)
    //{
    //    Comp.OriginalPosition = new Vector3Serializable(CalculatePosition(positions[num], initialSize));
    //    return base.SetupCompGO(emptyT, stem, num, typeCode, listInStem, initialSize, positions);
    //}

    public  GameObject LoadDroppedLeaf(Leaf leaf, GameObject plantGO)
    {
        if (leaf.IsDropped)
        {
            //GameObject newLeafGO = LoadHelper(leaf, plantGO, ConstantValues.Prefabs.Leaf, plantGO.GetComponent<PlantFE>().LeafGOs);
            //GameObject newLeafGO = Instantiate(Resources.Load(ConstantValues.Prefabs.Leaf)) as GameObject;
            gameObject.GetComponent<LeafFE>().Load(leaf, true, plantGO.GetComponent<PlantFE>().LeafGOs);

            ChangeSize(gameObject,
                -(gameObject.transform.localScale.x - plantGO.GetComponent<PlantFE>().Plant.Taxonomy.Species.Genus.LeafInitialSize.x),
                ConstantValues.PlantConsts.LeafPrefabScale.ToVector3(),
                gameObject.GetComponent<LeafFE>());
            return gameObject;
        }
        else
        {
            Debug.LogError("Leaf is not dropped. Returning null.");
            return null;
        }
    }
}
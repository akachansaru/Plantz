using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTest : MonoBehaviour
{
    public void Start()
    {
        TreePrototype[] treePrototypes = GetComponent<Terrain>().terrainData.treePrototypes;
        TreeInstance[] treeInstances = GetComponent<Terrain>().terrainData.treeInstances;

        List<TreeInstance> keepTrees = new List<TreeInstance>();
        foreach (TreeInstance treeInstance in treeInstances)
        {
            int index = treeInstance.prototypeIndex;
            if (index == 0)
            {
                keepTrees.Add(treeInstance);

            }
            else
            {
                Vector3 worldTreePos = Vector3.Scale(treeInstance.position, GetComponent<Terrain>().terrainData.size) + transform.position;
                GameObject newPlant = Instantiate(Resources.Load(ConstantValues.Prefabs.Plant), worldTreePos, Quaternion.identity, transform) as GameObject; // Create a prefab tree on its pos
            }
            Debug.Log(index);
        }
        GetComponent<Terrain>().terrainData.treeInstances = keepTrees.ToArray();
    }
}

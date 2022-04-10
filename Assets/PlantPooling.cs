using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPooling : MonoBehaviour
{
    public int numAvailable = 0;
    public List<GameObject> pooledPlants;
    public int nextAvailable = 0; // The index of the next plant that hasn't been used

    //private void Start()
    //{
    //    //numAvailable = transform.childCount;
    //    //pooledPlants = new List<GameObject>(numAvailable);

    //    //foreach (Transform child in transform)
    //    //{
    //    //    pooledPlants.Add(child.gameObject);
    //    //}
    //}

    public GameObject GetNextAvailable()
    {
        GameObject next = pooledPlants[nextAvailable];
        nextAvailable++;
        next.SetActive(true);
        return next;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FlowerFE : IPlantComponentFE<Flower>
{
    public GameObject pollenGlow;

    public void Start()
    {
        Assert.IsNotNull(pollenGlow);
    }

    public void Update()
    {
        if (Comp.HasPollen && !pollenGlow.activeSelf)
        {
            pollenGlow.SetActive(true);
        }

        if (!Comp.HasPollen && pollenGlow.activeSelf)
        {
            pollenGlow.SetActive(false);
        }
    }

    // This is is I want to generate the front end and back end at the same time
    public void StartFruitingFE(PlantFE plantFE, StemFE stemFE)
    {
        Fruit newFruit = Comp.StartFruiting(plantFE.Plant, stemFE.Stem);
        GameObject newFruitGO = Instantiate(Resources.Load(ConstantValues.Prefabs.Fruit)) as GameObject;
        newFruitGO.GetComponent<FruitFE>().Comp = newFruit;
        newFruitGO.name = newFruit.GetID();

        newFruitGO.transform.SetParent(stemFE.transform);
        newFruitGO.transform.localScale = newFruit.LocalScale.ToVector3();
        newFruitGO.transform.localRotation = Quaternion.Euler(newFruit.LocalRotation.ToVector3());
        newFruitGO.transform.localPosition = newFruit.LocalPosition.ToVector3();

        plantFE.FruitGOs.Add(newFruitGO);
    }
}

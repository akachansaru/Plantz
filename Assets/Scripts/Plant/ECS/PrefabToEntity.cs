using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// Add here all of the prefabs that need to be converted to entities
public class PrefabToEntity : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public static Entity stemEntity;
    public static Entity leafEntity;
    public static Entity flowerEntity;
    public static Entity fruitEntity;

    public GameObject stemPrefab;
    public GameObject leafPrefab;
    public GameObject flowerPrefab;
    public GameObject fruitPrefab;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(stemPrefab);
        referencedPrefabs.Add(leafPrefab);
        referencedPrefabs.Add(flowerPrefab);
        referencedPrefabs.Add(fruitPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        stemEntity = conversionSystem.GetPrimaryEntity(stemPrefab);
        leafEntity = conversionSystem.GetPrimaryEntity(leafPrefab);
        flowerEntity = conversionSystem.GetPrimaryEntity(flowerPrefab);
        flowerEntity = conversionSystem.GetPrimaryEntity(fruitPrefab);
    }
}

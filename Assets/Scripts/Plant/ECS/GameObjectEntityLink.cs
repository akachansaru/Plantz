using Unity.Entities;
using UnityEngine;

public class GameObjectEntityLink : MonoBehaviour
{
    public Entity BaseEntity { get; set; }

    private EntityManager entityManager;

    public void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    /// <summary>
    /// For use with the UI button in the plant menu
    /// </summary>
    public void ButtonCollectPollen()
    {
        // Add the CollectingPollenTag so that CollectPollenSystem knows which plant has been selected
        entityManager.AddComponent<CollectingPollenTag>(BaseEntity);
    }

}

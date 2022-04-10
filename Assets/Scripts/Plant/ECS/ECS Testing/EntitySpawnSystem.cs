using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class EntitySpawnSystem : ComponentSystem // Make a system to spawn leaves for the NumLeavesComponent number saved on StemComponents?
{
    public int maxSpawn = 0;
    public int numSpawned = 0;

    //private EntityArchetype leafArchetype;

    //protected override void OnCreate()
    //{
    //    base.OnStartRunning();
    //    leafArchetype = EntityManager.CreateArchetype(
    //           typeof(IDComponent),
    //           typeof(Translation),
    //           typeof(Scale),
    //           typeof(Rotation)
    //           );
    //}

    //private string ConvertIDToString(IDComponent id)
    //{
    //    return id.ID + id.num.ToString();
    //}

    protected override void OnUpdate()
    {
        //if (numSpawned < maxSpawn)
        //{
        //    Entity stemEntity = EntityManager.Instantiate(PrefabToEntity.stemEntity);
        //    IDComponent stemID = new IDComponent { ID = 'S', num = numSpawned };
        //    EntityManager.AddComponentData(stemEntity, stemID);
        //    NumLeavesComponent numLeaves = new NumLeavesComponent { numLeaves = 2 };
        //    EntityManager.AddComponentData(stemEntity, numLeaves); // This would need to come from taxonomy/what step the branch is in
        //    EntityManager.SetComponentData(stemEntity, new Translation { Value = new float3(0, numSpawned, 0) });
        //    EntityManager.SetName(stemEntity, ConvertIDToString(stemID));

        //    for (int i = 0; i < numLeaves.numLeaves; i++)
        //    {
        //        Entity leafEntity = EntityManager.Instantiate(PrefabToEntity.leafEntity);
        //        EntityManager.AddComponentData(leafEntity, new IDComponent { ID = 'L', num = numSpawned });

        //        EntityManager.SetComponentData(leafEntity, new Translation { Value = new float3(-1, 0, 0) });
        //        EntityManager.AddComponent(leafEntity, typeof(LocalToParent)); // This makes the Translation and scale relative to the Parent
        //        EntityManager.AddComponentData(leafEntity, new Parent { Value = stemEntity });

        //        EntityManager.SetName(leafEntity, 'L' + numSpawned.ToString());
        //    }
        //    numSpawned++;
        //}
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//public class LeafAuthoring : MonoBehaviour, IConvertGameObjectToEntity
//{

//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//        //IDComponent id = new IDComponent { ID = 'L', num = 0 };
//        //dstManager.AddComponentData(entity, id);
//        dstManager.AddComponentData(entity, new Translation { Value = new float3(0, 5, 0) });
//        //dstManager.AddComponentData(entity, new Parent { Value = })
//        dstManager.AddComponentData(entity, new TestComponent { num = 999 });
//        //dstManager.SetName(entity, id.ID.ToString() + id.num.ToString()); // Doesn't seem to work
//    }
//}

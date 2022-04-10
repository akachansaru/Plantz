using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//public class StemAuthoring : MonoBehaviour, IConvertGameObjectToEntity
//{

//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//        dstManager.AddComponentData(entity, new Translation { Value = new float3(0, 0, 0) });
//        dstManager.AddComponentData(entity, new BranchComponent { CurrStep = -1 });
//        dstManager.AddComponent(entity, typeof(StemLeafComponent));

//        //dstManager.AddComponentData(entity, new Parent { Value = })

//        dstManager.AddComponentData(entity, new TestComponent { num = 10 });

//        //dstManager.SetName(entity, id.ID.ToString() + id.num.ToString()); // Doesn't seem to work
//        //IDComponent id = new IDComponent { ID = 'S', num = 0 };
//        //dstManager.AddComponentData(entity, id);

//    }
//}

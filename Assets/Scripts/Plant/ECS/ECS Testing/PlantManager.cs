using System.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

namespace Assets.Scripts.Plant
{
    public class PlantManager : MonoBehaviour
    {
        [SerializeField] Mesh leafMesh;
        [SerializeField] Material leafMaterial;

        private void Start()
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityArchetype leafArchetype = entityManager.CreateArchetype(
                //typeof(IDComponent),
                typeof(Translation),
                typeof(Scale),
                typeof(Rotation),
                typeof(RenderMesh),
                typeof(LocalToWorld),
                typeof(RenderBounds),
                typeof(AmbientProbeTag),
                typeof(PerInstanceCullingTag),
                typeof(WorldToLocal_Tag)
                );

            NativeArray<Entity> leafArray = new NativeArray<Entity>(3, Allocator.Temp);
                        
            entityManager.CreateEntity(leafArchetype, leafArray);

            for (int i = 0; i < leafArray.Length; i++)
            {
                Entity leaf = leafArray[i];
               // IDComponent id = new IDComponent { ID = 'L', num = i };
                //entityManager.SetComponentData(leaf, id);
                //entityManager.SetComponentData(leaf, new Translation { Value = new float3(0, i, 0) });
                //entityManager.SetName(leaf, id.ID.ToString() + id.num.ToString());
                entityManager.SetComponentData(leaf, new Scale { Value = 1f });
                entityManager.SetComponentData(leaf, new Rotation { Value = quaternion.identity });
                entityManager.SetComponentData(leaf, new RenderBounds { Value = new AABB { Center = new float3(0, 0, 0), 
                                                                                           Extents = new float3(0.5f, 0.5f, 0.5f) }});
                entityManager.SetSharedComponentData(leaf, new RenderMesh {mesh = leafMesh, material = leafMaterial, 
                    castShadows = UnityEngine.Rendering.ShadowCastingMode.On, receiveShadows = true,});
            }

            leafArray.Dispose();
        }
    }
}
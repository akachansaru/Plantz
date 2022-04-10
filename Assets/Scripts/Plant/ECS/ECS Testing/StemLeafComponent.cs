using Unity.Entities;

public struct StemLeafComponent : IComponentData
{
    public Entity leaf;
    //public DynamicBuffer<Entity> leaves; // Need to look into this

}
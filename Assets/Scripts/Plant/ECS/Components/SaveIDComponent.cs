using Unity.Entities;

// Only added when saving. 
public struct SaveIDComponent : IComponentData
{
    public int Parent;
    public int Self;
    public int Root;
    public bool IsEmpty;
}

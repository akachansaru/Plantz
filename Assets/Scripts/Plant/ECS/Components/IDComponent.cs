using Unity.Collections;
using Unity.Entities;

public struct IDComponent : IComponentData
{
    public char ID;
    public int num;
}

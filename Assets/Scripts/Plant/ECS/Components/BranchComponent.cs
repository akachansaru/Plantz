using Unity.Entities;
using Unity.Mathematics;

public struct BranchComponent : IComponentData
{
    public int CurrStep;
    public BranchPatterns BranchPattern;
}

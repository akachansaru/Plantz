using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

// *** This contains all of the components with stats that are dictated by the genus. I think all of these are on the base empty ***
public struct GenusName : IComponentData
{
    public int Value;
}
public struct LeavesPerNodeComp : IComponentData
{
    public int Value;
}

public struct NodesBetweenLeavesComp : IComponentData
{
    public int Value;
}

public struct LeafInitialSizeComp : IComponentData
{
    public StatECS Value;
}

public struct LeafMaxSizeComp : IComponentData
{
    public StatECS Value;
}

// This goes on each leaf
public struct IsLeafMaxSizeComp : IComponentData
{
    public bool Value;
}

public struct WidenLeafTimeComponent : IComponentData
{
    public StatECS Value;
    public float CurrTime;
}

public struct FlowersPerNodeComp : IComponentData
{
    public int Value;
}

public struct NodesBetweenFlowersComp : IComponentData
{
    public int Value;
}

public struct FlowerInitialSizeComp : IComponentData
{
    public StatECS Value;
}

public struct FlowerMaxSizeComp : IComponentData
{
    public StatECS Value;
}

// This goes on each flower
public struct IsFlowerMaxSizeComp : IComponentData
{
    public bool Value;
}

public struct WidenFlowerTimeComponent : IComponentData
{
    public StatECS Value;
    public float CurrTime;
}

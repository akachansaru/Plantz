using System;
using Unity.Entities;
using Unity.Mathematics;

// *** This contains all of the components with stats that are dictated by the species. I think all of these are on the base empty ***
public struct SpeciesName : IComponentData
{
    public int Value;
}

public struct FamilyName : IComponentData
{
    public int Value;
}

public struct RarityComponent : IComponentData
{
    public Rarity Value;
}

// Not the same as age. The number of times new stems have grown
public struct NumStepsGrownComp : IComponentData
{
    public int Value;
}

// When CurrTime >= Value it should grow a new stem comp
public struct StemGrowthTimeComponent : IComponentData
{
    public StatECS Value;
    public float CurrTime;
}

public struct MaxPlantSizeComponent : IComponentData
{
    public StatECS Value;
    public bool IsMaxSize;
}

// On each StemComponent. When CurrTime >= WidenTime it should widen
public struct WidenStemTimeComponent : IComponentData
{
    public StatECS Value;
    public float CurrTime;
}

public struct InitialStemWidthComp : IComponentData
{
    public StatECS Value;
}

public struct MaxStemWidthComp : IComponentData
{
    public StatECS Value;
    public bool IsMaxSize;
}

/// <summary>
/// 0.00 to 1.00. The stem will widen Value * InitialStemWidth
/// </summary>
public struct StemWidenPercentComp : IComponentData
{
    public StatECS Value;
}

public struct BranchRotationComponent : IComponentData
{
    public float3 Value; // In degrees
}

public struct TrunkRotationComp : IComponentData
{
    public StatECS Value; // In degrees
}

// Number of stem segments to grow before branching
public struct InternodesComp : IComponentData
{
    public int Value;
}

// Number of branches spiraling/whorling before coming back to the beginning. 4 would be 1 branch every 90 degrees
public struct BranchesPerCycleComp : IComponentData
{
    public int Value;
}

//public struct NativeBiomesComp : IComponentData
//{
//    public List<Biomes> Value;
//}

[InternalBufferCapacity(8)]
[Serializable]
public struct BiomesBufferElement : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator Biomes(BiomesBufferElement e) { return e.Value; }
    public static implicit operator BiomesBufferElement(Biomes e) { return new BiomesBufferElement { Value = e }; }

    // Actual value each buffer element will store.
    public Biomes Value;
}

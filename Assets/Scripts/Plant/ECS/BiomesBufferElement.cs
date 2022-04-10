using Unity.Entities;
using System;

//[InternalBufferCapacity(8)]
//[Serializable]
//public struct BiomesBufferElement : IBufferElementData
//{
//    // These implicit conversions are optional, but can help reduce typing.
//    public static implicit operator Biomes(BiomesBufferElement e) { return e.Value; }
//    public static implicit operator BiomesBufferElement(Biomes e) { return new BiomesBufferElement { Value = e }; }

//    // Actual value each buffer element will store.
//    public Biomes Value;
//}
using Unity.Entities;

// Added to each entity to indicate it needs to have components loaded from save file. Removed once loaded
public struct LoadTag : IComponentData { }

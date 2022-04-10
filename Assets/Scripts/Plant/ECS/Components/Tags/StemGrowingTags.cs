using Unity.Entities;

// These are just on the empties
public struct GrowingStemTag : IComponentData { } // For all stems that haven't grown yet. On the ends. The others here will have this as well
public struct GrowNormalTag : IComponentData { }
public struct GrowBranchTag : IComponentData { }
public struct GrowContinueTag : IComponentData { }

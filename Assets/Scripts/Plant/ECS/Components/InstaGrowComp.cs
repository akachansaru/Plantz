using Unity.Entities;

public struct InstaGrowComp : IComponentData
{
    public int Value;
    public StatECS RealGrowthTime; // The stat from the taxonomy. A placeholder of 0 will be used while insta growing and this will be added when done.
    public StatECS RealWidenTime;
}

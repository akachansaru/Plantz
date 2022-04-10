using Unity.Entities;

// Attach to each plant comp to keep track of its base
public struct RootComponent: IComponentData 
{
    public Entity Value;
}

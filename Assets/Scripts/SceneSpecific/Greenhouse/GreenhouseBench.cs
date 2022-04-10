
/// <summary>
/// For placing plants in greenhouse grid.
/// </summary>
public class GreenhouseBench : Bench
{
    public override void Start()
    {
        saveList = GlobalControl.Instance.savedValues.GreenhousePlants;
        base.Start();
    }
}

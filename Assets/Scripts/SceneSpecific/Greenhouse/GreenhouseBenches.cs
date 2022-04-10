

public class GreenhouseBenches : IBenches
{
    public override void Start()
    {
        saveList = GlobalControl.Instance.savedValues.GreenhousePlants;
        base.Start();
    }
}
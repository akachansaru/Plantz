using System.Collections.Generic;

public class Workbench : Bench
{
    public override void Start()
    {
        saveList = new List<Plant[]> { new Plant[] { GlobalControl.Instance.savedValues.WorktablePlant } };
        base.Start();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotUI : PotFrontEnd
{
    public void OnMouseDown()
    {
        PlantingManager.Instance.SwitchToSoilCanvas(Pot);
    }
}

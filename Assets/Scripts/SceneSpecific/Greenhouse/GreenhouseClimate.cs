using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GreenhouseClimate : MonoBehaviour
{
    [SerializeField] private float currentTemperature = 60f;
    [SerializeField] private float currentHumidity = 30f;

    public float CurrentTemperature { get { return currentTemperature; } private set { currentTemperature = value; } }
    public float CurrentHumidity { get { return currentHumidity; } private set { currentHumidity = value; } }

    public void SetTemperature(float temp)
    {
        currentTemperature = temp;
        // UNDONE: make these go smoothly from old value to new
    }

    public void SetHumidity(float humidity)
    {
        currentHumidity = humidity;
    }
}

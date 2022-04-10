using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ClimateControl : ObjectMenu
{
    [SerializeField] private Slider tempSlider = null;
    [SerializeField] private Slider humiditySlider = null;

    public override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(tempSlider);
        Assert.IsNotNull(humiditySlider);
    }

    public override void Start()
    {
        base.Start();
        tempSlider.value = GetComponentInParent<GreenhouseClimate>().CurrentTemperature;
        humiditySlider.value = GetComponentInParent<GreenhouseClimate>().CurrentHumidity;
    }

    // For the temp slider control
    public void SliderTempControl()
    {
        GetComponentInParent<GreenhouseClimate>().SetTemperature(tempSlider.value);
    }

    public void SliderHumidityControl()
    {
        GetComponentInParent<GreenhouseClimate>().SetHumidity(humiditySlider.value);
    }
}

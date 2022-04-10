using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public bool useMoon = false;

    public CelestialOrbit sun;
    public CelestialOrbit moon;

    public Color nightSkyColor;
    public Color daySkyColor;

    private Material skybox;
    // 808080
    private void Start()
    {
        //skybox = Instantiate(Resources.Load("Skyboxes/Skybox")) as Material;
    }

    void Update()
    {
        //if (useMoon)
        //{
        //    RenderSettings.sun = moon.GetComponent<Light>();
        //    sun.gameObject.SetActive(false);
        //    moon.gameObject.SetActive(true);
        //    RenderSettings.ambientSkyColor = nightSkyColor;
        //    //RenderSettings.skybox.SetColor("_Tint", Color.red);
        //    RenderSettings.skybox.SetColor("_SkyTint", Color.red);
        //}
        //else
        //{
        //    RenderSettings.sun = sun.GetComponent<Light>();
        //    moon.gameObject.SetActive(false);
        //    sun.gameObject.SetActive(true);
        //    RenderSettings.ambientSkyColor = daySkyColor;
        //}
    }
}

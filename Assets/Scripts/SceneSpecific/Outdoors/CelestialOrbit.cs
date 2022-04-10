using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CelestialBodies { Sun, Moon }
public class CelestialOrbit : MonoBehaviour
{
    public CelestialBodies celestialBody;
    private const float DEGREES_PER_MINUTE = 360f / GameTime.DAY;

    private string directionProperty;

    public void Start()
    {
        switch (celestialBody)
        {
            case CelestialBodies.Sun:
                directionProperty = "_SunDirection";
                Shader.SetGlobalVector(directionProperty, transform.forward); // So the sun will follow the sun in the skybox
                Shader.SetGlobalFloat("_SunAngle", transform.rotation.x / 360); // Time in the lerp function to blend the day and night skies
                break;
            case CelestialBodies.Moon:
                directionProperty = "_MoonDirection";
                Shader.SetGlobalVector(directionProperty, transform.forward); // So the moon will follow the moon in the skybox
                break;
            default:
                Debug.LogError("Celestial body not found.");
                directionProperty = "_SunDirection";
                break;
        }
    }

    void Update()
    {
        if (!GameTime.GamePaused)
        {
            float angle = DEGREES_PER_MINUTE * Time.deltaTime;
            transform.Rotate(new Vector3(angle, 0, 0));
            Shader.SetGlobalVector(directionProperty, transform.forward); // So the sun will follow the sun in the skybox

            if (celestialBody == CelestialBodies.Sun)
            {
                // Adjust the input angle to the skybox shader so it alligns with day/night
                Quaternion adjustedAngle = transform.rotation * Quaternion.Euler(-90, 0, 0);
                Shader.SetGlobalFloat("_SunAngle", adjustedAngle.x); // Time in the lerp function to blend the day and night skies
            }
        }
    }
}

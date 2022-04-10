using UnityEngine;

public class EntitySaveTest : MonoBehaviour
{
    public static bool readyToSave;

    private void Start()
    {
        readyToSave = false;
    }

    // Button
    public void OnSave()
    {
        readyToSave = true; // Resets in EntitySaveSystem
    }
}
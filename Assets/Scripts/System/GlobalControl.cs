using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

/// <summary>
/// Add this class to a Game Object to save and load data specified in SaveValues.cs
/// </summary>
/// <returns></returns>
public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;

    public SaveValues savedValues;

    string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("Creating new GlobalControl");
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Updating GlobalControl");
            Destroy(gameObject);
        }
        savePath = Application.persistentDataPath + "/saveValues.sheep";
        Load();
    }

    /// <summary>
    ///  Initialize save values when starting a new game.
    /// </summary>
    public void CreateNewGame()
    {
        savedValues.SaveFile = "";
        savedValues.MusicMuted = false;
        savedValues.MusicVolume = 1f; // Full volume

        SpeciesImplementation implementation = new SpeciesImplementation();
        savedValues.AllSpecies = implementation.GenerateStartingSpeciesList();

        savedValues.EntityPlants = new List<EntityPlant>();

        savedValues.Inventory = new Inventory(100, new List<Pollen> { new Pollen(new Taxonomy(Biomes.Forest)) });
        savedValues.GreenhousePlants = new List<Plant[]>();
        savedValues.ForestPlants = new List<Plant>();
        savedValues.DesertPlants = new List<Plant>();
        savedValues.SwampPlants = new List<Plant>();

        savedValues.Pots = new List<Pot> { new Pot(new Vector3Serializable(1, 1, 1), new Vector4Serializable(0.5f, 1, 0f, 1)),
                                           new Pot(new Vector3Serializable(1, 1, 1), new Vector4Serializable(0.5f, 1, 0f, 1)),
                                           new Pot(new Vector3Serializable(0.5f, 0.5f, 0.5f), new Vector4Serializable(0.9803922f, 0, 0.5568628f, 1)),
                                           new Pot(new Vector3Serializable(0.5f, 0.5f, 0.5f), new Vector4Serializable(0.9803922f, 0, 0.5568628f, 1)),
                                           new Pot(new Vector3Serializable(0.5f, 0.5f, 0.5f), new Vector4Serializable(0.9803922f, 0, 0.5568628f, 1)),
                                           new Pot(new Vector3Serializable(0.5f, 0.5f, 0.5f), new Vector4Serializable(0.9803922f, 0, 0.5568628f, 1)),
                                           new Pot(new Vector3Serializable(0.5f, 0.5f, 0.5f), new Vector4Serializable(0.9803922f, 0, 0.5568628f, 1)),
                                           new Pot(new Vector3Serializable(0.5f, 0.5f, 0.5f), new Vector4Serializable(0.9803922f, 0, 0.5568628f, 1)),
                                           new Pot(new Vector3Serializable(0.5f, 0.5f, 0.5f), new Vector4Serializable(0.9803922f, 0, 0.5568628f, 1))};

        savedValues.Soils = new List<Soil> { new Soil(Biomes.Forest), new Soil(Biomes.Desert), new Soil(Biomes.Swamp) };
        savedValues.GameTime = 0;
        savedValues.Year = 1;
        savedValues.Season = 0;
        savedValues.Day = 1;
        savedValues.Hour = 0;
        savedValues.Minute = 0;

        Debug.Log("New game");
    }

    public void Save()
    {
        savedValues.SaveFile = savePath;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        bf.Serialize(file, savedValues);
        file.Close();
        Debug.Log("Saved to " + savePath);
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            savedValues = (SaveValues)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            CreateNewGame();
        }
    }

    void AskToQuit()
    {
        Application.Quit();
    }

    //public void Update() {
    //if (Input.GetKey(KeyCode.Escape)) {
    //    if (SceneManager.GetActiveScene().name.Equals("MainMenu")) {
    //        // UNDONE Ask if user wants to exit game
    //        AskToQuit();
    //    } else if (SceneManager.GetActiveScene().name.Equals("LevelSelection")) {
    //        SceneManager.LoadScene("MainMenu");
    //    } else if (SceneManager.GetActiveScene().name.Equals("Customization")) {
    //        GlobalControl.Instance.Save(); // Saves the items placed on the pig
    //        SceneManager.LoadScene("LevelSelection");
    //    } else if (SceneManager.GetActiveScene().name.Equals("ItemCreation")) {
    //        SceneManager.LoadScene("Customization");
    //    } else {
    //        // UNDONE Ask if user wants to return to quit level
    //        SceneManager.LoadScene("LevelSelection");
    //    }
    //}
    //  }
}

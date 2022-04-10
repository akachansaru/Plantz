using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

/// <summary>
/// Add anything that needs saving here and GlobalControl.cs will save and load it
/// </summary>
[Serializable]
public class SaveValues
{
    public string SaveFile { get; set; }
    public bool MusicMuted { get; set; }
    public float MusicVolume { get; set; }

    public Inventory Inventory { get; set; }

    public int PlantIDNum { get; set; } = 0; // This will always increment, even if a plant is removed to eliminate the need to adjust the ID num for each new plant
    // TODO: make this throw an error if the number is decreased

    public List<Taxonomy> AllSpecies { get; set; } // All species unlocked including ones created as hybrids

    /// <summary>
    /// Each Plant[] is a bench
    /// </summary>
    public List<Plant[]> GreenhousePlants { get; set; }
    public List<Plant> ForestPlants { get; set; }
    public List<Plant> DesertPlants { get; set; }
    public List<Plant> SwampPlants { get; set; }

    public List<EntityPlant> EntityPlants { get; set; }

    public List<Pot> Pots { get; set; } // Pots available to use
    public List<Soil> Soils { get; set; } // Soil available to use

    public Plant WorktablePlant { get; set; } // The temp place where the plant goes after it's made

    public float GameTime { get; set; }
    public int Year { get; set; }
    public int Season { get; set; }
    public int Day { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
   // public float Second { get; set; }

    public float TimeToFlowerGrowth { get; set; } // TODO make each leaf and stem grow on it's own time - move to Leaf and Stem classes
    public float TimeToLeafGrowth { get; set; } // TODO make each leaf and stem grow on it's own time - move to Leaf and Stem classes
    public float TimeToFruitGrowth { get; set; } // TODO make each leaf and stem grow on it's own time - move to Leaf and Stem classes

}

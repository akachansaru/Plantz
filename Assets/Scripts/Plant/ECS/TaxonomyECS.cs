using System.Collections.Generic;
using Unity.Entities;

// Probably don't need this and could just use Species
public class TaxonomyECS
{
    public SpeciesECS Species { get; private set; }

    public TaxonomyECS(Biomes biome)
    {
        List<int> speciesInBiome = SpeciesECS.GetSpeciesInBiome(biome);
        Species = new SpeciesECS(speciesInBiome[UnityEngine.Random.Range(0, speciesInBiome.Count)]);
    }

    public TaxonomyECS(Entity rootEntity)
    {
        Species = new SpeciesECS(rootEntity);
    }
}

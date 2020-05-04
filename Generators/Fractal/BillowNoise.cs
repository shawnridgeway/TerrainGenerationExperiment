using UnityEngine;

public class BillowNoise : TerrainGenerator{

    private readonly CoherentNoise.Generator _generator;

    public BillowNoise(
        int seed = 0,
        float persistance = 0.5f,
        float frequency = 1 / 240f,
        float lacunarity = 2.17f,
        int octaveCount = 6
    ) {
        _generator = new ThreadSafeGenerator(() => 
            new CoherentNoise.Generation.Fractal.BillowNoise(seed) {
                Frequency = frequency,
                Lacunarity = lacunarity,
                OctaveCount = octaveCount,
                Persistence = persistance
            }
        );
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

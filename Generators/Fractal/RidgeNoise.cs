using UnityEngine;

public class RidgeNoise : TerrainGenerator {

    private readonly CoherentNoise.Generator _generator;

    public RidgeNoise(
        int seed = 0,
        float frequency = 1 / 240f,
        float lacunarity = 2.17f,
        float exponent = 1f,
        float offset = 1f,
        float gain = 2f,
        int octaveCount = 6
    ) {
        _generator = new ThreadSafeGenerator(() => 
            new CoherentNoise.Generation.Fractal.RidgeNoise(seed) {
                Frequency = frequency,
                Lacunarity = lacunarity,
                Exponent = exponent,
                Offset = offset,
                Gain = gain,
                OctaveCount = octaveCount,
            }
        );
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

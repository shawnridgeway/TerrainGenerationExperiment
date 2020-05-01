using UnityEngine;

public class RidgeNoise : TerrainGenerator {

    private readonly CoherentNoise.Generator _generator;

    public RidgeNoise(
        int seed = 0,
        float frequency = 1f,
        float lacunarity = 2f,
        float exponent = 1f,
        float offset = 1f,
        float gain = 2f,
        int octaveCount = 6
    ) {
        _generator =
            new CoherentNoise.Generation.Fractal.RidgeNoise(seed) {
                Frequency = frequency,
                Lacunarity = lacunarity,
                OctaveCount = octaveCount,
                Exponent = exponent,
                Offset = offset,
                Gain = gain,
            };
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

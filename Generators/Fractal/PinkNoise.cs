using UnityEngine;

public class PinkNoise : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public PinkNoise(
        int seed = 0,
        float interval = 100f,
        float persistance = 0.3f,
        float frequency = 0.5f,
        float lacunarity = 1.2f,
        int octaveCount = 6
    ) {

        CoherentNoise.Generation.Fractal.PinkNoise pinkNoise =
            new CoherentNoise.Generation.Fractal.PinkNoise(11) {
                Frequency = frequency,
                Lacunarity = lacunarity,
                OctaveCount = octaveCount,
                Persistence = persistance
            };
        _generator = new CoherentNoise.Generation.Displacement.Scale(pinkNoise, Vector3.one / interval);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

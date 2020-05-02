using UnityEngine;

public class BillowNoise : TerrainGenerator{

    private readonly CoherentNoise.Generator _generator;

    public BillowNoise(
        int seed = 0,
        float interval = 240f,
        float persistance = 0.5f,
        float frequency = 1f,
        float lacunarity = 2.17f,
        int octaveCount = 6
    ) {
        _generator = new ThreadSafeGenerator(() => new CoherentNoise.Generation.Displacement.Scale(
            new CoherentNoise.Generation.Fractal.BillowNoise(seed) {
                Frequency = frequency,
                Lacunarity = lacunarity,
                OctaveCount = octaveCount,
                Persistence = persistance
            },
            Vector3.one / interval
        ));
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

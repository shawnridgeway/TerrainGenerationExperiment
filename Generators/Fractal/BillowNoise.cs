using UnityEngine;

public class BillowNoise : TerrainGenerator{

    private readonly CoherentNoise.Generator _generator;

    public BillowNoise(
        int seed = 0,
        float persistance = 0.4f,
        float frequency = 4f,
        float lacunarity = 2f
        //int octaveCount = 2
    ) {
        CoherentNoise.Generation.Fractal.BillowNoise billowNoise =
            new CoherentNoise.Generation.Fractal.BillowNoise(seed)
            {
                Frequency = frequency,
                //Lacunarity = lacunarity,
                //OctaveCount = octaveCount,
                Persistence = persistance
            };
        _generator = new CoherentNoise.Generation.Displacement.Scale(billowNoise, Vector3.one / 100f);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

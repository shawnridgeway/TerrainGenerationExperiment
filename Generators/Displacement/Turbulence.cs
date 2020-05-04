using UnityEngine;

public class Turbulence : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Turbulence(
        CoherentNoise.Generator source,
        int seed = 0,
        float power = 1f,
        float frequency = 1f,
        int octaveCount = 6
    ) {
        _generator = new CoherentNoise.Generation.Displacement.Turbulence(source, seed) {
            Power = power,
            Frequency = frequency,
            OctaveCount = octaveCount
        };
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

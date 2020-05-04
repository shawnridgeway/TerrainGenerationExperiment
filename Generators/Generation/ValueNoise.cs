using UnityEngine;

public class ValueNoise : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public ValueNoise(
        int seed = 0,
        float frequency = 1 / 100f,
        int period = 0,
        bool use2D = false
    ) {
        _generator = ChooseGenerator(seed, frequency, period, use2D);
    }

    private CoherentNoise.Generator ChooseGenerator(int seed, float frequency, int period, bool use2D) {
        CoherentNoise.Generator valueNoise;
        if (use2D) {
            valueNoise = new SwapYZGenerator(new CoherentNoise.Generation.ValueNoise2D(seed) { Period = period });
        } else {
            valueNoise = new CoherentNoise.Generation.ValueNoise(seed) { Period = period };
        }
        return new CoherentNoise.Generation.Displacement.Scale(valueNoise, Vector3.one * frequency);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

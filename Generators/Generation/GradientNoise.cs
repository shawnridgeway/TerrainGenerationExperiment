using UnityEngine;

public class GradientNoise : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public GradientNoise(
        int seed = 0,
        float frequency = 1 / 100f,
        int period = 0,
        bool use2D = false
    ) {
        _generator = ChooseGenerator(seed, frequency, period, use2D);
    }

    private CoherentNoise.Generator ChooseGenerator(int seed, float frequency, int period, bool use2D) {
        CoherentNoise.Generator gradientNoise;
        if (use2D) {
            gradientNoise = new SwapYZGenerator(new CoherentNoise.Generation.GradientNoise2D(seed) { Period = period });
        } else {
            gradientNoise = new CoherentNoise.Generation.GradientNoise(seed) { Period = period };
        }
        return new CoherentNoise.Generation.Displacement.Scale(gradientNoise, Vector3.one * frequency);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

using UnityEngine;

public class GradientNoise : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public GradientNoise(
        int seed,
        float interval = 100f,
        bool use2D = false
    ) {
        _generator = ChooseGenerator(seed, interval, use2D);
    }

    private CoherentNoise.Generator ChooseGenerator(int seed, float interval, bool use2D) {
        CoherentNoise.Generator gradientNoise;
        if (use2D) {
            gradientNoise = new CoherentNoise.Generation.GradientNoise2D(seed);
        } else {
            gradientNoise = new CoherentNoise.Generation.GradientNoise(seed);
        }
        return new CoherentNoise.Generation.Displacement.Scale(gradientNoise, Vector3.one / interval);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

using UnityEngine;

public class ValueNoise : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public ValueNoise(
        int seed,
        bool use2D = false
    ) {
        _generator = ChooseGenerator(seed, use2D);
    }

    private CoherentNoise.Generator ChooseGenerator(int seed, bool use2D) {
        if (use2D) {
            return new CoherentNoise.Generation.ValueNoise2D(seed);
        }
        return new CoherentNoise.Generation.ValueNoise(seed);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

using UnityEngine;

public class Constant : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Constant(
        float value
    ) {
        _generator = new CoherentNoise.Generation.Constant(value);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

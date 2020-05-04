using UnityEngine;

public class Translate : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Translate(
        CoherentNoise.Generator source,
        Vector3 value
    ) {
        _generator = new CoherentNoise.Generation.Displacement.Translate(source, value);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

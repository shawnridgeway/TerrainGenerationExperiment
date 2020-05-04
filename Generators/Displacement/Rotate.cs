using UnityEngine;

public class Rotate : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Rotate(
        CoherentNoise.Generator source,
        Quaternion value
    ) {
        _generator = new CoherentNoise.Generation.Displacement.Rotate(source, value);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

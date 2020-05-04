using UnityEngine;

public class Scale : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Scale(
        CoherentNoise.Generator source,
        Vector3 value
    ) {
        _generator = new CoherentNoise.Generation.Displacement.Scale(source, value);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

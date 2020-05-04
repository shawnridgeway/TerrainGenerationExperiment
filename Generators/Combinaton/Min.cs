using UnityEngine;

public class Min : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Min(
        TerrainGenerator source1,
        TerrainGenerator source2
    ) {
        _generator = new CoherentNoise.Generation.Combination.Min(source1.GetGenerator(), source2.GetGenerator());
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

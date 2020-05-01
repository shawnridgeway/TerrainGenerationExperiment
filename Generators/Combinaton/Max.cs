using UnityEngine;

public class Max : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Max(
        TerrainGenerator source1,
        TerrainGenerator source2
    ) {
        _generator = new CoherentNoise.Generation.Combination.Max(source1.GetGenerator(), source2.GetGenerator());
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

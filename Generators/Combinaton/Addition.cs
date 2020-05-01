using UnityEngine;

public class Addition : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Addition(
        TerrainGenerator source1,
        TerrainGenerator source2
    ) {
        _generator = new CoherentNoise.Generation.Combination.Add(source1.GetGenerator(), source2.GetGenerator());
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

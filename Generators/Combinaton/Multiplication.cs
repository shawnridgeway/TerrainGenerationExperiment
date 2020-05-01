using UnityEngine;

public class Multiplication : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Multiplication(
        TerrainGenerator source1,
        TerrainGenerator source2
    ) {
        _generator = new CoherentNoise.Generation.Combination.Multiply(source1.GetGenerator(), source2.GetGenerator());
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

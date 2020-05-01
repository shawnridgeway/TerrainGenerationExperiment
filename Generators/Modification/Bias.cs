using UnityEngine;

public class Bias : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Bias(
        TerrainGenerator source,
        float bias
    ) {
        _generator = new CoherentNoise.Generation.Modification.Bias(source.GetGenerator(), bias);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

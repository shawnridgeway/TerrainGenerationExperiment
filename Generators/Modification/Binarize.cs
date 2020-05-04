using UnityEngine;

public class Binarize : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Binarize(
        TerrainGenerator source,
        float threshold = 0
    ) {
        _generator = new CoherentNoise.Generation.Modification.Binarize(source.GetGenerator(), threshold);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

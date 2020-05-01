using UnityEngine;

public class Identity : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Identity(
        TerrainGenerator source
    ) {
        _generator = source.GetGenerator();
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

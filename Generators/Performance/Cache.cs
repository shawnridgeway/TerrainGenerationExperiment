using UnityEngine;

public class Cache : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Cache(
        TerrainGenerator source
    ) {
        _generator = new CoherentNoise.Generation.Cache(source.GetGenerator());
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

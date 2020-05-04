using UnityEngine;
using System;

public class Modify : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Modify(
        TerrainGenerator source,
        Func<float, float> modifier
    ) {
        _generator = new CoherentNoise.Generation.Modification.Modify(source.GetGenerator(), modifier);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

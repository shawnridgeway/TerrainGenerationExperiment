using System;
using UnityEngine;

public class Function : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Function(
        Func<float, float, float, float> function
    ) {
        _generator = new CoherentNoise.Generation.Function(function);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

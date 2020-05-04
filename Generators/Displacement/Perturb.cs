using UnityEngine;
using System;

public class Perturb : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Perturb(
        CoherentNoise.Generator source,
        Func<Vector3, Vector3> perturber
    ) {
        _generator = new CoherentNoise.Generation.Displacement.Perturb(source, perturber);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

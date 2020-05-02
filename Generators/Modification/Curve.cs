using System.Collections.Concurrent;
using UnityEngine;
using System.Threading;

public class Curve : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Curve(TerrainGenerator source, AnimationCurve animationCurve) {
        _generator = new ThreadSafeGenerator(() => {
            AnimationCurve curveClone = new AnimationCurve(animationCurve.keys);
            return new CoherentNoise.Generation.Modification.Curve(source.GetGenerator(), curveClone);
        });
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

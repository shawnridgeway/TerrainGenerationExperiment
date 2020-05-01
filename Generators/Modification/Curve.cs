using System.Collections.Concurrent;
using UnityEngine;
using System.Threading;

public class Curve : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Curve(TerrainGenerator source, AnimationCurve animationCurve) {
        this._generator = new CurveGenerator(source, animationCurve);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

class CurveGenerator : CoherentNoise.Generator {
    private readonly TerrainGenerator source;
    private readonly AnimationCurve originalCurve;
    private readonly ConcurrentDictionary<int, CoherentNoise.Generation.Modification.Curve> generatorsByThread =
        new ConcurrentDictionary<int, CoherentNoise.Generation.Modification.Curve>();

    public CurveGenerator(
        TerrainGenerator source,
        AnimationCurve animationCurve
    ) {
        this.source = source;
        this.originalCurve = animationCurve;
    }

    public override float GetValue(float x, float y, float z) {
        return GetGenerator().GetValue(x, y, z);
    }

    private CoherentNoise.Generator GetGenerator() {
        // Get a separate clone per thread so there are no concurrency issues
        int currentThreadHash = Thread.CurrentThread.GetHashCode();
        CoherentNoise.Generation.Modification.Curve generator;
        bool curveExists = generatorsByThread.TryGetValue(currentThreadHash, out generator);
        if (!curveExists) {
            AnimationCurve curve = new AnimationCurve(originalCurve.keys);
            generator = new CoherentNoise.Generation.Modification.Curve(source.GetGenerator(), curve);
            generatorsByThread.TryAdd(currentThreadHash, generator);
        }
        return generator;
    }
}
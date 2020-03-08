using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modulus : TerrainTransform {
    private readonly TerrainTransform a;
    private readonly ModulusOptions options;

    public Modulus(TerrainTransform a, ModulusOptions options) {
        this.a = a;
        this.options = options;
    }

    protected override float Evaluate(Point point) {
        Point modifiedPoint = point.MapPoint(location => new Vector3(
            MathUtils.CanonicalModulus(location.x, options.interval),
            MathUtils.CanonicalModulus(location.y, options.interval),
            MathUtils.CanonicalModulus(location.z, options.interval)
        ));
        return a.Process(modifiedPoint);
    }

    public override TerrainInformation GetTerrainInformation() {
        return a.GetTerrainInformation();
    }
}

public class ModulusOptions {
    public readonly float interval;

    public ModulusOptions(float interval = 1f) {
        this.interval = interval;
    }
}
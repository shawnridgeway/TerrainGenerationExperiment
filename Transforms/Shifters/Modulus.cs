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
        Point cannonicalPoint = point.MapPoint(location => new Vector3(
            MathUtils.CanonicalModulus(location.x, options.scale.x),
            MathUtils.CanonicalModulus(location.y, options.scale.y),
            MathUtils.CanonicalModulus(location.z, options.scale.z)
        ));
        return a.Process(cannonicalPoint);
    }

    public override TerrainInformation GetTerrainInformation() {
        return a.GetTerrainInformation();
    }
}

public class ModulusOptions {
    public readonly Vector3 scale;

    public ModulusOptions(Vector3 scale = new Vector3()) {
        this.scale = scale;
    }
}
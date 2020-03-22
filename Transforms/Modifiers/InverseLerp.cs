using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseLerp : TerrainTransform {
    private readonly TerrainTransform a;
    private readonly float min;
    private readonly float max;

    public InverseLerp(TerrainTransform a, InverseLerpOptions options = null) {
        this.a = a;
        TerrainInformation aInfo = a.GetTerrainInformation();
        this.min = options != null ? options.lowerBound : aInfo.min;
        this.max = options != null ? options.upperBound : aInfo.max;
    }

    protected override float Evaluate(Point point) {
        float aValue = a.Process(point);
        return Mathf.InverseLerp(min, max, aValue);
    }

    public override TerrainInformation GetTerrainInformation() {
        return new TerrainInformation(0, 1);
    }
}

public class InverseLerpOptions {
    public readonly float lowerBound;
    public readonly float upperBound;

    public InverseLerpOptions(float lowerBound, float upperBound) {
        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
    }
}
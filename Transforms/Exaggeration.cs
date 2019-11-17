using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Exaggerates features
// Sharpness of 0...1: Exaggerates verticals
// Sharpness of 1...n: Exaggerates horizontals
public class Exaggeration : TerrainTransform {
    private readonly TerrainTransform a;
    private readonly ExaggerationOptions options;
    private readonly float deviation;
    private readonly float center;

    public Exaggeration(TerrainTransform a, ExaggerationOptions options) {
        this.a = a;
        this.options = options;
        TerrainInformation aInfo = a.GetTerrainInformation();
        deviation = (aInfo.max - aInfo.min) / 2;
        center = deviation + aInfo.min;
    }

    protected override float Evaluate(Point point) {
        float aValue = a.Process(point);
        return Apply(aValue);
    }

    public override TerrainInformation GetTerrainInformation() {
        TerrainInformation aInfo = a.GetTerrainInformation();
        return new TerrainInformation(aInfo.min, aInfo.max);
    }

    float Apply(float x) {
        return Mathf.Sign(x - center) *
            Mathf.Pow(Mathf.Sign(x - center) * x + -1 * Mathf.Sign(x - center) * center, options.intensity) /
            Mathf.Pow(deviation, options.intensity - 1) +
            center;
    }
}

public class ExaggerationOptions {
    public readonly float intensity;

    public ExaggerationOptions(float intensity = 1) {
        this.intensity = intensity;
    }
}

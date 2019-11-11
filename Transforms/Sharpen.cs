using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sharpen : TerrainTransform {
    private readonly TerrainTransform a;
    private readonly SharpenOptions options;
    private readonly float deviation;
    private readonly float center;

    public Sharpen(TerrainTransform a, SharpenOptions options) {
        this.a = a;
        this.options = options;
        TerrainInformation aInfo = a.GetTerrainInformation();
        deviation = (aInfo.max - aInfo.min) / 2;
        center = deviation + aInfo.min;
    }

    public IEnumerable<float> Process(IEnumerable<Point> points) {
        IEnumerable<float> aValues = a.Process(points);
        foreach (float value in aValues) {
            yield return Evaluate(value);
        }
    }

    public TerrainInformation GetTerrainInformation() {
        TerrainInformation aInfo = a.GetTerrainInformation();
        return new TerrainInformation(aInfo.min, aInfo.max);
    }

    float Evaluate(float x) {
        return Mathf.Sign(x - center) *
            Mathf.Pow(Mathf.Sign(x - center) * x + -1 * Mathf.Sign(x - center) * center, options.sharpness) /
            Mathf.Pow(deviation, options.sharpness - 1) +
            center;
    }
}

public class SharpenOptions {
    public readonly float sharpness;

    public SharpenOptions(float sharpness) {
        this.sharpness = sharpness;
    }
}

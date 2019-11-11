using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: make this flatten instead of sharpen
public class Flatten : TerrainTransform {
    private readonly TerrainTransform a;
    private readonly FlattenOptions options;
    private readonly float deviation;
    private readonly float center;

    public Flatten(TerrainTransform a, FlattenOptions options) {
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
            Mathf.Pow(Mathf.Sign(x - center) * x + -1 * Mathf.Sign(x - center) * center, options.flatness) /
            Mathf.Pow(deviation, options.flatness - 1) +
            center;
    }
}

public class FlattenOptions {
    public readonly float flatness;

    public FlattenOptions(float flatness) {
        this.flatness = flatness;
    }
}

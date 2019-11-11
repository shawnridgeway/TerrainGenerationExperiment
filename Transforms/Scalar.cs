using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scalar : TerrainTransform {
    private readonly TerrainTransform a;
    private readonly ScalarOptions options;

    public Scalar(TerrainTransform a, ScalarOptions options) {
        this.a = a;
        this.options = options;
    }

    public IEnumerable<float> Process(IEnumerable<Point> points) {
        IEnumerable<float> aValues = a.Process(points);
        foreach (float value in aValues) {
            yield return value * options.scalar;
        }
    }

    public TerrainInformation GetTerrainInformation() {
        TerrainInformation aInfo = a.GetTerrainInformation();
        return new TerrainInformation(
            Mathf.Min(aInfo.min * options.scalar, aInfo.max * options.scalar),
            Mathf.Max(aInfo.min * options.scalar, aInfo.max * options.scalar)
        );
    }
}

public class ScalarOptions {
    public readonly float scalar;

    public ScalarOptions(float scalar = 1) {
        this.scalar = scalar;
    }
}
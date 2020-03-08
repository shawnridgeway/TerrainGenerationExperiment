using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainTransform {
    public float Process(Point point) {
        return Evaluate(point);
    }

    public IEnumerable<float> Process(IEnumerable<Point> points) {
        foreach (Point point in points) {
            yield return Evaluate(point);
        }
    }

    public abstract TerrainInformation GetTerrainInformation();

    protected abstract float Evaluate(Point point);
}

public struct TerrainInformation {
    public readonly float min;
    public readonly float max;
    public TerrainInformation(float min, float max) {
        this.min = min;
        this.max = max;
    }
}
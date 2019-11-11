using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TerrainTransform {
    IEnumerable<float> Process(IEnumerable<Point> locations);
    TerrainInformation GetTerrainInformation();
}

public struct TerrainInformation {
    public readonly float min;
    public readonly float max;
    public TerrainInformation(float min, float max) {
        this.min = min;
        this.max = max;
    }
}
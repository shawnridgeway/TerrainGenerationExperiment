using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant : TerrainTransform {
    private readonly ConstantOptions options;

    public Constant(ConstantOptions options) {
        this.options = options;
    }

    public IEnumerable<float> Process(IEnumerable<Point> points) {
        foreach (Point point in points) {
            yield return options.constant;
        }
    }

    public TerrainInformation GetTerrainInformation() {
        return new TerrainInformation(options.constant, options.constant);
    }
}

public class ConstantOptions {
    public readonly float constant;

    public ConstantOptions(float constant = 0) {
        this.constant = constant;
    }
}
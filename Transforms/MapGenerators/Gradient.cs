using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gradient : TerrainTransform {
    private readonly GradientOptions options;

    public Gradient(GradientOptions options) {
        this.options = options;
    }

    protected override float Evaluate(Point point) {
        return options.width;
    }

    public override TerrainInformation GetTerrainInformation() {
        return new TerrainInformation(options.width, options.width);
    }
}

public class GradientOptions {
    public readonly float width;

    public GradientOptions(float width = 100) {
        this.width = width;
    }
}
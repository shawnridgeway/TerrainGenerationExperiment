﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LinearGradient : TerrainTransform {
    private readonly LinearGradientOptions options;

    public LinearGradient(LinearGradientOptions options) {
        this.options = options;
    }

    protected override float Evaluate(Point point) {
        Vector2 diffVector = new Vector2(point.GetLocation().x, point.GetLocation().z) - options.center;
        float linearPosition = diffVector.x * Mathf.Sin(options.rotation) - diffVector.y * Mathf.Cos(options.rotation);
        float scalePosition = (linearPosition / options.scale) + 0.5f; // Add half to account for center offset
        return options.gradientPattern.EvaluateAtInterval(scalePosition);
    }

    public override TerrainInformation GetTerrainInformation() {
        return new TerrainInformation(
            options.gradientPattern.GetMinKey(),
            options.gradientPattern.GetMaxKey()
        );
    }
}

public class LinearGradientOptions {
    public readonly float scale;
    public readonly Vector2 center;
    public readonly float rotation;
    public readonly GradientPattern gradientPattern;

    public LinearGradientOptions(
        float scale = 100f,
        Vector2 center = new Vector2(),
        float rotation = 0f,
        GradientPattern gradientPattern = null
    ) {
        this.scale = scale;
        this.center = center;
        this.rotation = rotation;
        this.gradientPattern = gradientPattern;
        if (this.gradientPattern == null) {
            this.gradientPattern = GradientPattern.Default();
        }
    }
}
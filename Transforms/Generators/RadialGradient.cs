using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RadialGradient : TerrainTransform {
    private readonly RadialGradientOptions options;

    public RadialGradient(RadialGradientOptions options) {
        this.options = options;
    }

    protected override float Evaluate(Point point) {
        Vector2 diffVector = new Vector2(point.GetLocation().x, point.GetLocation().z) - options.center;
        float linearPosition = Mathf.Sqrt(diffVector.x * diffVector.x + diffVector.y * diffVector.y);
        float scalePosition = linearPosition / options.scale;
        return options.gradientPattern.EvaluateAtInterval(scalePosition);
    }

    public override TerrainInformation GetTerrainInformation() {
        return new TerrainInformation(
            options.gradientPattern.GetMinKey(),
            options.gradientPattern.GetMaxKey()
        );
    }
}

public class RadialGradientOptions {
    public readonly float scale;
    public readonly Vector2 center;
    public readonly GradientPattern gradientPattern;

    public RadialGradientOptions(
        float scale = 100f,
        Vector2 center = new Vector2(),
        GradientPattern gradientPattern = null
    ) {
        this.scale = scale;
        this.center = center;
        this.gradientPattern = gradientPattern;
        if (this.gradientPattern == null) {
            this.gradientPattern = GradientPattern.Default();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mask : TerrainTransform {
    private readonly TerrainTransform a;
    private readonly TerrainTransform b;
    private readonly TerrainTransform mask;

    public Mask(TerrainTransform a, TerrainTransform b, MaskOptions options) {
        this.a = a;
        this.b = b;
        this.mask = new InverseLerp(
            options.mask,
            new InverseLerpOptions(options.lowerBound, options.upperBound)
        );
    }

    protected override float Evaluate(Point point) {
        float maskValue = mask.Process(point);
        // Optimization, if one is unused, do not process that one
        if (maskValue == 0) {
            return b.Process(point);
        }
        if (maskValue == 1) {
            return a.Process(point);
        }
        // Else, process both and weigh each by the maskValue
        float aValue = a.Process(point);
        float bValue = b.Process(point);
        return (aValue * maskValue) + (bValue * (1 - maskValue));
    }

    public override TerrainInformation GetTerrainInformation() {
        TerrainInformation aInfo = a.GetTerrainInformation();
        TerrainInformation bInfo = b.GetTerrainInformation();
        return new TerrainInformation(
            Mathf.Min(aInfo.min, bInfo.min),
            Mathf.Max(aInfo.max, bInfo.max)
        );
    }
}

public class MaskOptions {
    public readonly TerrainTransform mask;
    public readonly float lowerBound;
    public readonly float upperBound;

    public MaskOptions(
        TerrainTransform mask,
        float lowerBound = float.MinValue,
        float upperBound = float.MaxValue
    ) {
        this.mask = mask;
        this.lowerBound = lowerBound != float.MinValue
            ? lowerBound
            : mask.GetTerrainInformation().min;
        this.upperBound = upperBound != float.MaxValue
            ? upperBound
            : mask.GetTerrainInformation().max;
    }
}
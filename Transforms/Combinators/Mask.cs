using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mask : TerrainTransform {
    private readonly TerrainTransform mask;
    private readonly TerrainTransform a;
    private readonly TerrainTransform b;

    public Mask(TerrainTransform a, TerrainTransform b, MaskOptions options) {
        this.a = a;
        this.b = b;
        this.mask = new InverseLerp(
            options.mask,
            new InverseLerpOptions(options.lowerBound, options.upperBound)
        );
    }

    protected override float Evaluate(Point point) {
        float aValue = a.Process(point);
        float bValue = b.Process(point);
        float maskValue = mask.Process(point);
        return (aValue * maskValue) + (bValue * (1 - maskValue));
    }

    public override TerrainInformation GetTerrainInformation() {
        TerrainInformation aInfo = a.GetTerrainInformation();
        TerrainInformation bInfo = b.GetTerrainInformation();
        return new TerrainInformation(
            Mathf.Min(aInfo.min * bInfo.min, aInfo.min * bInfo.max, aInfo.max * bInfo.min, aInfo.max * bInfo.max),
            Mathf.Max(aInfo.min * bInfo.min, aInfo.min * bInfo.max, aInfo.max * bInfo.min, aInfo.max * bInfo.max)
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
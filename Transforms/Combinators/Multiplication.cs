using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Multiplication : TerrainTransform {
    private readonly TerrainTransform a;
    private readonly TerrainTransform b;

    public Multiplication(TerrainTransform a, TerrainTransform b) {
        this.a = a;
        this.b = b;
    }

    protected override float Evaluate(Point point) {
        float aValue = a.Process(point);
        float bValue = b.Process(point);
        return aValue * bValue;
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

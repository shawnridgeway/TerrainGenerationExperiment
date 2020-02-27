using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Addition : TerrainTransform {
    private readonly TerrainTransform a;
    private readonly TerrainTransform b;

    public Addition(TerrainTransform a, TerrainTransform b) {
        this.a = a;
        this.b = b;
    }

    protected override float Evaluate(Point point) {
        float aValue = a.Process(point);
        float bValue = b.Process(point);
        return aValue + bValue;
    }

    public override TerrainInformation GetTerrainInformation() {
        TerrainInformation aInfo = a.GetTerrainInformation();
        TerrainInformation bInfo = b.GetTerrainInformation();
        return new TerrainInformation(aInfo.min + bInfo.min, aInfo.max + bInfo.max);
    }
}

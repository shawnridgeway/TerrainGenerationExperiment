using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identity : TerrainTransform {
    private readonly TerrainTransform a;

    public Identity(TerrainTransform a) {
        this.a = a;
    }

    protected override float Evaluate(Point point) {
        float aValue = a.Process(point);
        return aValue;
    }

    public override TerrainInformation GetTerrainInformation() {
        TerrainInformation aInfo = a.GetTerrainInformation();
        return new TerrainInformation(aInfo.min, aInfo.max);
    }
}

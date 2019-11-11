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

    public IEnumerable<float> Process(IEnumerable<Point> points) {
        IEnumerable<float> aValues = a.Process(points);
        IEnumerable<float> bValues = b.Process(points);
        IEnumerable<(float, float)> operands = aValues.Zip(bValues, (aValue, bValue) => (aValue, bValue));
        foreach ((float, float) pair in operands) {
            yield return pair.Item1 + pair.Item2;
        }
    }

    public TerrainInformation GetTerrainInformation() {
        TerrainInformation aInfo = a.GetTerrainInformation();
        TerrainInformation bInfo = b.GetTerrainInformation();
        return new TerrainInformation(aInfo.min + bInfo.min, aInfo.max + bInfo.max);
    }
}

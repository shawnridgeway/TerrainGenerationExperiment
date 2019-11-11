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

    public IEnumerable<float> Process(IEnumerable<Point> points) {
        IEnumerable<float> aValues = a.Process(points);
        IEnumerable<float> bValues = b.Process(points);
        IEnumerable<(float, float)> operands = aValues.Zip(bValues, (aValue, bValue) => (aValue, bValue));
        foreach ((float, float) pair in operands) {
            yield return pair.Item1 * pair.Item2;
        }
    }

    public TerrainInformation GetTerrainInformation() {
        TerrainInformation aInfo = a.GetTerrainInformation();
        TerrainInformation bInfo = b.GetTerrainInformation();
        return new TerrainInformation(
            Mathf.Min(aInfo.min * bInfo.min, aInfo.min * bInfo.max, aInfo.max * bInfo.min, aInfo.max * bInfo.max),
            Mathf.Max(aInfo.min * bInfo.min, aInfo.min * bInfo.max, aInfo.max * bInfo.min, aInfo.max * bInfo.max)
        );
    }
}

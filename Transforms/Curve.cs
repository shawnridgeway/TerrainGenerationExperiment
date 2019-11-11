using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : TerrainTransform {
	private readonly TerrainTransform a;
	private readonly CurveOptions options;

	public Curve(TerrainTransform a, CurveOptions options) {
		this.a = a;
		this.options = options;
	}

	public IEnumerable<float> Process(IEnumerable<Point> points) {
        // Get a separate clone so that there are no threading issues
        AnimationCurve animationCurveClone = new AnimationCurve(options.curve.keys);
        IEnumerable<float> aValues = a.Process(points);
        TerrainInformation aInfo = a.GetTerrainInformation();
		foreach (float value in aValues) {
			yield return Mathf.Lerp(aInfo.min, aInfo.max, animationCurveClone.Evaluate(Mathf.InverseLerp(aInfo.min, aInfo.max, value)));
		}
    }

    public TerrainInformation GetTerrainInformation() {
        // Get a separate clone so that there are no threading issues
        AnimationCurve animationCurveClone = new AnimationCurve(options.curve.keys);
        TerrainInformation aInfo = a.GetTerrainInformation();
        return new TerrainInformation(
            Mathf.Min(animationCurveClone.Evaluate(aInfo.min), animationCurveClone.Evaluate(aInfo.max)),
            Mathf.Max(animationCurveClone.Evaluate(aInfo.min), animationCurveClone.Evaluate(aInfo.max))
        );
    }
}

public class CurveOptions {
	public readonly AnimationCurve curve;

    public CurveOptions(AnimationCurve curve) {
		this.curve = curve;
	}
}

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

    protected override float Evaluate(Point point) {
        // Get a separate clone so that there are no threading issues
        AnimationCurve animationCurveClone = new AnimationCurve(options.curve.keys);
        float aValue = a.Process(point);
        TerrainInformation aInfo = a.GetTerrainInformation();
		return Mathf.Lerp(
            aInfo.min,
            aInfo.max,
            animationCurveClone.Evaluate(
                Mathf.InverseLerp(
                    aInfo.min,
                    aInfo.max,
                    aValue
                )
            )
        );
    }

    public override TerrainInformation GetTerrainInformation() {
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

    public CurveOptions(AnimationCurve curve = null) {
		this.curve = curve;
        if (this.curve == null) {
            this.curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        }
	}
}

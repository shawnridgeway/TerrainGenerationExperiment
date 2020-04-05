using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Curve : TerrainTransform {
	private readonly TerrainTransform a;
	private readonly CurveOptions options;

    private readonly ConcurrentDictionary<int, AnimationCurve> curveClonesByThread =
        new ConcurrentDictionary<int, AnimationCurve>();

	public Curve(TerrainTransform a, CurveOptions options) {
		this.a = a;
		this.options = options;
	}

    protected override float Evaluate(Point point) {
        AnimationCurve curve = GetAnimationCurve();
        float aValue = a.Process(point);
        TerrainInformation aInfo = a.GetTerrainInformation();
		return Mathf.Lerp(
            aInfo.min,
            aInfo.max,
            curve.Evaluate(
                Mathf.InverseLerp(
                    aInfo.min,
                    aInfo.max,
                    aValue
                )
            )
        );
    }

    public override TerrainInformation GetTerrainInformation() {
        AnimationCurve curve = GetAnimationCurve();
        TerrainInformation aInfo = a.GetTerrainInformation();
        return new TerrainInformation(
            Mathf.Min(curve.Evaluate(aInfo.min), curve.Evaluate(aInfo.max)),
            Mathf.Max(curve.Evaluate(aInfo.min), curve.Evaluate(aInfo.max))
        );
    }

    private AnimationCurve GetAnimationCurve() {
        // Get a separate clone per thread so there are no concurrency issues
        int currentThreadHash = Thread.CurrentThread.GetHashCode();
        AnimationCurve curve;
        bool curveExists = curveClonesByThread.TryGetValue(currentThreadHash, out curve);
        if (!curveExists) {
            curve = new AnimationCurve(options.curve.keys);
            curveClonesByThread.TryAdd(currentThreadHash, curve);
        }
        return curve;
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

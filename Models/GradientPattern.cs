using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GradientPattern {
    private readonly GradientPatternOptions options;
    private readonly float firstKeyPosition;
    private readonly float lastKeyPosition;
    private readonly float gradientLength;

    public GradientPattern(GradientPatternOptions options) {
        this.options = options;
        this.firstKeyPosition = options.keys[0].position;
        this.lastKeyPosition = options.keys[options.keys.Length - 1].position;
        this.gradientLength = lastKeyPosition - firstKeyPosition;
    }

    public float EvaluateAtInterval(float interval) {
        if (interval < firstKeyPosition || interval >= lastKeyPosition) {
            interval = HandleOutOfBoundsInterval(interval);
        }
        GradientPatternKey leftKey;
        try {
            leftKey = options.keys.Last(key => key.position <= interval);
        } catch(Exception) {
            return options.keys[0].value / 2f;
        }
        GradientPatternKey rightKey;
        try {
            rightKey = options.keys.First(key => key.position > interval);
        } catch(Exception) {
            return options.keys[options.keys.Length - 1].value / 2f;
        }
        float keyIntervalLength = rightKey.position - leftKey.position;
        return (
            Mathf.Abs((interval - leftKey.position) - keyIntervalLength) / keyIntervalLength * leftKey.value +
            Mathf.Abs((rightKey.position - interval) - keyIntervalLength) / keyIntervalLength * rightKey.value
        );
    }

    float HandleOutOfBoundsInterval(float interval) {
        if (options.mode == GradientPatternMode.Clamp) {
            if (interval < firstKeyPosition) {
                return firstKeyPosition;
            }
            if (interval >= lastKeyPosition) {
                return lastKeyPosition;
            }
        }
        if (options.mode == GradientPatternMode.Repeat) {
            return MathUtils.CanonicalModulus(interval - firstKeyPosition, gradientLength) + firstKeyPosition;
        }
        return interval;
    }

    public float GetMinKey() {
        return options.keys.Select(key => key.value).Min();
    }

    public float GetMaxKey() {
        return options.keys.Select(key => key.value).Max();
    }

    public static GradientPattern Default() {
        return new GradientPattern(
            new GradientPatternOptions(
                new GradientPatternKey[] {
                    new GradientPatternKey(0f, 0f),
                    new GradientPatternKey(1f, 1f)
                }
            )
        );
    }
}

public class GradientPatternOptions {
    public readonly GradientPatternKey[] keys;
    public readonly GradientPatternMode mode;

    public GradientPatternOptions(
        GradientPatternKey[] keys,
        GradientPatternMode mode = GradientPatternMode.Clamp
    ) {
        this.keys = keys.OrderBy(key => key.position).ToArray();
        this.mode = mode;
    }

}

public struct GradientPatternKey {
    public readonly float position;
    public readonly float value;

    public GradientPatternKey(float position, float value) {
        this.position = position;
        this.value = value;
    }
}

public enum GradientPatternMode {
    Clamp,
    Repeat
}

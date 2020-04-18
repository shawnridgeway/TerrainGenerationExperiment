using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils {

    static float deg30inRad = Mathf.Sin(Mathf.PI / 6);
    static float deg45inRad = Mathf.Sin(Mathf.PI / 4);
    static float deg60inRad = Mathf.Sin(Mathf.PI / 3);
    static float deg90inRad = Mathf.Sin(Mathf.PI / 2);

    static float sin30 = Mathf.Sin(deg30inRad);
    static float sin45 = Mathf.Sin(deg45inRad);
    static float sin60 = Mathf.Sin(deg60inRad);

    // Modulus operation that returns the equivalance class rather than remainder.
    // Essendially a modulus that always returns a positive value.
    public static float CanonicalModulus(float quotient, float divisor) {
        if (divisor == 0) {
            return 0;
        }
        float remainder = quotient % divisor;
        if (remainder < 0) {
            return remainder + divisor;
        }
        return remainder;
    }

    public static float GetSqrDistance(Vector3 a, Vector3 b) {
        return
            Mathf.Pow(b.x - a.x, 2) +
            Mathf.Pow(b.y - a.y, 2) +
            Mathf.Pow(b.z - a.z, 2);
    }

    public static float Sin30() {
        return sin30;
    }

    public static float Sin45() {
        return sin45;
    }

    public static float Sin60() {
        return sin60;
    }
}

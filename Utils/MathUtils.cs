using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
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
}

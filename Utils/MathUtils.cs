using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    // Modulus operation that returns the equivalance class rather than remainder.
    // Essendially a modulus that always returns a positive value.
    public static float CanonicalModulus(float quotient, float divisor) {
        float remainder = quotient % divisor;
        if (remainder < 0) {
            return remainder + divisor;
        }
        return remainder;
    }
}

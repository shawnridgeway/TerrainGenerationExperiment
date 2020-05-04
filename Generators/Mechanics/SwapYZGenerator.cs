using UnityEngine;

class SwapYZGenerator : CoherentNoise.Generator {
    private readonly CoherentNoise.Generator source;

    public SwapYZGenerator(CoherentNoise.Generator source) {
        this.source = source;
    }

    public override float GetValue(float x, float y, float z) {
        return source.GetValue(x, z, y); // Here we swap y and z axis
    }
}

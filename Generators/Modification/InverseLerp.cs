using UnityEngine;

public class InverseLerp : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public InverseLerp(
        TerrainGenerator source,
        float lowerBound,
        float upperBound
    ) {
        _generator = new InverseLerpGenerator(source.GetGenerator(), lowerBound, upperBound);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
        }
}

class InverseLerpGenerator : CoherentNoise.Generator {
    private readonly CoherentNoise.Generator source;
    private readonly float lowerBound;
    private readonly float upperBound;

    public InverseLerpGenerator(CoherentNoise.Generator source, float lowerBound, float upperBound) {
        this.source = source;
        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
    }

    public override float GetValue(float x, float y, float z) {
        float value = source.GetValue(x, y, z);
        return Mathf.InverseLerp(lowerBound, upperBound, value);
    }
}

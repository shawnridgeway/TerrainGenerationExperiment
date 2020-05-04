using UnityEngine;

public class Gain : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Gain(
        TerrainGenerator source,
        float gain
    ) {
        _generator = new CoherentNoise.Generation.Modification.Gain(source.GetGenerator(), gain);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

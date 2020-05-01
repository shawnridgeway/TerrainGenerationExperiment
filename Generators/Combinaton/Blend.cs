using UnityEngine; 

public class Blend : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public Blend(
        TerrainGenerator source1,
        TerrainGenerator source2,
        TerrainGenerator weight
    ) {
        _generator = new CoherentNoise.Generation.Combination.Blend(source1.GetGenerator(), source2.GetGenerator(), weight.GetGenerator());
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

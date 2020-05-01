using UnityEngine;

public class TexturePattern : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public TexturePattern(
        Texture2D texture,
        TextureWrapMode wrapMode
    ) {
        _generator = new CoherentNoise.Generation.Patterns.TexturePattern(texture, wrapMode);
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

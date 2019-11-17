using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom : TerrainTransform {
    private readonly CustomOptions options;

    public Custom(CustomOptions options) {
        this.options = options;
    }

    protected override float Evaluate(Point point) {
        int x = Mathf.RoundToInt(point.GetLocation().x);
        int z = Mathf.RoundToInt(point.GetLocation().z);
        return options.image.GetPixel(x, z).grayscale;
    }

    public override TerrainInformation GetTerrainInformation() {
        return new TerrainInformation(0, 1);
    }
}

public class CustomOptions {
    public readonly Texture2D image;
    public readonly Vector2 offset;

    public CustomOptions(Texture2D image, Vector2 offset = new Vector2()) {
        this.image = image;
        this.offset = offset;
    }
}
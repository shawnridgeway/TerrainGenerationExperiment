using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom : TerrainTransform {
    private readonly CustomOptions options;

    public Custom(CustomOptions options) {
        this.options = options;
    }

    public IEnumerable<float> Process(IEnumerable<Point> points) {
        foreach (Point point in points) {
            int x = Mathf.RoundToInt(point.GetLocation().x);
            int z = Mathf.RoundToInt(point.GetLocation().z);
            yield return options.image.GetPixel(x, z).grayscale;
        }
    }

    public TerrainInformation GetTerrainInformation() {
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
using System;
using UnityEngine;

public class SimpleVoronoi : TerrainTransform {
    private readonly SimpleVoronoiOptions options;

    public SimpleVoronoi(SimpleVoronoiOptions options) {
        this.options = options;
    }

    protected override float Evaluate(Point point) {
        VoronoiRegion region = options.voronoiModel.GetCanonicalRegionForPoint(point.GetLocation());
        return options.mapResultToValue(region, point);
    }

    public override TerrainInformation GetTerrainInformation() {
        return new TerrainInformation(options.min, options.max);
    }
}

public class SimpleVoronoiOptions {
    public readonly Voronoi voronoiModel;
    public readonly Func<VoronoiRegion, Point, float> mapResultToValue;
    public readonly float min;
    public readonly float max;

    public SimpleVoronoiOptions(
        Voronoi voronoiModel,
        Func<VoronoiRegion, Point, float> mapResultToValue = null,
        float min = -Mathf.Infinity,
        float max = Mathf.Infinity
    ) {
        this.voronoiModel = voronoiModel;
        this.mapResultToValue = mapResultToValue;
        if (this.mapResultToValue == null) {
            this.mapResultToValue = (result, point) => 0f;
        }
        this.min = min;
        this.max = max;
    }
}

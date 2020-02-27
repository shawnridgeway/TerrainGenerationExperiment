using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiTesselation : TerrainTransform {
    private readonly VoronoiTesselationOptions options;

    public VoronoiTesselation(VoronoiTesselationOptions options) {
        this.options = options;
    }

    protected override float Evaluate(Point point) {
        TerrainTransform fill = GetFillForPoint(point);
        return fill.Process(point);
    }

    public override TerrainInformation GetTerrainInformation() {
        float min = Mathf.Infinity;
        float max = -Mathf.Infinity;
        foreach (TerrainTransform fill in options.fills) {
            float minCandidate = fill.GetTerrainInformation().min;
            if (minCandidate < min) {
                min = minCandidate;
            }
            float maxCandidate = fill.GetTerrainInformation().max;
            if (maxCandidate > max) {
                min = maxCandidate;
            }
        }
        return new TerrainInformation(min, max);
    }

    TerrainTransform GetFillForPoint(Point point) {
        VoronoiResult voronoiResult = options.voronoiModel.EvaluateAtLocation(point.GetLocation());
        return options.fills[voronoiResult.closestSiteIndex % options.fills.Length];
    }
}

public class VoronoiTesselationOptions {
    public readonly TerrainTransform[] fills;
    public readonly Voronoi voronoiModel;

    public VoronoiTesselationOptions(
        TerrainTransform[] fills,
        Voronoi voronoiModel
    ) {
        this.fills = fills;
        this.voronoiModel = voronoiModel;
    }
}

using System;
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
        return options.fills[options.mapResultToFillIndex(voronoiResult)];
    }
}

public class VoronoiTesselationOptions {
    public readonly TerrainTransform[] fills;
    public readonly Voronoi voronoiModel;
    public readonly Func<VoronoiResult, int> mapResultToFillIndex;

    public VoronoiTesselationOptions(
        TerrainTransform[] fills,
        Voronoi voronoiModel,
        Func<VoronoiResult, int> mapResultToFillIndex = null
    ) {
        this.fills = fills;
        this.voronoiModel = voronoiModel;
        this.mapResultToFillIndex = mapResultToFillIndex;
        if (this.mapResultToFillIndex == null) {
            this.mapResultToFillIndex = (result => result.closestSiteIndex % this.fills.Length);
        }
    }
}

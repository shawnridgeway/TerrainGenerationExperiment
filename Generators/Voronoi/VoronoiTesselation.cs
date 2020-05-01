//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;

//public class VoronoiTesselation : TerrainGenerator {
//    private readonly VoronoiTesselationOptions options;
//    private readonly Dictionary<VoronoiRegion, TerrainGenerator> dictRegionToFill = new Dictionary<VoronoiRegion, TerrainGenerator>();
//    private readonly TerrainGenerator transform;

//    public VoronoiTesselation(VoronoiTesselationOptions options) {
//        this.options = options;
//        int i = 0;
//        foreach (VoronoiRegion region in options.voronoiModel.GetCanonicalRegions()) {
//            dictRegionToFill.Add(region, options.fills[i]);
//            i++;
//        }
//        Vector3 scale = options.voronoiModel.GetScale();
//        Func<VoronoiRegion, Point, float> mapFunction = (region, point) => {
//            // If the closest site for a given point lies outside of the bounds,
//            // use the equivalent point whose closest site is in bounds.
//            Vector3 regionCenter = region.GetCenter();
//            if (regionCenter.x < 0) {
//                point = point.MapPoint(location => new Vector3(location.x + scale.x, location.y, location.z));
//            }
//            if (regionCenter.x >= scale.x) {
//                point = point.MapPoint(location => new Vector3(location.x - scale.x, location.y, location.z));
//            }
//            if (regionCenter.y < 0) {
//                point = point.MapPoint(location => new Vector3(location.x, location.y + scale.y, location.z));
//            }
//            if (regionCenter.y >= scale.y) {
//                point = point.MapPoint(location => new Vector3(location.x, location.y - scale.y, location.z));
//            }
//            if (regionCenter.z < 0) {
//                point = point.MapPoint(location => new Vector3(location.x, location.y, location.z + scale.z));
//            }
//            if (regionCenter.z >= scale.z) {
//                point = point.MapPoint(location => new Vector3(location.x, location.y, location.z - scale.z));
//            }
//            TerrainGenerator fill = dictRegionToFill[region];
//            return fill.GetValue(point);
//        };
//        transform = new Modulus(
//            new SimpleVoronoi(
//                new SimpleVoronoiOptions(
//                    options.voronoiModel,
//                    mapFunction
//                )
//            ),
//            new ModulusOptions(scale * 1f)
//        );
//    }

//    protected override float Evaluate(Point point) {
//        return transform.GetValue(point);
//    }

//    public override TerrainInformation GetTerrainInformation() {
//        float min = Mathf.Infinity;
//        float max = -Mathf.Infinity;
//        foreach (TerrainGenerator fill in options.fills) {
//            float minCandidate = fill.GetTerrainInformation().min;
//            if (minCandidate < min) {
//                min = minCandidate;
//            }
//            float maxCandidate = fill.GetTerrainInformation().max;
//            if (maxCandidate > max) {
//                min = maxCandidate;
//            }
//        }
//        return new TerrainInformation(min, max);
//    }
//}

//public class VoronoiTesselationOptions {
//    public readonly TerrainGenerator[] fills;
//    public readonly Voronoi voronoiModel;

//    public VoronoiTesselationOptions(
//        TerrainGenerator[] fills,
//        Voronoi voronoiModel
//    ) {
//        this.fills = fills;
//        this.voronoiModel = voronoiModel;
//        if (fills.Length != voronoiModel.GetCount()) {
//            throw new Exception("The count of fills must be the same as the count of sites in the Voronoi model.");
//        }
//    }
//}

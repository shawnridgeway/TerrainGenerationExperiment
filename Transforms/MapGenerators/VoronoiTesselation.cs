using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiTesselation : TerrainTransform {
    private readonly VoronoiTesselationOptions options;
    private readonly Vector3[] sites;

    public VoronoiTesselation(VoronoiTesselationOptions options) {
        this.options = options;
        sites = new Vector3[options.count];
        System.Random prng = new System.Random(options.seed);
        for (int i = 0; i < options.count; i++) {
            float offsetX = (prng.Next(0, options.scale) + options.offset.x) % options.scale;
            float offsetZ = (prng.Next(0, options.scale) - options.offset.z) % options.scale;
            sites[i] = new Vector3(offsetX, 0, offsetZ);
        }
    }

    protected override float Evaluate(Point point) {
        TerrainTransform fill = GetFillForPoint(point);
        return fill.Process(point);
    }

    public override TerrainInformation GetTerrainInformation() {
        return new TerrainInformation(0, 0);
    }

    TerrainTransform GetFillForPoint(Point point) {
        int closestSiteIndex = 0;
        float closestSiteDistanceSqr = Mathf.Infinity;
        // Iterate through 3 periods in each dimension for seamless tesselation
        // TODO: use 4 copies instead of 9 by looking at closest neighbor (and dist to closest edge)
        for (int periodX = -1; periodX <= 1; periodX++) {
            for (int periodZ = -1; periodZ <= 1; periodZ++) {
                for (int i = 0; i < sites.Length; i++) {
                    Vector3 site = sites[i];
                    float siteDistanceSqr = GetDistance(site, point, periodX, periodZ);
                    if (siteDistanceSqr < closestSiteDistanceSqr) {
                        closestSiteIndex = i;
                        closestSiteDistanceSqr = siteDistanceSqr;
                    }
                }
            }
        }
        return options.fills[closestSiteIndex % options.fills.Length];
    }

    float GetDistance(Vector3 site, Point point, int periodX, int periodZ) {
        return
            Mathf.Pow(site.x + periodX * options.scale - point.GetLocation().x % options.scale, 2) +
            Mathf.Pow(site.z + periodZ * options.scale - point.GetLocation().z % options.scale, 2);
    }
}

public class VoronoiTesselationOptions {
    public readonly TerrainTransform[] fills;
    public readonly int count;
    public readonly int scale;
    public readonly Vector3 offset;
    public readonly int seed;

    public VoronoiTesselationOptions(
        TerrainTransform[] fills,
        int count = 10,
        int scale = 100,
        Vector3 offset = new Vector3(),
        int seed = 0
    ) {
        this.fills = fills;
        this.count = count;
        this.scale = scale;
        this.offset = offset;
        this.seed = seed;
    }
}

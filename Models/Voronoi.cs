using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voronoi {
    private readonly VoronoiOptions options;
    private readonly Vector3[] sites;

    public Voronoi(VoronoiOptions options) {
        this.options = options;
        sites = new Vector3[options.count];
        System.Random prng = new System.Random(options.seed);
        for (int i = 0; i < options.count; i++) {
            float offsetX = (prng.Next(0, options.scale) + options.offset.x) % options.scale;
            float offsetZ = (prng.Next(0, options.scale) - options.offset.z) % options.scale;
            sites[i] = new Vector3(offsetX, 0, offsetZ);
        }
    }

    public VoronoiResult EvaluateAtLocation(Vector3 location) {
        Vector3 closestSite = Vector3.zero;
        int closestSiteIndex = 0;
        float closestSiteDistanceSqr = Mathf.Infinity;
        Vector3 secondClosestSite = Vector3.zero;
        int secondClosestSiteIndex = 0;
        float secondClosestSiteDistanceSqr = Mathf.Infinity;
        // Iterate through 3 periods in each dimension for seamless tesselation
        // TODO: use 4 copies instead of 9 by looking at closest neighbor (and dist to closest edge)
        for (int periodX = -1; periodX <= 1; periodX++) {
            for (int periodZ = -1; periodZ <= 1; periodZ++) {
                for (int i = 0; i < sites.Length; i++) {
                    Vector3 rawSite = sites[i];
                    Vector3 site = new Vector3(
                        rawSite.x + periodX * options.scale,
                        rawSite.y,
                        rawSite.z + periodZ * options.scale
                    );
                    float siteDistanceSqr = GetDistance(site, location);
                    if (siteDistanceSqr < closestSiteDistanceSqr) {
                        secondClosestSite = closestSite;
                        secondClosestSiteIndex = closestSiteIndex;
                        secondClosestSiteDistanceSqr = closestSiteDistanceSqr;
                        closestSite = site;
                        closestSiteIndex = i;
                        closestSiteDistanceSqr = siteDistanceSqr;
                    }
                }
            }
        }
        return new VoronoiResult(
            closestSiteIndex: closestSiteIndex,
            closestSite: closestSite,
            distanceToClosestSite: Mathf.Sqrt(closestSiteDistanceSqr),
            distanceToClosestBorder: Mathf.Sqrt(closestSiteDistanceSqr) - Mathf.Sqrt(secondClosestSiteDistanceSqr)
        );
    }

    float GetDistance(Vector3 site, Vector3 location) {
        return
            Mathf.Pow(site.x - location.x % options.scale, 2) +
            Mathf.Pow(site.z - location.z % options.scale, 2);
    }
}

public class VoronoiOptions {
    public readonly int count;
    public readonly int scale;
    public readonly Vector3 offset;
    public readonly int seed;

    public VoronoiOptions(
        int count = 10,
        int scale = 100,
        Vector3 offset = new Vector3(),
        int seed = 0
    ) {
        this.count = count;
        this.scale = scale;
        this.offset = offset;
        this.seed = seed;
    }
}

public struct VoronoiResult {
    public readonly int closestSiteIndex;
    public readonly Vector3 closestSite;
    public readonly float distanceToClosestSite;
    public readonly float distanceToClosestBorder;

    public VoronoiResult(int closestSiteIndex, Vector3 closestSite, float distanceToClosestSite, float distanceToClosestBorder) {
        this.closestSiteIndex = closestSiteIndex;
        this.closestSite = closestSite;
        this.distanceToClosestSite = distanceToClosestSite;
        this.distanceToClosestBorder = distanceToClosestBorder;
    }
}

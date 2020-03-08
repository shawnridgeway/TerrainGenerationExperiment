using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voronoi {
    private readonly VoronoiOptions options;
    private readonly Vector3[] sites;
    private readonly Vector3[] normalizedCandidateSites;

    public Voronoi(VoronoiOptions options) {
        this.options = options;
        sites = new Vector3[options.count];
        System.Random prng = new System.Random(options.seed);
        for (int i = 0; i < options.count; i++) {
            float offsetX = MathUtils.CanonicalModulus(prng.Next(0, options.scale) + options.offset.x, options.scale);
            float offsetZ = MathUtils.CanonicalModulus(prng.Next(0, options.scale) - options.offset.z, options.scale);
            sites[i] = new Vector3(offsetX, 0, offsetZ);
        }

        // Iterate through 3 periods in each dimension for seamless tesselation
        // TODO: use 4 copies instead of 9 by looking at closest neighbor (and dist to closest edge)
        normalizedCandidateSites = new Vector3[options.count * 9];
        int index = 0;
        for (int periodX = -1; periodX <= 1; periodX++) {
            for (int periodZ = -1; periodZ <= 1; periodZ++) {
                foreach (Vector3 site in sites) {
                    normalizedCandidateSites[index] = new Vector3(
                        site.x + periodX * options.scale,
                        site.y,
                        site.z + periodZ * options.scale
                    );
                    index++;
                }
            }
        }
    }

    public Vector3[] GetSites() {
        return sites;
    }

    public VoronoiResult EvaluateAtLocation(Vector3 location) {
        Vector3 closestSite = Vector3.zero;
        int closestSiteIndex = 0;
        float closestSiteDistanceSqr = Mathf.Infinity;
        Vector3 secondClosestSite = Vector3.zero;
        int secondClosestSiteIndex = 0;
        float secondClosestSiteDistanceSqr = Mathf.Infinity;
        for (int candidateIndex = 0; candidateIndex < normalizedCandidateSites.Length; candidateIndex++) {
            Vector3 normalizedCandidateSite = normalizedCandidateSites[candidateIndex];
            Vector3 normalizedLocation = new Vector3(
                MathUtils.CanonicalModulus(location.x, options.scale),
                0,
                MathUtils.CanonicalModulus(location.z, options.scale)
            );
            Vector3 normalOffset = new Vector3(
                Mathf.Floor(location.x / options.scale),
                0,
                Mathf.Floor(location.z / options.scale)
            );
            float siteDistanceSqr = GetSqrDistance(normalizedCandidateSite, normalizedLocation);
            if (siteDistanceSqr < closestSiteDistanceSqr) {
                secondClosestSite = closestSite;
                secondClosestSiteIndex = closestSiteIndex;
                secondClosestSiteDistanceSqr = closestSiteDistanceSqr;
                closestSite = new Vector3(
                    normalizedCandidateSite.x + normalOffset.x * options.scale,
                    normalizedCandidateSite.y + normalOffset.y * options.scale,
                    normalizedCandidateSite.z + normalOffset.z * options.scale
                );
                closestSiteIndex = candidateIndex % options.count; // Account for repeats
                closestSiteDistanceSqr = siteDistanceSqr;
            }
        }
        return new VoronoiResult(
            scale: options.scale,
            closestSiteIndex: closestSiteIndex,
            closestSite: closestSite,
            distanceToClosestSite: Mathf.Sqrt(closestSiteDistanceSqr),
            distanceToClosestBorder: Mathf.Sqrt(closestSiteDistanceSqr) - Mathf.Sqrt(secondClosestSiteDistanceSqr)
        );
    }

    private float GetSqrDistance(Vector3 site, Vector3 location) {
        return
            Mathf.Pow(site.x - location.x, 2) +
            Mathf.Pow(site.z - location.z, 2);
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
    public readonly int scale;
    public readonly int closestSiteIndex;
    public readonly Vector3 closestSite;
    public readonly float distanceToClosestSite;
    public readonly float distanceToClosestBorder;

    public VoronoiResult(int scale, int closestSiteIndex, Vector3 closestSite, float distanceToClosestSite, float distanceToClosestBorder) {
        this.scale = scale;
        this.closestSiteIndex = closestSiteIndex;
        this.closestSite = closestSite;
        this.distanceToClosestSite = distanceToClosestSite;
        this.distanceToClosestBorder = distanceToClosestBorder;
    }
}

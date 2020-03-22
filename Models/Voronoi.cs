using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voronoi {
    private readonly VoronoiOptions options;
    private readonly VoronoiRegion[] canonicalRegions;

    public Voronoi(VoronoiOptions options) {
        this.options = options;
        System.Random prng = new System.Random(options.seed);
        canonicalRegions = new VoronoiRegion[options.count];
        for (int i = 0; i < options.count; i++) {
            float x = (float)prng.NextDouble() * options.scale.x;
            float y = (float)prng.NextDouble() * options.scale.y;
            float z = (float)prng.NextDouble() * options.scale.z;
            canonicalRegions[i] = new VoronoiRegion(new Vector3(x, y, z), options.scale);
        }
        foreach (VoronoiRegion region in canonicalRegions) {
            foreach (VoronoiRegion neighborCandidate in canonicalRegions) {
                Vector3[] neighborCandidateCenters = VoronoiHelpers.GetLocationWithAllIterations(neighborCandidate.GetCenter(), options.scale);
                foreach (Vector3 neighborCandidateCenter in neighborCandidateCenters) {
                    if (IsNeighbor(region.GetCenter(), neighborCandidateCenter)) {
                        region.AddNeighbor(neighborCandidate, neighborCandidateCenter);
                    }
                }
            }
        }
    }

    public VoronoiRegion[] GetCanonicalRegions() {
        return canonicalRegions;
    }

    public Vector3 GetScale() {
        return options.scale;
    }

    public int GetCount() {
        return options.count;
    }

    public VoronoiRegion GetCanonicalRegionForPoint(Vector3 location) {
        Vector3 canonicalLocation = new Vector3(
            MathUtils.CanonicalModulus(location.x, options.scale.x),
            MathUtils.CanonicalModulus(location.y, options.scale.y),
            MathUtils.CanonicalModulus(location.z, options.scale.z)
        );
        // Get iterations of the canonicalLocation on the opposite side to test for regions across repetitons
        Vector3[] locationWithIterations = VoronoiHelpers.GetLocationWithIterations(canonicalLocation, options.scale);
        VoronoiRegion closestRegion = canonicalRegions[0];
        float closestRegionDistanceSqr = Mathf.Infinity;
        foreach (Vector3 locationIteration in locationWithIterations) {
            foreach (VoronoiRegion candidateRegion in canonicalRegions) {
                float candidateDistanceSqr = MathUtils.GetSqrDistance(candidateRegion.GetCenter(), locationIteration);
                if (candidateDistanceSqr < closestRegionDistanceSqr) {
                    closestRegion = candidateRegion;
                    closestRegionDistanceSqr = candidateDistanceSqr;
                }
            }
        }
        return closestRegion;
    }

    private Vector3 GetRegionCenterForPoint(Vector3 location) {
        Vector3 canonicalLocation = new Vector3(
            MathUtils.CanonicalModulus(location.x, options.scale.x),
            MathUtils.CanonicalModulus(location.y, options.scale.y),
            MathUtils.CanonicalModulus(location.z, options.scale.z)
        );
        Vector3 offestFromCanonical = location - canonicalLocation;
        VoronoiRegion closestRegion = GetCanonicalRegionForPoint(canonicalLocation);
        return closestRegion.GetCenter() + offestFromCanonical;
    }

    private bool IsNeighbor(Vector3 a, Vector3 b) {
        if (a == b) {
            return false;
        }
        //Vector3 midPoint = (a + b) / 2;
        //Vector3 midPointRegionCenter = GetRegionCenterForPoint(midPoint);
        // TODO: figure out how to determine if two regions share a border
        return true;
    }
}

public class VoronoiOptions {
    public readonly int count;
    public readonly Vector3 scale;
    public readonly int seed;

    public VoronoiOptions(
        int count = 10,
        Vector3 scale = new Vector3(),
        int seed = 0
    ) {
        this.count = count;
        this.scale = scale;
        this.seed = seed;
    }
}

public struct VoronoiRegion {
    private readonly Vector3 center;
    private Vector3 scale;
    private readonly List<VoronoiRegion> neighborRegions;
    private readonly List<Vector3> borderPoints;
    private readonly List<Vector3> borderNormals;

    public VoronoiRegion(
        Vector3 center,
        Vector3 scale
    ) {
        this.center = center;
        this.scale = scale;
        this.neighborRegions = new List<VoronoiRegion>();
        this.borderPoints = new List<Vector3>();
        this.borderNormals = new List<Vector3>();
    }

    public void AddNeighbor(VoronoiRegion neighborRegion, Vector3 neighborRegionIterationCenter) {
        neighborRegions.Add(neighborRegion);
        Vector3 borderNormal = (neighborRegionIterationCenter - center).normalized;
        borderNormals.Add(borderNormal);
        Vector3 borderPoint = (neighborRegionIterationCenter + center) / 2;
        borderPoints.Add(borderPoint);
    }

    public Vector3 GetCenter() {
        return center;
    }

    public float GetDistanceFromCenter(Vector3 point) {
        Vector3 cannonicalLocation = GetLocationInCannonicalRegions(point);
        return Vector3.Distance(cannonicalLocation, center);
    }

    public float GetDistanceToClosestBorder(Vector3 point) {
        Vector3 cannonicalLocation = GetLocationInCannonicalRegions(point);
        float minDistanceToBorder = Mathf.Infinity;
        for (int i = 0; i < borderPoints.Count; i++) {
            Vector3 borderPoint = borderPoints[i];
            Vector3 borderNormal = borderNormals[i];
            float distanceToBorder = GetDistanceFromPointToBorder(cannonicalLocation, borderPoint, borderNormal);
            if (distanceToBorder < minDistanceToBorder) {
                minDistanceToBorder = distanceToBorder;
            }
        }
        return minDistanceToBorder;
    }

    private float GetDistanceFromPointToBorder(Vector3 location, Vector3 borderPoint, Vector3 borderNormal) {
        // Source: http://geomalgorithms.com/a04-_planes.html
        float sn = -Vector3.Dot(borderNormal, location - borderPoint);
        float sd = Vector3.Dot(borderNormal, borderNormal);
        float sb = sn / sd;
        Vector3 basePoint = location + sb * borderNormal;
        return Mathf.Abs(Vector3.Distance(location, basePoint));
    }

    // Get a given point inside the range of the cannonical region set
    private Vector3 GetLocationInCannonicalRegions(Vector3 location) {
        Vector3 canonicalLocation = new Vector3(
            MathUtils.CanonicalModulus(location.x, scale.x),
            MathUtils.CanonicalModulus(location.y, scale.y),
            MathUtils.CanonicalModulus(location.z, scale.z)
        );
        Vector3[] locationWithIterations = VoronoiHelpers.GetLocationWithIterations(canonicalLocation, scale);
        Vector3 cannonicalLocation = Vector3.zero;
        float closestRegionDistanceSqr = Mathf.Infinity;
        foreach (Vector3 locationIteration in locationWithIterations) {
            float candidateDistanceSqr = MathUtils.GetSqrDistance(center, locationIteration);
            if (candidateDistanceSqr < closestRegionDistanceSqr) {
                cannonicalLocation = locationIteration;
                closestRegionDistanceSqr = candidateDistanceSqr;
            }
        }
        return cannonicalLocation;
    }
}

static class VoronoiHelpers {
    // Given a location, get iterations on the far side of each dimension
    public static Vector3[] GetLocationWithIterations(Vector3 location, Vector3 scale) {
        float x = location.x;
        float y = location.y;
        float z = location.z;
        float xShift = x < scale.x / 2 ? 1 : -1;
        float yShift = y < scale.y / 2 ? 1 : -1;
        float zShift = z < scale.z / 2 ? 1 : -1;
        float newX = x + xShift * scale.x;
        float newY = y + yShift * scale.y;
        float newZ = z + zShift * scale.z;
        Vector3[] locationAndSurroundingIterations = new Vector3[8];
        locationAndSurroundingIterations[0] = new Vector3(x, y, z);
        locationAndSurroundingIterations[1] = new Vector3(x, y, newZ);
        locationAndSurroundingIterations[2] = new Vector3(x, newY, z);
        locationAndSurroundingIterations[3] = new Vector3(x, newY, newZ);
        locationAndSurroundingIterations[4] = new Vector3(newX, y, z);
        locationAndSurroundingIterations[5] = new Vector3(newX, y, newZ);
        locationAndSurroundingIterations[6] = new Vector3(newX, newY, z);
        locationAndSurroundingIterations[7] = new Vector3(newX, newY, newZ);
        return locationAndSurroundingIterations;
    }

    // Given a location, get iterations on the far side of each dimension
    public static Vector3[] GetLocationWithAllIterations(Vector3 location, Vector3 scale) {
        float x = location.x;
        float y = location.y;
        float z = location.z;
        Vector3[] locationAndSurroundingIterations = new Vector3[27];
        int index = 0;
        for (int i = -1; i <= 1; i++) {
            float dx = i * scale.x;
            for (int j = -1; j <= 1; j++) {
                float dy = j * scale.y;
                for (int k = -1; k <= 1; k++) {
                    float dz = k * scale.z;
                    locationAndSurroundingIterations[index] = new Vector3(x + dx, y + dy, z + dz);
                    index++;
                }
            }
        }
        return locationAndSurroundingIterations;
    }
}
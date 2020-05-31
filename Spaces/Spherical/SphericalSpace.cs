using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Chunking happens as follows: the space is divided from the origin (0, 0, 0)
 * into the 8 cardinal octants. Each of these curvy triangular subsections of
 * the sphere are then divided into 2^n rows of similarly shaped triangular
 * chunks. 
 * 
 * Note on directions
 * 
 *     2  1   
 *      \/
 *   3 ---- 0 
 *      /\
 *     4  5
 */

public class SphericalSpace : ChunkedSpace {

    public enum Direction {
        E = 0,
        NE = 1,
        NW = 2,
        W = 3,
        SW = 4,
        SE = 5
    }

    private static readonly int cardinality = 3; // Number of dimensions

    // TODO: determine how many points/triangles can be in width give limit in mesh
    public static readonly int chunkWidth = 240; // How many GridUnits across each Chunk is on the edges
    public static readonly int chunkPointCount = 241; // How many points across each Chunk is (chunkWidth + 1)
    public readonly float gridUnit; // Distance between Points (in Coordinates along axises only)
    public readonly float chunkUnit; // Length of an edge of a chunk (in Coordinates along axises only)
    public readonly int divisions;

    private readonly float scale = 1; // Distance between Positions, essentially radius

    public SphericalSpace(float scale = 1, int divisions = 1) {
        this.scale = scale;
        this.divisions = divisions;
        chunkUnit = Mathf.PI / (2 * divisions);
        gridUnit = chunkUnit / chunkWidth;
    }

    public int GetCardinality() {
        return cardinality;
    }

    // How wide across a chunk is in terms of Position
    public float GetChunkScale() {
        return chunkUnit * scale;
    }

    public Point GetPointFromPosition(Vector3 position) {
        position = position.normalized;
        float longitude = Mathf.Atan2(position.x, position.z);
        float latitude = Mathf.Asin(position.y);
        return new SphericalPoint(new Vector2(latitude, longitude), this);
    }

    public Vector3 GetNormalFromPosition(Vector3 position) {
        return position.normalized;
    }

    public float GetDistanceBetweenPoints(Point a, Point b) {
        throw new NotImplementedException();
    }

    public Point GetClosestPointTo(Point origin) {
        throw new NotImplementedException();
    }

    public Point[] GetPointsWithin(Point origin, float distance) {
        throw new NotImplementedException();
    }

    public bool IsPointInRange(Point origin, Point point, float distance) {
        throw new NotImplementedException();
    }

    public Chunk GetClosestChunkTo(Point origin) {
        if (origin is SphericalPoint point) {
            Vector2 coordinate = point.GetCoordinate();
            float centerLatitude = Mathf.Round((coordinate.x - chunkUnit / 2f) / chunkUnit) * chunkUnit + (chunkUnit / 2f);
            float longitudeChunkUnitAtLatitude = GetLongitudeChunkUnitFromLatitude(centerLatitude);
            float closestCenterLongitude = Mathf.Round((coordinate.y - longitudeChunkUnitAtLatitude / 2f) / longitudeChunkUnitAtLatitude) * longitudeChunkUnitAtLatitude + (longitudeChunkUnitAtLatitude / 2f);
            SphericalChunk closestChunk = new SphericalChunk(new Vector2(centerLatitude, closestCenterLongitude), this);
            bool isClosestChunkInverted = closestChunk.IsChunkInverted();
            float centerLongitude;
            if (
                (isClosestChunkInverted && coordinate.x > centerLatitude) ||
                (!isClosestChunkInverted && coordinate.x <= centerLatitude)
            ) {
                centerLongitude = closestCenterLongitude;
            } else {
                float chunkLongitudeWidthAtLatitude = closestChunk.GetChunkLongitudalWidthAtLatitude(coordinate.x);
                if (Mathf.Abs(coordinate.y - closestCenterLongitude) < chunkLongitudeWidthAtLatitude) {
                    centerLongitude = closestCenterLongitude;
                } else {
                    if (coordinate.y > closestCenterLongitude) {
                        centerLongitude = closestCenterLongitude + longitudeChunkUnitAtLatitude;
                    } else {
                        centerLongitude = closestCenterLongitude - longitudeChunkUnitAtLatitude;
                    }
                }
            }
            Vector2 chunkCenterCoordinate = new Vector2(
                centerLatitude,
                centerLongitude
            );
            return new SphericalChunk(chunkCenterCoordinate, this);
        }
        return null;
    }

    public Chunk[] GetChunksWithin(Point origin, float distance) {
        Chunk closestChunk = GetClosestChunkTo(origin);
        if (closestChunk is SphericalChunk sc) {
            Debug.Log(string.Format("closestChunk {0}", sc.GetCenterCoordinate()));
        }
        HashSet<Chunk> acceptedChunks = new HashSet<Chunk>();
        HashSet<Chunk> rejectedChunks = new HashSet<Chunk>();
        Queue<Chunk> unprocessedChunks = new Queue<Chunk>();
        unprocessedChunks.Enqueue(closestChunk);
        while (unprocessedChunks.Count > 0) {
            Chunk currentChunk = unprocessedChunks.Dequeue();
            if (acceptedChunks.Contains(currentChunk) || rejectedChunks.Contains(currentChunk)) {
                continue;
            }
            if (IsChunkInRange(origin, currentChunk, distance)) {
                acceptedChunks.Add(currentChunk);
                foreach (SphericalChunk neighbor in currentChunk.GetNeighbors()) {
                    unprocessedChunks.Enqueue(neighbor);
                }
            } else {
                rejectedChunks.Add(currentChunk);
            }
        }
        Chunk[] chunksInRange = new Chunk[acceptedChunks.Count];
        acceptedChunks.CopyTo(chunksInRange);
        return chunksInRange;
    }

    public bool IsChunkInRange(Point origin, Chunk chunk, float distance) {
        // TODO
        return true;
    }

    public float DistanceFromCenter(Vector3 position) {
        return Vector3.Distance(Vector3.zero, position);
    }

    public Vector2 GetCanonicalCoordinates(Vector2 originalCoordinates) {
        float latitude = originalCoordinates.x;
        float longitude = originalCoordinates.y;
        // Get canonical latitude in a full period (-PI to PI)
        float canonicalLatitude = MathUtils.CanonicalModulus(latitude + Mathf.PI, Mathf.PI * 2) - Mathf.PI;
        // If lat is out of bounds, adjust lat and long together
        if (canonicalLatitude > Mathf.PI / 2) {
            canonicalLatitude = Mathf.PI - canonicalLatitude;
            longitude += Mathf.PI;
        } else if (canonicalLatitude < -Mathf.PI / 2) {
            canonicalLatitude = -Mathf.PI - canonicalLatitude;
            longitude += Mathf.PI;
        }
        // Get the canonical longitude 
        float canonicalLongitude = MathUtils.CanonicalModulus(longitude + Mathf.PI, Mathf.PI * 2) - Mathf.PI;
        return new Vector2(canonicalLatitude, canonicalLongitude);
    }

    public Vector3 GetPositionFromCoordinate(Vector2 coordinate) {
        //Vector3 origin = new Vector3(0, 0, 1);
        //Quaternion rotation = Quaternion.Euler(coordinate.x * Mathf.Rad2Deg, coordinate.y * Mathf.Rad2Deg, 0);
        //return rotation * origin;
        return new Vector3(
            scale * Mathf.Cos(coordinate.x) * Mathf.Cos(coordinate.y),
            scale * Mathf.Sin(coordinate.x),
            scale * Mathf.Cos(coordinate.x) * Mathf.Sin(coordinate.y)
        );
    }

    public MeshHelper GetMeshHelper(Chunk chunk, int interval, int borderSize) {
        return new SphericalMeshHelper(chunk, interval, borderSize);
    }

    public int GetChunkCount(int interval, int borderSize) {
        int n = chunkWidth / interval + 1 + 3 * borderSize; // Use 3 here for 2 on top, 1 on bottom
        return n * (n + 1) / 2;
    }

    // Get the grid unit length for the longitude given the latitude
    public float GetLongitudeGridUnitFromLatitude(float latitude) {
        float pointRowIndex = Mathf.Round(((Mathf.PI / 2f) - Mathf.Abs(latitude)) / gridUnit);
        if (pointRowIndex == 0f) {
            return 0f;
        }
        return Mathf.PI / (2f * pointRowIndex);
    }

    // Get the chunk unit length for the longitude given the latitude
    public float GetLongitudeChunkUnitFromLatitude(float latitude) {
        float chunkRowIndex = Mathf.Round(((Mathf.PI / 2f) - Mathf.Abs(latitude) - (chunkUnit / 2f)) / chunkUnit);
        //if (chunkRowIndex == 0) {
        //    Debug.Log(string.Format("should be 1.57 {0}", (Mathf.PI / 2f) / (2f * chunkRowIndex + 1f)));
        //}
        return (Mathf.PI / 2f) / (2f * chunkRowIndex + 1f);
    }
}

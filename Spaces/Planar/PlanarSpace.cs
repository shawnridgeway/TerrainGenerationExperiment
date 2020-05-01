using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Note on directions
 * 
 *       1
 *       |
 *   2 ----- 0
 *       |
 *       3
 */

public class PlanarSpace : ChunkedSpace {

    public enum Direction {
        E = 0,
        N = 1,
        W = 2,
        S = 3
    }

    private static readonly int cardinality = 2; // Number of dimensions

    public static readonly int chunkWidth = 240; // How many coordinate units across each chunk is
    public static readonly int chunkPointCount = 241; // How many points across each chunk has (chunkWidth + 1)
    // The GridUnit here is kept at 1, so Coordinates == GridUnits here

    private readonly float scale; // Distance between Positions

    public PlanarSpace(float scale = 1) {
        this.scale = scale;
    }

    public int GetCardinality() {
        return cardinality;
    }

    // Scaled size of chunk
    public float GetChunkScale() {
        return chunkWidth * scale;
    }

    // Map a point in 3D space to one in PlanarSpace
    public Point GetPointFromPosition(Vector3 origin) {
        return new PlanarPoint(new Vector2(
            origin.x / scale,
            origin.z / scale
        ), this);
    }

    // Get a vector that points in the positive direction of the plane at a given position
    public Vector3 GetNormalFromPosition(Vector3 position) {
        return new Vector3(0, 1, 0);
    }

    // Get the distance between two points along the space
    public float GetDistanceBetweenPoints(Point a, Point b) {
        return Vector3.Distance(a.GetPosition(), b.GetPosition());
    }

    // Get the closest Point that falls on the grind in this space
    public Point GetClosestPointTo(Point origin) {
        Vector3 closestPointCoordinate = new Vector2(
            Mathf.Round(origin.GetPosition().x / scale),
            Mathf.Round(origin.GetPosition().z / scale)
        );
        return new PlanarPoint(closestPointCoordinate, this);
    }

    // Get all of the Points that fall within the given distance from the origin
    public Point[] GetPointsWithin(Point origin, float distance) {
        throw new System.NotImplementedException();
    }

    // Check if the two points are within the given distance along this space
    public bool IsPointInRange(Point origin, Point target, float distance) {
        return Vector3.SqrMagnitude(target.GetPosition() - origin.GetPosition())
            < distance * distance;
    }

    // Get the closest Chunk to the given origin
    public Chunk GetClosestChunkTo(Point origin) {
        float chunkScale = GetChunkScale();
        Vector2 closestChunkCenterCoordinate = new Vector2(
            Mathf.Round(origin.GetPosition().x / chunkScale) * chunkWidth,
            Mathf.Round(origin.GetPosition().z / chunkScale) * chunkWidth
        );
        return new PlanarChunk(closestChunkCenterCoordinate, this);
    }

    // Get all of the Chunks that fall within the given distance from the origin
    public Chunk[] GetChunksWithin(Point origin, float distance) {
        Chunk closestChunk = GetClosestChunkTo(origin);
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
                foreach (Chunk neighbor in currentChunk.GetNeighbors()) {
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

    // Check if a Chunk is within the given distance of a Point along this space
    public bool IsChunkInRange(Point origin, Chunk chunk, float distance) {
        float chunkScale = GetChunkScale();
        Bounds bounds = new Bounds(
            chunk.GetCenterPosition(),
            new Vector3(
                chunkScale,
                chunkScale,
                chunkScale
            )
        );
        return bounds.SqrDistance(origin.GetPosition())
            < distance * distance;
    }

    public Vector3 GetPositionFromCoordinate(Vector2 coordinate) {
        return new Vector3(coordinate.x, 0, coordinate.y) * scale;
    }

    public MeshHelper GetMeshHelper(int interval, int borderSize) {
        return new PlanarMeshHelper(interval, borderSize);
    }

    public int GetChunkCount(int interval, int borderSize) {
        int rowCount = (chunkWidth / interval) + 1 + 2 * borderSize;
        return rowCount * rowCount;
    }
}

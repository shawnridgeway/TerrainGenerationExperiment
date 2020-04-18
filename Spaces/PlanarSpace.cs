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
}


public class PlanarChunk : Chunk {
    private readonly Vector2 centerCoordinate;
    private readonly Vector3 centerPostion;
    private readonly PlanarSpace space;

    public PlanarChunk(Vector2 centerCoordinate, PlanarSpace space) {
        this.centerCoordinate = centerCoordinate;
        this.centerPostion = space.GetPositionFromCoordinate(centerCoordinate);
        this.space = space;
    }

    public Vector3 GetCenterPosition() {
        return centerPostion;
    }

    public IEnumerable<Point> GetPoints(int interval = 1, int borderSize = 0) {
        int chunkPointCount = PlanarSpace.chunkPointCount;
        float start = (-chunkPointCount / 2) - (borderSize * interval);
        float end = (chunkPointCount / 2) + (borderSize * interval);
        for (float x = start; x <= end; x += interval) {
            for (float y = start; y <= end; y += interval) {
                yield return new PlanarPoint(
                    new Vector2(x, y) + centerCoordinate,
                    space
                );
            }
        }
    }

    public IEnumerable<Chunk> GetNeighbors() {
        yield return GetNeighbor(PlanarSpace.Direction.E);
        yield return GetNeighbor(PlanarSpace.Direction.N);
        yield return GetNeighbor(PlanarSpace.Direction.W);
        yield return GetNeighbor(PlanarSpace.Direction.S);
    }

    public Chunk GetNeighbor(Enum direction) {
        float chunkWidth = PlanarSpace.chunkWidth;
        Vector2 offsetVector = Vector3.zero;
        switch (direction) {
            case PlanarSpace.Direction.E:
                offsetVector = new Vector2(1, 0);
                break;
            case PlanarSpace.Direction.N:
                offsetVector = new Vector2(0, 1);
                break;
            case PlanarSpace.Direction.W:
                offsetVector = new Vector2(-1, 0);
                break;
            case PlanarSpace.Direction.S:
                offsetVector = new Vector2(0, -1);
                break;
        }
        return new PlanarChunk(centerCoordinate + offsetVector * chunkWidth, space);
    }

    public bool IsPositionInChunk(Vector3 position) {
        Point point = space.GetPointFromPosition(position);
        return space.IsChunkInRange(point, this, Mathf.Epsilon);
    }

    public float GetScale() {
        return space.GetChunkScale();
    }

    public override int GetHashCode() {
        return centerCoordinate.ToString().GetHashCode();
    }

    public override bool Equals(object obj) {
        if (obj is PlanarChunk) {
            return ((PlanarChunk)obj).GetHashCode() == GetHashCode();
        }
        return Equals(obj);
    }
}


public class PlanarPoint : Point {
    private readonly Vector2 coordinate;
    private readonly Vector3 position;
    private readonly PlanarSpace space;

    public PlanarPoint(Vector2 coordinate, PlanarSpace space) {
        this.coordinate = coordinate;
        this.position = space.GetPositionFromCoordinate(coordinate);
        this.space = space;
    }

    public Vector3 GetPosition() {
        return position;
    }

    public float GetDistanceToPoint(Point point) {
        return space.GetDistanceBetweenPoints(this, point);
    }

    public IEnumerable<Point> GetNeighbors() {
        yield return GetNeighbor(PlanarSpace.Direction.E);
        yield return GetNeighbor(PlanarSpace.Direction.N);
        yield return GetNeighbor(PlanarSpace.Direction.W);
        yield return GetNeighbor(PlanarSpace.Direction.S);
    }

    public Point GetNeighbor(Enum direction) {
        Vector2 offsetVector = Vector3.zero;
        switch (direction) {
            case PlanarSpace.Direction.E:
                offsetVector = new Vector2(1, 0);
                break;
            case PlanarSpace.Direction.N:
                offsetVector = new Vector2(0, 1);
                break;
            case PlanarSpace.Direction.W:
                offsetVector = new Vector2(-1, 0);
                break;
            case PlanarSpace.Direction.S:
                offsetVector = new Vector2(0, -1);
                break;
        }
        return new PlanarPoint(coordinate + offsetVector, space);
    }

    public IEnumerable<Point> GetBorderPoints(int borderSize) {
        for (float x = -borderSize; x <= borderSize; x++) {
            for (float y = -borderSize; y <= borderSize; y ++) {
                yield return new PlanarPoint(new Vector2(x, y) + coordinate, space);
            }
        }
    }

    public Point MapPoint(Func<Vector3, Vector3> mapFunction) {
        return space.GetPointFromPosition(mapFunction(position));
    }

    public override int GetHashCode() {
        return coordinate.ToString().GetHashCode();
    }

    public override bool Equals(object obj) {
        if (obj is PlanarPoint) {
            return ((PlanarPoint)obj).GetHashCode() == GetHashCode();
        }
        return Equals(obj);
    }
}
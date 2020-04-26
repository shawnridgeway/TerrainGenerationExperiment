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

    public TriangleGenerator GetTriangleGenerator(int interval, int borderSize) {
        return new PlanarTriangleGenerator(interval, borderSize);
    }

    public int GetChunkCount(int interval, int borderSize) {
        int rowCount = (chunkWidth / interval) + 1 + 2 * borderSize;
        return rowCount * rowCount;
    }
}


public readonly struct PlanarChunk : Chunk {
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
        for (float y = end; y >= start; y -= interval) {
            for (float x = start; x <= end; x += interval) {
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

    public override bool Equals(object otherObj) {
        if (otherObj is PlanarChunk otherChunk) {
            return otherChunk.GetHashCode() == GetHashCode();
        }
        return base.Equals(otherObj);
    }
}


public readonly struct PlanarPoint : Point {
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

    public IEnumerable<Point> GetNeighbors(int interval = 1) {
        yield return GetNeighbor(PlanarSpace.Direction.E, interval);
        yield return GetNeighbor(PlanarSpace.Direction.N, interval);
        yield return GetNeighbor(PlanarSpace.Direction.W, interval);
        yield return GetNeighbor(PlanarSpace.Direction.S, interval);
    }

    public Point GetNeighbor(Enum direction, int interval = 1) {
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
        return new PlanarPoint(coordinate + offsetVector * interval, space);
    }

    public IEnumerable<Point> GetBorderPoints(int borderSize = 1, int interval = 1) {
        for (float x = -borderSize * interval; x <= borderSize * interval; x += interval) {
            for (float y = -borderSize * interval; y <= borderSize * interval; y += interval) {
                yield return new PlanarPoint(new Vector2(x, y) + coordinate, space);
            }
        }
    }

    public Point MapPoint(Func<Vector3, Vector3> mapFunction) {
        return space.GetPointFromPosition(mapFunction(GetPosition()));
    }

    public (Point, Point, Point)[] GetTrianglesForPoint(int interval) {
        (Point, Point, Point)[] triangles = new (Point, Point, Point)[2];
        Point pointN = GetNeighbor(PlanarSpace.Direction.N, interval);
        Point pointW = GetNeighbor(PlanarSpace.Direction.W, interval);
        Point pointNW = GetNeighbor(PlanarSpace.Direction.N, interval)
            .GetNeighbor(PlanarSpace.Direction.W, interval);
        triangles[0] = (this, pointW, pointNW);
        triangles[1] = (this, pointNW, pointN);
        return triangles;
    }

    public override int GetHashCode() {
        return coordinate.ToString().GetHashCode();
    }

    public override bool Equals(object otherObj) {
        if (otherObj is PlanarPoint otherPoint) {
            return otherPoint.GetHashCode() == GetHashCode();
        }
        return base.Equals(otherObj);
    }

    public override String ToString() {
        return coordinate.ToString();
    }

    // Return only triangles that are in border or chunk
    //public (int a, int b, int c, bool triangleIsInChunk)[] GetTriangleIndiciesForPoint(int index, bool pointIsInChunk, int interval, int borderSize) {
    //    (int, int, int, bool)[] triangles;
    //    int rowCount = PlanarSpace.chunkWidth / interval + 1;
    //    int rowWithBorderCount = rowCount + 2 * borderSize;
    //    int pointN, pointW, pointNW;
    //    if (pointIsInChunk) {
    //        bool isOnEdgeN = index < rowCount;
    //        bool isOnEdgeW = index % rowCount == 0;
    //        triangles = new (int, int, int, bool)[2];
    //        pointN = isOnEdgeN ? index + borderSize
    //            : index - rowCount;
    //        pointW = isOnEdgeW ? index + rowWithBorderCount + (index / rowCount) * borderSize * 2
    //            : index - 1;
    //        pointNW = isOnEdgeN && isOnEdgeW ? 0
    //            : isOnEdgeN ? pointN - 1
    //            : isOnEdgeW ? pointW - borderSize * 2
    //            : index - rowCount - 1;
    //        triangles[0] = (index, pointW, pointNW, !isOnEdgeN && !isOnEdgeW);
    //        triangles[1] = (index, pointNW, pointN, !isOnEdgeN && !isOnEdgeW);
    //    } else {
    //        bool isOnEdgeN = index < rowWithBorderCount;
    //        bool isOnEdgeW = (index - rowWithBorderCount) % (borderSize * 2) == 0;
    //        bool isOnEdgeS = index >= rowWithBorderCount + (borderSize * 2) * rowCount;
    //        bool isOnEdgeE = (index - rowWithBorderCount) % (borderSize * 2) == borderSize * 2 - 1;
    //        if (isOnEdgeN || isOnEdgeW) {
    //            return new (int, int, int, bool)[0];
    //        }
    //        triangles = new (int, int, int, bool)[2];
    //        // TODO
    //        //pointN = isOnEdgeN ? index + borderSize
    //        //    : index - rowCount;
    //        //pointW = isOnEdgeW ? index + rowWithBorderCount + (index / rowCount) * borderSize * 2
    //        //    : index - 1;
    //        //pointNW = isOnEdgeN && isOnEdgeW ? 0
    //        //    : isOnEdgeN ? pointN - 1
    //        //    : isOnEdgeW ? pointW - borderSize * 2
    //        //    : index - rowCount - 1;
    //        triangles[0] = (index, pointW, pointNW, !isOnEdgeN && !isOnEdgeW);
    //        triangles[1] = (index, pointNW, pointN, !isOnEdgeN && !isOnEdgeW);
    //    }
    //    return triangles;
    //}

    
}

public struct PlanarTriangleGenerator : TriangleGenerator {
    private readonly int interval;
    private readonly int borderSize;
    private readonly int rowCount;
    private readonly int rowWithBorderCount;

    public PlanarTriangleGenerator(int interval, int borderSize) {
        this.interval = interval;
        this.borderSize = borderSize;
        rowCount = PlanarSpace.chunkWidth / interval + 1;
        rowWithBorderCount = rowCount + 2 * borderSize;
    }

    public (int, int, int)[] GetTriangleIndiciesForPoint(int absIndex) {
        // If the point is on the N or W edge, then we do not include the triangles
        bool isOnEdgeN = absIndex < rowWithBorderCount;
        bool isOnEdgeW = absIndex % rowWithBorderCount == 0;
        if (isOnEdgeN || isOnEdgeW) {
            return new (int, int, int)[0];
        }
        // Otherwise, get the 2 triangle for this point
        (int, int, int)[] triangles = new (int, int, int)[2];
        int absIndexN = absIndex - rowWithBorderCount;
        int absIndexW = absIndex - 1;
        int absIndexNW = absIndex - rowWithBorderCount - 1;

        triangles[0] = (
            ConvertAbsToRelativeIndex(absIndex),
            ConvertAbsToRelativeIndex(absIndexW),
            ConvertAbsToRelativeIndex(absIndexNW)
        );
        triangles[1] = (
            ConvertAbsToRelativeIndex(absIndex),
            ConvertAbsToRelativeIndex(absIndexNW),
            ConvertAbsToRelativeIndex(absIndexN)
        );
        return triangles;
    }

    private int ConvertAbsToRelativeIndex(int absIndex) {
        // N border
        if (absIndex < rowWithBorderCount * borderSize) {
            return GetBorderIndexAsNegative(absIndex);
        }
        // S border
        if (absIndex >= rowWithBorderCount * (rowWithBorderCount - borderSize)) {
            return GetBorderIndexAsNegative(absIndex - rowCount * rowCount);
        }
        int colIndex = absIndex % rowWithBorderCount;
        int rowIndex = absIndex / rowWithBorderCount;
        // W border
        if (colIndex < borderSize) {
            return GetBorderIndexAsNegative(rowWithBorderCount * borderSize + 2 * borderSize * (rowIndex - borderSize));
        }
        // E border
        if (colIndex >= rowWithBorderCount - borderSize) {
            return GetBorderIndexAsNegative(rowWithBorderCount * borderSize + 2 * borderSize * (rowIndex - borderSize) + borderSize);
        }
        // in chunk
        return absIndex - rowWithBorderCount * borderSize - (2 * borderSize * (rowIndex - borderSize) + borderSize);
    }

    private int GetBorderIndexAsNegative(int positiveIndex) {
        return (positiveIndex + 1) * -1;
    }

    public Vector2 GetUv(int indexInChunk) {
        int colIndex = indexInChunk % rowWithBorderCount;
        int rowIndex = indexInChunk / rowWithBorderCount;
        return new Vector2(
            0.5f + (colIndex / (float)rowCount),
            0.5f - (rowIndex / (float)rowCount)
        );
    }

    public bool IsInChunk(int absIndex) {
        return absIndex >= rowWithBorderCount * borderSize && // On N side
            absIndex % rowWithBorderCount >= borderSize && // On W side
            absIndex % rowWithBorderCount < rowWithBorderCount - borderSize && // On E side
            absIndex < rowWithBorderCount * (rowWithBorderCount - borderSize); // On S side
    }
}
using System.Collections.Generic;
using UnityEngine;
using System;

/* Glossary
 *
 * - Space: A system that dictates which subset of all possible 3D space
 *     are to be included in this terrain and how they relate to eachother.
 *
 * - Chunk: A subset of the Space which breaks down the total size of the Space
 *     into renderable pieces.
 *
 * - Point: A location that lies within the given Space.
 *
 * - Position: In contrast to Point, the term Position is used to describe a
 *     3D vector representing a location that lies anywhere in 3D space, not
 *     necessarily within a given Space.
 *
 * - Coordinate: The internal coordinate representation of a Point (cartesian, polar...)
 *
 * - GridUnit: The distance (in Coordinates) between 2 Points in the highest LOD.
*/

public interface Space {
    int GetCardinality();
    Point GetPointFromPosition(Vector3 position);
    Vector3 GetNormalFromPosition(Vector3 position);
    float GetDistanceBetweenPoints(Point a, Point b);
    Point GetClosestPointTo(Point origin);
    Point[] GetPointsWithin(Point origin, float distance);
    bool IsPointInRange(Point origin, Point point, float distance);
    MeshHelper GetMeshHelper(int interval, int borderSize);
}

public interface ChunkedSpace : Space {
    float GetChunkScale();
    int GetChunkCount(int interval, int borderSize);
    Chunk GetClosestChunkTo(Point origin);
    Chunk[] GetChunksWithin(Point origin, float distance);
    bool IsChunkInRange(Point origin, Chunk chunk, float distance);
}

public interface Chunk {
    float GetScale();
    Vector3 GetCenterPosition(); // Identifier
    IEnumerable<Point> GetPoints(int interval, int borderSize);
    Chunk GetNeighbor(Enum direction);
    IEnumerable<Chunk> GetNeighbors();
    bool IsPositionInChunk(Vector3 position);
}

public interface Point {
    Vector3 GetPosition(); // Identifier
    Point GetNeighbor(Enum direction, int interval);
    IEnumerable<Point> GetNeighbors(int interval);
    IEnumerable<Point> GetBorderPoints(int borderSize, int interval);
    Point MapPoint(Func<Vector3, Vector3> mapFunction);
    float GetDistanceToPoint(Point otherPoint);
    (Point, Point, Point)[] GetTrianglesForPoint(int interval);
}

public interface MeshHelper {
    (int, int, int)[] GetTriangleIndiciesForPoint(int absIndex);
    bool IsInChunk(int absIndex);
    Vector2 GetUv(int indexInChunk);
}
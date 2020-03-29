using System.Collections.Generic;
using UnityEngine;
using System;


public interface Space {
    int GetCardinality();
    Point GetPointInSpace(Vector3 location);
    Point GetClosestPointTo(Point origin);
    Point[] GetPointsWithin(Point origin, float distance);
    bool IsPointInRange(Point origin, Point point, float distance);
}

public interface ChunkedSpace : Space {
    int GetChunkSize();
    Chunk GetClosestChunkTo(Point origin);
    Chunk[] GetChunksWithin(Point origin, float distance);
    bool IsChunkInRange(Point origin, Chunk chunk, float distance);
}

public interface Chunk {
    int GetSize();
    Vector3 GetCenterLocation(); // Identifier
    IEnumerable<Point> GetPoints(int interval, int borderSize);
    IEnumerable<Chunk> GetNeighbors();
}

public interface Point {
    Vector3 GetLocation(); // Identifier
    IEnumerable<Point> GetNeighbors();
    IEnumerable<Point> GetBorderPoints(int borderSize);
    Point MapPoint(Func<Vector3, Vector3> mapFunction);
}

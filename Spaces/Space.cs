using System.Collections.Generic;
using UnityEngine;
using System;


public interface Space {
    Point[] GetPointsWithin(Vector3 origin, float distance);
    Point GetClosestPointTo(Vector3 origin);
    int GetCardinality();
}

public interface ChunkedSpace : Space {
    int GetChunkSize();
    Chunk[] GetChunksWithin(Vector3 origin, float distance);
}

public interface Chunk {
    Vector3 GetCenterLocation(); // Identifier
    IEnumerable<Point> GetPoints(int interval, int borderSize);
    IEnumerable<Chunk> GetNeighbors();
    int GetSize();
}

public interface Point {
    Vector3 GetLocation(); // Identifier
    IEnumerable<Point> GetNeighbors();
    IEnumerable<Point> GetBorderPoints(int borderSize);
    Point MapPoint(Func<Vector3, Vector3> mapFunction);
}

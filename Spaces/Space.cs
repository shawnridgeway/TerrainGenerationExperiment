using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface Space {
    Point[] GetPointsWithin(Vector3 origin, float distance);
    int GetCardinality();
}

public interface ChunkedSpace : Space {
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
}

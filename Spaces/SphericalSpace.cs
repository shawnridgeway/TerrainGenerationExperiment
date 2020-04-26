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
    public readonly float gridUnit; // Distance between Points (in Coordinates on latitude only)
    public readonly float chunkUnit; // Length of an edge of a chunk (in Coordinates on latitude only)

    private readonly float scale = 1; // Distance between Positions, essentially radius

    public SphericalSpace(float scale = 1, int divisions = 1) {
        this.scale = scale;
        chunkUnit = Mathf.PI / (2 * divisions);
        gridUnit = chunkUnit / chunkWidth;
    }

    public int GetCardinality() {
        return cardinality;
    }

    public float GetChunkScale() {
        return chunkUnit * scale;
    }

    public float GetGridUnit() {
        return gridUnit;
    }

    public Point GetPointFromPosition(Vector3 position) {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public Chunk[] GetChunksWithin(Point origin, float distance) {
        throw new NotImplementedException();
    }

    public bool IsChunkInRange(Point origin, Chunk chunk, float distance) {
        throw new NotImplementedException();
    }

    public float DistanceFromCenter(Vector3 position) {
        return Vector3.Distance(Vector3.zero, position);
    }

    public Vector2 GetCanonicalCoordinates(Vector2 originalCoordinates) {
        return new Vector2(
            (originalCoordinates.x + Mathf.PI / 2) % Mathf.PI - Mathf.PI / 2,
            (originalCoordinates.y + Mathf.PI) % Mathf.PI * 2 - Mathf.PI
        );
    }

    public Vector3 GetPositionFromCoordinate(Vector2 coordinate) {
        return new Vector3(
            scale * Mathf.Cos(coordinate.x) * Mathf.Cos(coordinate.y),
            scale * Mathf.Cos(coordinate.x) * Mathf.Sin(coordinate.y),
            scale * Mathf.Sin(coordinate.x)
        );
    }

    public TriangleGenerator GetTriangleGenerator(int interval, int borderSize) {
        throw new NotImplementedException();
    }

    public int GetChunkCount(int interval, int borderSize) {
        throw new NotImplementedException();
    }
}

public class SphericalChunk : Chunk {
    private readonly Vector2 centerCoordinate;
    private readonly Vector3 centerPosition;
    private readonly SphericalSpace space;

    public SphericalChunk(Vector2 centerCoordinate, SphericalSpace space) {
        this.centerCoordinate = space.GetCanonicalCoordinates(centerCoordinate);
        this.centerPosition = space.GetPositionFromCoordinate(this.centerCoordinate);
        this.space = space;
    }

    public Vector3 GetCenterPosition() {
        return centerPosition;
    }

    public float GetScale() {
        return space.GetChunkScale();
    }

    // Get the grid unit length for the longitude given the latitude
    private float GetChunkLongitudeUnitFromLatitude(float latitude) {
        int pointRowIndex = Mathf.RoundToInt(((Mathf.PI / 2) - Mathf.Abs(latitude)) / space.gridUnit);
        if (pointRowIndex == 0) {
            return 0;
        }
        return Mathf.PI / (2 * pointRowIndex);
    }

    // Get which row along the latitudal direction from the pole the the chunk is in
    private int GetChunkRowIndex() {
        float chunkUnit = space.chunkUnit;
        return Mathf.RoundToInt((centerCoordinate.x - (chunkUnit / 2)) / chunkUnit);
    }

    // Get which column alont the longitudal direction from the left of the octant the the chunk is in
    private int GetChunkColIndex() {
        int rowIndex = GetChunkRowIndex();
        float longitudeInOctant = MathUtils.CanonicalModulus(centerCoordinate.y, Mathf.PI / 2);
        return Mathf.RoundToInt(longitudeInOctant / (Mathf.PI / (4 * (rowIndex + 1)))) - 1;
    }

    // Determine if the chunk is inverted (the top points toward the equator)
    // A chunk is inverted if it is in an odd index in a row in the octant
    private bool IsChunkInverted() {
        int colIndex = GetChunkColIndex();
        return MathUtils.CanonicalModulus(colIndex, 2) == 1;
    }

    public IEnumerable<Point> GetPoints(int interval, int borderSize) {
        //bool isInverted = IsChunkInverted();
        float chunkUnit = space.chunkUnit;
        //float start = (-chunkPointCount / 2) - (borderSize * interval);
        //float end = (chunkPointCount / 2) + (borderSize * interval);

        // Not inverted case
        float latitudeStart = centerCoordinate.x - (chunkUnit / 2);
        float latitudeEnd = centerCoordinate.x + (chunkUnit / 2);
        float gridInterval = interval * space.gridUnit;
        int rowIndex = 0;
        for (float x = latitudeStart; x <= latitudeEnd; x += gridInterval) {
            float longitudeUnit = GetChunkLongitudeUnitFromLatitude(x);
            // TODO: this assumes not inverted case vvvvvvvvvvvvvvvvvvvvvv
            float longitudeStart = longitudeUnit * (GetChunkColIndex() / 2) * SphericalSpace.chunkWidth;
            for (int colIndex = 0; colIndex <= rowIndex; colIndex += interval) {
                float y = longitudeStart + colIndex * longitudeUnit;
                yield return new SphericalPoint(
                    new Vector2(x, y),
                    space
                );
            }
            rowIndex++;
        }
    }

    public IEnumerable<Chunk> GetNeighbors() {
        yield return GetNeighbor(SphericalSpace.Direction.E);
        yield return GetNeighbor(SphericalSpace.Direction.NE);
        yield return GetNeighbor(SphericalSpace.Direction.NW);
        yield return GetNeighbor(SphericalSpace.Direction.W);
        yield return GetNeighbor(SphericalSpace.Direction.SW);
        yield return GetNeighbor(SphericalSpace.Direction.SE);
    }

    public Chunk GetNeighbor(Enum direction) {
        int chunkWidth = SphericalSpace.chunkWidth;
        float gridUnit = space.GetGridUnit();
        Vector2 offsetVector = Vector3.zero;
        switch (direction) {
            case SphericalSpace.Direction.E:
                offsetVector = new Vector2(0, 1);
                break;
            case SphericalSpace.Direction.NE:
                offsetVector = new Vector2(1, 1);
                break;
            case SphericalSpace.Direction.NW:
                offsetVector = new Vector2(1, -1);
                break;
            case SphericalSpace.Direction.W:
                offsetVector = new Vector2(0, -1);
                break;
            case SphericalSpace.Direction.SW:
                offsetVector = new Vector2(-1, -1);
                break;
            case SphericalSpace.Direction.SE:
                offsetVector = new Vector2(-1, 1);
                break;
        }
        return new SphericalChunk(centerCoordinate + offsetVector * gridUnit * chunkWidth, space);
    }

    public bool IsPositionInChunk(Vector3 position) {
        Point point = space.GetPointFromPosition(position);
        return space.IsChunkInRange(point, this, 0);
    }

    public override int GetHashCode() {
        return centerCoordinate.ToString().GetHashCode();
    }

    public override bool Equals(object otherObj) {
        if (otherObj is SphericalChunk otherChunk) {
            return otherChunk.GetHashCode() == GetHashCode();
        }
        return base.Equals(otherObj);
    }
}

public class SphericalPoint : Point {
    private readonly Vector2 coordinate; // x = latitude -PI/2 to PI/2, y = longitude -PI to PI
    private readonly Vector3 position;
    private readonly SphericalSpace space;

    public SphericalPoint(Vector2 coordinate, SphericalSpace space) {
        this.coordinate = space.GetCanonicalCoordinates(coordinate);
        this.position = space.GetPositionFromCoordinate(this.coordinate);
        this.space = space;
    }

    public Vector3 GetPosition() {
        return position;
    }

    public float GetDistanceToPoint(Point point) {
        return space.GetDistanceBetweenPoints(this, point);
    }

    public IEnumerable<Point> GetNeighbors(int interval = 1) {
        yield return GetNeighbor(SphericalSpace.Direction.E, interval);
        yield return GetNeighbor(SphericalSpace.Direction.NE, interval);
        yield return GetNeighbor(SphericalSpace.Direction.NW, interval);
        yield return GetNeighbor(SphericalSpace.Direction.W, interval);
        yield return GetNeighbor(SphericalSpace.Direction.SW, interval);
        yield return GetNeighbor(SphericalSpace.Direction.SE, interval);
    }

    public Point GetNeighbor(Enum direction, int interval = 1) {
        float gridUnit = space.GetGridUnit();
        Vector2 offsetVector = Vector3.zero;
        switch (direction) {
            case SphericalSpace.Direction.E:
                offsetVector = new Vector2(0, 1);
                break;
            case SphericalSpace.Direction.NE:
                offsetVector = new Vector2(1, 1);
                break;
            case SphericalSpace.Direction.NW:
                offsetVector = new Vector2(1, -1);
                break;
            case SphericalSpace.Direction.W:
                offsetVector = new Vector2(0, -1);
                break;
            case SphericalSpace.Direction.SW:
                offsetVector = new Vector2(-1, -1);
                break;
            case SphericalSpace.Direction.SE:
                offsetVector = new Vector2(-1, 1);
                break;
        }
        return new SphericalPoint(coordinate + offsetVector * gridUnit * interval, space);
    }

    public IEnumerable<Point> GetBorderPoints(int borderSize = 1, int interval = 1) {
        throw new NotImplementedException();
    }

    public Point MapPoint(Func<Vector3, Vector3> mapFunction) {
        throw new NotImplementedException();
    }

    public (Point, Point, Point)[] GetTrianglesForPoint(int interval) {
        (Point, Point, Point)[] triangles = new (Point, Point, Point)[2];
        Point pointNE, pointNW, pointW;
        pointW = GetNeighbor(SphericalSpace.Direction.W, interval);
        pointNW = GetNeighbor(SphericalSpace.Direction.NW, interval);
        pointNE = GetNeighbor(SphericalSpace.Direction.NE, interval);
        triangles[0] = (this, pointW, pointNW);
        triangles[1] = (this, pointNW, pointNE);
        return triangles;
    }

    public override int GetHashCode() {
        return coordinate.ToString().GetHashCode();
    }

    public override bool Equals(object otherObj) {
        if (otherObj is SphericalPoint otherPoint) {
            return otherPoint.GetHashCode() == GetHashCode();
        }
        return base.Equals(otherObj);
    }
}
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

    public MeshHelper GetMeshHelper(int interval, int borderSize) {
        throw new NotImplementedException();
    }

    public int GetChunkCount(int interval, int borderSize) {
        throw new NotImplementedException();
    }
}

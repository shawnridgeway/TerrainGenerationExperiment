﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Note on coordinates
 * 
 *       1
 *       |
 *   2 ----- 0
 *       |
 *       3
 *
 *     2  1   
 *      \/
 *   3 ---- 0 
 *      /\
 *     4  5
 */


public class CartesianSpace : ChunkedSpace {

    public enum Direction {
        E = 0,
        N = 1,
        W = 2,
        S = 3
    }

    private readonly int chunkSize = 240; // Width, not point count
    private readonly int cardinality = 2; // Number of dimensions
    private readonly float scale; // Distance between points

    public CartesianSpace(float scale = 1) {
        this.scale = scale;
    }

    public Chunk[] GetChunksWithin(Vector3 origin, float distance) {
        Chunk closestChunk = GetClosestChunk(origin);
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

    public Point[] GetPointsWithin(Vector3 origin, float distance) {
        throw new System.NotImplementedException();
    }

    public int GetCardinality() {
        return cardinality;
    }

    Chunk GetClosestChunk(Vector3 origin) {
        Vector3 closestChunkCenter = new Vector3(
            Mathf.Round(origin.x / chunkSize / scale) * chunkSize * scale,
            0,
            Mathf.Round(origin.z / chunkSize / scale) * chunkSize * scale
        );
        return new CartesianChunk(closestChunkCenter, scale, chunkSize);
    }

    bool IsChunkInRange(Vector3 origin, Chunk chunk, float distance) {
        Bounds bounds = new Bounds(chunk.GetCenterLocation(), new Vector3(chunkSize * scale, chunkSize * scale, chunkSize * scale));
        return bounds.SqrDistance(origin) <= distance * distance;
    }
}


public class CartesianChunk : Chunk {
    private readonly Vector3 centerLoaction;
    private readonly float scale;
    private readonly int chunkSize;

    public CartesianChunk(Vector3 centerLoaction, float scale, int chunkSize) {
        this.centerLoaction = centerLoaction;
        this.scale = scale;
        this.chunkSize = chunkSize;
    }

    public Vector3 GetCenterLocation() {
        return centerLoaction;
    }

    public IEnumerable<Point> GetPoints(int interval = 1, int borderSize = 0) {
        int pointCount = chunkSize + 1;
        float start = (-pointCount / 2 - (borderSize * interval));
        float end = (pointCount / 2 + (borderSize * interval));
        for (float x = start; x <= end; x += interval) {
            for (float y = start; y <= end; y += interval) {
                yield return new CartesianPoint(new Vector3(x, 0, y) * scale + centerLoaction, scale);
            }
        }
    }

    public IEnumerable<Chunk> GetNeighbors() {
        yield return GetNeighbor(CartesianSpace.Direction.E);
        yield return GetNeighbor(CartesianSpace.Direction.N);
        yield return GetNeighbor(CartesianSpace.Direction.W);
        yield return GetNeighbor(CartesianSpace.Direction.S);
    }

    public Chunk GetNeighbor(CartesianSpace.Direction direction) {
        float xOffset = direction == CartesianSpace.Direction.E ? 1 : direction == CartesianSpace.Direction.W ? -1 : 0;
        float zOffset = direction == CartesianSpace.Direction.N ? 1 : direction == CartesianSpace.Direction.S ? -1 : 0;
        return new CartesianChunk(
            new Vector3(centerLoaction.x + xOffset * chunkSize * scale, 0, centerLoaction.z + zOffset * chunkSize * scale),
            scale,
            chunkSize
        );
    }

    public int GetSize() {
        return chunkSize;
    }

    public override int GetHashCode() {
        return centerLoaction.ToString().GetHashCode();
    }

    public override bool Equals(object obj) {
        if (obj is CartesianChunk) {
            return ((CartesianChunk)obj).GetHashCode() == GetHashCode();
        }
        return Equals(obj);
    }
}


public class CartesianPoint : Point {
    private readonly Vector3 location;
    private readonly float scale;

    public CartesianPoint(Vector3 location, float scale) {
        this.location = location;
        this.scale = scale;
    }

    public Vector3 GetLocation() {
        return location;
    }

    public IEnumerable<Point> GetNeighbors() {
        yield return GetNeighbor(CartesianSpace.Direction.E);
        yield return GetNeighbor(CartesianSpace.Direction.N);
        yield return GetNeighbor(CartesianSpace.Direction.W);
        yield return GetNeighbor(CartesianSpace.Direction.S);
    }

    public Point GetNeighbor(CartesianSpace.Direction direction) {
        float xOffset = direction == CartesianSpace.Direction.E ? 1 : direction == CartesianSpace.Direction.W ? -1 : 0;
        float zOffset = direction == CartesianSpace.Direction.N ? 1 : direction == CartesianSpace.Direction.S ? -1 : 0;
        return new CartesianPoint(
            new Vector3(location.x + xOffset * scale, 0, location.z + zOffset * scale),
            scale
        );
    }

    public IEnumerable<Point> GetBorderPoints(int borderSize) {
        for (float x = -borderSize; x <= borderSize; x += scale) {
            for (float y = -borderSize; y <= borderSize; y += scale) {
                yield return new CartesianPoint(new Vector3(x, 0, y) + location, scale);
            }
        }
    }

    public Point MapPoint(Func<Vector3, Vector3> mapFunction) {
        return new CartesianPoint(mapFunction(location), scale);
    }

    public override int GetHashCode() {
        return location.ToString().GetHashCode();
    }

    public override bool Equals(object obj) {
        if (obj is CartesianPoint) {
            return ((CartesianPoint)obj).GetHashCode() == GetHashCode();
        }
        return Equals(obj);
    }
}
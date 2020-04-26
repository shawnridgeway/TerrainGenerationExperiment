using System;
using UnityEngine;
using System.Collections.Generic;

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


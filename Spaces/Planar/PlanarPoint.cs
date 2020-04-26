using System;
using UnityEngine;
using System.Collections.Generic;

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
}

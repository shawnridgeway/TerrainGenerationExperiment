using System;
using UnityEngine;
using System.Collections.Generic;

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

    public Vector2 GetCoordinate() {
        return coordinate;
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
        float gridUnit = space.gridUnit;
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

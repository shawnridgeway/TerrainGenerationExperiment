using System;
using UnityEngine;
using System.Collections.Generic;

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

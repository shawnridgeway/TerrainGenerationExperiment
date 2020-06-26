using System;
using UnityEngine;
using System.Collections.Generic;

public class SphericalChunk : Chunk {

    // Chunks have 3 neighbors, 2 horizontal and one vertical
    public enum SphericalChunkNeighbors {
        H1, H2, V
    }

    private readonly Vector2 centerCoordinate;
    private readonly Vector3 centerPosition;
    private readonly SphericalSpace space;

    public SphericalChunk(Vector2 centerCoordinate, SphericalSpace space) {
        this.centerCoordinate = space.GetCanonicalCoordinates(centerCoordinate);
        this.centerPosition = space.GetPositionFromCoordinate(this.centerCoordinate);
        this.space = space;
    }

    public Vector2 GetCenterCoordinate() {
        return centerCoordinate;
    }

    public Vector3 GetCenterPosition() {
        return centerPosition;
    }

    public float GetScale() {
        return space.GetChunkScale();
    }

    // Get which row along the latitudal direction from the pole the the chunk is in
    public int GetChunkRowIndex() {
        float chunkUnit = space.chunkUnit;
        return Mathf.RoundToInt((Mathf.PI / 2 - Mathf.Abs(centerCoordinate.x) - (chunkUnit / 2)) / chunkUnit);
    }

    // Get which column along the longitudal direction from the left of the octant the the chunk is in
    public int GetChunkColIndex() {
        int rowIndex = GetChunkRowIndex();
        float longitudeInOctant = MathUtils.CanonicalModulus(centerCoordinate.y, Mathf.PI / 2f);
        return Mathf.RoundToInt(longitudeInOctant / (Mathf.PI / (4f * (rowIndex + 1f)))) - 1;
    }

    // Determine if the chunk is inverted (the top points south)
    // A chunk is inverted if it is in an odd index in a row in the octant (nothern hemisphere)
    // or is in an even index in a row in the octant (southern hemisphere)
    public bool IsChunkInverted() {
        int colIndex = GetChunkColIndex();
        bool isInSouthernHemisphere = IsChunkInSouthernHemisphere();
        bool isInEvenPositionInCol = MathUtils.CanonicalModulus(colIndex, 2) == 0;
        return (
            isInSouthernHemisphere && isInEvenPositionInCol
        ) || (
            !isInSouthernHemisphere && !isInEvenPositionInCol
        );
    }

    private bool IsChunkInSouthernHemisphere() {
        return centerCoordinate.x < 0;
    }

    public float GetChunkLongitudalWidthAtLatitude(float latitude) {
        float longitudeUnit = space.GetLongitudeGridUnitFromLatitude(latitude);
        int rowIndex = GetPointRowIndexInChunk(latitude);
        int countInRow = IsChunkInverted() ? SphericalSpace.chunkWidth - rowIndex : rowIndex;
        return countInRow * longitudeUnit;
    }

    private int GetPointRowIndexInChunk(float latitude) {
        return Mathf.RoundToInt((latitude - GetChunkTopLatitude()) / space.gridUnit);
    }

    private float GetChunkTopLatitude() {
        return centerCoordinate.x + (space.chunkUnit / 2f);
    }

    private float GetChunkQuadrantLongitudeStart() {
        float quadrantIndex = Mathf.Floor(centerCoordinate.y / (Mathf.PI / 2f));
        return (quadrantIndex) * (Mathf.PI / 2f);
    }

    public IEnumerable<Point> GetPoints(int interval, int borderSize) {
        bool isFlatSideUp = IsChunkInverted();
        int edgePointCountInChunk = SphericalSpace.chunkWidth / interval;
        int edgePointCount = edgePointCountInChunk + borderSize * 3;

        // A border adds 1x rows on the flat side, and 2x rows on the pointy end
        int borderStartOffset = isFlatSideUp ? borderSize : borderSize * 2;
        float latitudeUnit = interval * space.gridUnit;
        float inChunkLatitudeStart = GetChunkTopLatitude();
        float latitudeStart = inChunkLatitudeStart + borderStartOffset * latitudeUnit;

        float chunkQuadrantLongitudeStart = GetChunkQuadrantLongitudeStart();

        // Iterate through latitude, top to bottom
        for (int rowIndex = 0; rowIndex <= edgePointCount; rowIndex++) {
            float latitude = latitudeStart - rowIndex * latitudeUnit;

            int colsInRow = isFlatSideUp
                ? edgePointCount - rowIndex
                : rowIndex;

            float longitudeUnit = interval * space.GetLongitudeGridUnitFromLatitude(latitude);
            int colIndexInQuadrantStart = IsChunkInSouthernHemisphere()
                ? isFlatSideUp
                    ? (GetChunkColIndex() / 2) * edgePointCountInChunk
                    : ((GetChunkColIndex() + 1) / 2) * edgePointCountInChunk - rowIndex - borderSize + 3 // Add 3 for the point
                : isFlatSideUp
                    ? ((GetChunkColIndex() - 1) / 2) * edgePointCountInChunk + rowIndex - borderSize
                    : (GetChunkColIndex() / 2) * edgePointCountInChunk;

            // Iterate through longitude, left to right
            for (int colIndex = 0; colIndex <= colsInRow; colIndex++) {
                float longitude = (colIndexInQuadrantStart + colIndex - borderSize) * longitudeUnit + chunkQuadrantLongitudeStart;

                yield return new SphericalPoint(
                    new Vector2(latitude, longitude),
                    space
                );
            }
        }
    }

    public IEnumerable<Chunk> GetNeighbors() {
        yield return GetNeighbor(SphericalChunkNeighbors.H1);
        yield return GetNeighbor(SphericalChunkNeighbors.H2);
        yield return GetNeighbor(SphericalChunkNeighbors.V);
    }

    private Vector2 GetChunkCenterAtIndicies(int rowIndex, int colIndex, bool isInSouthernHemisphere) {
        float centerLatitude = isInSouthernHemisphere
            ? -(Mathf.PI / 2) + (rowIndex + .5f) * space.chunkUnit
            : (Mathf.PI / 2) - (rowIndex + .5f) * space.chunkUnit;
        float chunkUnitLongitude = GetChunkLongitudalWidthAtLatitude(centerLatitude);
        float centerLongitude = (colIndex + .5f) * chunkUnitLongitude + GetChunkQuadrantLongitudeStart();
        return new Vector2(centerLatitude, centerLongitude);
    }

    public Chunk GetNeighbor(Enum direction) {
        switch (direction) {
            case SphericalChunkNeighbors.H1: {
                    float longitudeUnit = space.GetLongitudeChunkUnitFromLatitude(centerCoordinate.x);
                    Vector2 offsetVector = new Vector2(0, longitudeUnit);
                    return new SphericalChunk(centerCoordinate + offsetVector, space);
                }
            case SphericalChunkNeighbors.H2: {
                    float longitudeUnit = space.GetLongitudeChunkUnitFromLatitude(centerCoordinate.x);
                    Vector2 offsetVector = new Vector2(0, -longitudeUnit);
                    return new SphericalChunk(centerCoordinate + offsetVector, space);
                }
            case SphericalChunkNeighbors.V: {
                    bool isChunkInSouthernHemisphere = IsChunkInSouthernHemisphere();
                    int chunkColIndex = GetChunkColIndex(); 
                    int chunkRowIndex = GetChunkRowIndex(); 
                    bool neighborIsAbove = IsChunkInverted();
                    bool neighborIsInSouthernHemisphere;
                    int neighborRowIndex;
                    int neighborColIndex;
                    bool neighborIsAcrossEquator =
                        (isChunkInSouthernHemisphere && neighborIsAbove && chunkRowIndex == space.divisions - 1) ||
                        (!isChunkInSouthernHemisphere && !neighborIsAbove && chunkRowIndex == space.divisions - 1);
                    if (neighborIsAcrossEquator) {
                        neighborIsInSouthernHemisphere = !isChunkInSouthernHemisphere;
                        neighborRowIndex = chunkRowIndex;
                        neighborColIndex = chunkColIndex;
                    } else {
                        neighborIsInSouthernHemisphere = isChunkInSouthernHemisphere;
                        neighborRowIndex = isChunkInSouthernHemisphere
                            ? neighborIsAbove
                                ? chunkRowIndex + 1
                                : chunkRowIndex - 1
                            : neighborIsAbove
                                ? chunkRowIndex - 1
                                : chunkRowIndex + 1;
                        neighborColIndex = isChunkInSouthernHemisphere
                            ? neighborIsAbove
                                ? chunkColIndex + 1
                                : chunkColIndex - 1
                            : neighborIsAbove
                                ? chunkColIndex - 1
                                : chunkColIndex + 1;
                    }
                    Vector2 neigborCoordinates = GetChunkCenterAtIndicies(neighborRowIndex, neighborColIndex, neighborIsInSouthernHemisphere);
                    return new SphericalChunk(neigborCoordinates, space);
                }
            default:
                return null;
        }
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

using System;
using UnityEngine;

public struct PlanarMeshHelper : MeshHelper {
    private readonly int interval;
    private readonly int borderSize;
    private readonly int rowCount;
    private readonly int rowWithBorderCount;

    public PlanarMeshHelper(int interval, int borderSize) {
        this.interval = interval;
        this.borderSize = borderSize;
        rowCount = PlanarSpace.chunkWidth / interval + 1;
        rowWithBorderCount = rowCount + 2 * borderSize;
    }

    public (int, int, int)[] GetTriangleIndiciesForPoint(int absIndex) {
        // If the point is on the N or W edge, then we do not include the triangles
        bool isOnEdgeN = absIndex < rowWithBorderCount;
        bool isOnEdgeW = absIndex % rowWithBorderCount == 0;
        if (isOnEdgeN || isOnEdgeW) {
            return new (int, int, int)[0];
        }
        // Otherwise, get the 2 triangle for this point
        (int, int, int)[] triangles = new (int, int, int)[2];
        int absIndexN = absIndex - rowWithBorderCount;
        int absIndexW = absIndex - 1;
        int absIndexNW = absIndex - rowWithBorderCount - 1;

        triangles[0] = (
            ConvertAbsToRelativeIndex(absIndex),
            ConvertAbsToRelativeIndex(absIndexW),
            ConvertAbsToRelativeIndex(absIndexNW)
        );
        triangles[1] = (
            ConvertAbsToRelativeIndex(absIndex),
            ConvertAbsToRelativeIndex(absIndexNW),
            ConvertAbsToRelativeIndex(absIndexN)
        );
        return triangles;
    }

    private int ConvertAbsToRelativeIndex(int absIndex) {
        // N border
        if (absIndex < rowWithBorderCount * borderSize) {
            return GetBorderIndexAsNegative(absIndex);
        }
        // S border
        if (absIndex >= rowWithBorderCount * (rowWithBorderCount - borderSize)) {
            return GetBorderIndexAsNegative(absIndex - rowCount * rowCount);
        }
        int colIndex = absIndex % rowWithBorderCount;
        int rowIndex = absIndex / rowWithBorderCount;
        // W border
        if (colIndex < borderSize) {
            return GetBorderIndexAsNegative(rowWithBorderCount * borderSize + 2 * borderSize * (rowIndex - borderSize));
        }
        // E border
        if (colIndex >= rowWithBorderCount - borderSize) {
            return GetBorderIndexAsNegative(rowWithBorderCount * borderSize + 2 * borderSize * (rowIndex - borderSize) + borderSize);
        }
        // in chunk
        return absIndex - rowWithBorderCount * borderSize - (2 * borderSize * (rowIndex - borderSize) + borderSize);
    }

    private int GetBorderIndexAsNegative(int positiveIndex) {
        return (positiveIndex + 1) * -1;
    }

    public Vector2 GetUv(int indexInChunk) {
        int colIndex = indexInChunk % rowWithBorderCount;
        int rowIndex = indexInChunk / rowWithBorderCount;
        return new Vector2(
            0.5f + (colIndex / (float)rowCount),
            0.5f - (rowIndex / (float)rowCount)
        );
    }

    public bool IsInChunk(int absIndex) {
        return absIndex >= rowWithBorderCount * borderSize && // On N side
            absIndex % rowWithBorderCount >= borderSize && // On W side
            absIndex % rowWithBorderCount < rowWithBorderCount - borderSize && // On E side
            absIndex < rowWithBorderCount * (rowWithBorderCount - borderSize); // On S side
    }
}
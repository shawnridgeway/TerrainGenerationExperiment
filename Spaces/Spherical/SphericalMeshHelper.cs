using System;
using UnityEngine;

public class SphericalMeshHelper : MeshHelper {
    private readonly int borderSize;
    private readonly bool isChunkInverted;

    // Cached values
    private readonly int rowsInChunk;
    private readonly int rowsInTotal;

    private readonly int rowsInTopBorder;
    private readonly int countInTopBorder;

    private readonly int countInChunk;
    private readonly int countInTotal;

    private readonly int countBeforeBottomBorder;

    public SphericalMeshHelper(Chunk chunk, int interval, int borderSize) {
        if (chunk is SphericalChunk sphericalChunk) {
            this.isChunkInverted = sphericalChunk.IsChunkInverted();
        }
        this.borderSize = borderSize;

        rowsInChunk = SphericalSpace.chunkWidth / interval + 1;
        rowsInTotal = rowsInChunk + 3 * borderSize;

        countInChunk = rowsInChunk * (rowsInChunk + 1) / 2;
        countInTotal = rowsInTotal * (rowsInTotal + 1) / 2;

        int rowsInPointyBorder = 2 * borderSize;
        int rowsInFlatBorder = borderSize;
        int countInPointyBorder = rowsInPointyBorder * (rowsInPointyBorder + 1) / 2;
        int countInFlatBorder = countInTotal - (rowsInChunk + rowsInPointyBorder) * (rowsInChunk + rowsInPointyBorder + 1) / 2;

        rowsInTopBorder = isChunkInverted ? rowsInFlatBorder : rowsInPointyBorder;
        countInTopBorder = isChunkInverted ? countInFlatBorder : countInPointyBorder;

        int rowsInBottomBorder = isChunkInverted ? rowsInPointyBorder : rowsInFlatBorder;
        int countInBottomBorder = isChunkInverted ? countInPointyBorder : countInFlatBorder;

        countBeforeBottomBorder = countInTotal - countInBottomBorder;
    }

    public (int, int, int)[] GetTriangleIndiciesForPoint(int absIndex) {
        int rowIndex = GetRowIndex(absIndex);
        int countAtPreviousRow = GetCountAtRow(rowIndex - 1);
        int colIndex = absIndex - countAtPreviousRow;
        bool isOnEdgeN = absIndex < countInTopBorder;
        bool isOnEdgeW = colIndex == 0;
        // If the point is on the W edge, then we do not include the triangles
        if (isOnEdgeN || isOnEdgeW) {
            return new (int, int, int)[0];
        }
        int countAtCurrentRow = GetCountAtRow(rowIndex);
        int currentRowWidth = countAtCurrentRow - countAtPreviousRow;
        int previousRowWidth = isChunkInverted
            ? currentRowWidth + 1
            : currentRowWidth - 1;
        // Otherwise, get the 2
        (int, int, int)[] triangles = new (int, int, int)[2];
        int absIndexW = absIndex - 1;
        int absIndexNW = isChunkInverted ? absIndex - previousRowWidth : absIndex - previousRowWidth - 1;
        int absIndexNE = isChunkInverted ? absIndex - previousRowWidth + 1 : absIndex - previousRowWidth;

        triangles[0] = (
            ConvertAbsToRelativeIndex(absIndex, rowIndex, colIndex, currentRowWidth),
            ConvertAbsToRelativeIndex(absIndexW, rowIndex, colIndex - 1, currentRowWidth),
            ConvertAbsToRelativeIndex(absIndexNW, rowIndex - 1, colIndex + (isChunkInverted ? 0 : -1), currentRowWidth + (isChunkInverted ? 1 : -1))
        );
        triangles[1] = (
            ConvertAbsToRelativeIndex(absIndex, rowIndex, colIndex, currentRowWidth),
            ConvertAbsToRelativeIndex(absIndexNW, rowIndex - 1, colIndex + (isChunkInverted ? 0 : -1), currentRowWidth + (isChunkInverted ? 1 : -1)),
            ConvertAbsToRelativeIndex(absIndexNE, rowIndex - 1, colIndex + (isChunkInverted ? 1 : 0), currentRowWidth + (isChunkInverted ? 1 : -1))
        );
        return triangles;
    }

    private int ConvertAbsToRelativeIndex(int absIndex, int rowIndex, int colIndex, int currentRowWidth) {
        // N border
        if (absIndex < countInTopBorder) {
            return GetBorderIndexAsNegative(absIndex);
        }
        // S border
        if (absIndex >= countBeforeBottomBorder) {
            return GetBorderIndexAsNegative(absIndex - countInChunk);
        }
        // W border
        if (colIndex < borderSize) {
            return GetBorderIndexAsNegative(absIndex - GetCountAtRow(rowIndex - 1 - rowsInTopBorder, true));
        }
        // E border
        if (colIndex >= currentRowWidth - borderSize) {
            return GetBorderIndexAsNegative(absIndex - GetCountAtRow(rowIndex - rowsInTopBorder, true));
        }
        // in chunk
        return absIndex - countInTopBorder - 2 * borderSize * (rowIndex - rowsInTopBorder) - borderSize;
    }

    private int GetBorderIndexAsNegative(int positiveIndex) {
        return (positiveIndex + 1) * -1;
    }

    // Get the index of this row (0 based index)
    private int GetRowIndex(int absIndex, bool isChunkOnly = false) {
        if (isChunkInverted) {
            int invertedAbsIndex = (isChunkOnly ? countInChunk : countInTotal) - absIndex - 1;
            int invertedRowIndex = Mathf.CeilToInt((Mathf.Sqrt(1 + 8 * (invertedAbsIndex + 1)) - 1f) / 2f) - 1;
            return (isChunkOnly ? rowsInChunk : rowsInTotal) - invertedRowIndex - 1;
        }
        return Mathf.CeilToInt((Mathf.Sqrt(1 + 8 * (absIndex + 1)) - 1f) / 2f) - 1;
    }

    // Get the total count of points at the end of this row (0 based index)
    private int GetCountAtRow(int rowIndex, bool isChunkOnly = false) {
        if (rowIndex < 0) {
            return 0;
        }
        if (isChunkInverted) {
            int missingRowsFromBottom = (isChunkOnly ? rowsInChunk : rowsInTotal) - rowIndex - 1;
            int count = isChunkOnly ? countInChunk : countInTotal;
            return count - (missingRowsFromBottom) * (missingRowsFromBottom + 1) / 2;
        }
        return (rowIndex + 1) * (rowIndex + 2) / 2;
    }

    public Vector2 GetUv(int indexInChunk) {
        int rowIndex = GetRowIndex(indexInChunk, true);
        int countAtPreviousRow = GetCountAtRow(rowIndex - 1, true);
        int colIndex = indexInChunk - countAtPreviousRow;
        int countAtCurrentRow = GetCountAtRow(rowIndex, true);
        int currentRowWidth = countAtCurrentRow - countAtPreviousRow;
        return new Vector2(
            0.5f + (colIndex / (float)currentRowWidth),
            0.5f - (rowIndex / (float)rowsInChunk)
        );
    }

    public bool IsInChunk(int absIndex) {
        // TODO: something may be wrong here, or that's my hunch at least...
        int rowIndex = GetRowIndex(absIndex);
        int countAtPreviousRow = GetCountAtRow(rowIndex - 1);
        int colIndex = absIndex - countAtPreviousRow;
        int countAtCurrentRow = GetCountAtRow(rowIndex);
        int currentRowWidth = countAtCurrentRow - countAtPreviousRow;
        return !(
            absIndex < countInTopBorder || // N border
            absIndex >= countBeforeBottomBorder || // S border
            colIndex < borderSize || // W border
            colIndex >= currentRowWidth - borderSize // E border
        );
    }
}

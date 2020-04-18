using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SphericalMeshBuilder {
    readonly int verticiesPerLineWithBorder;
    readonly Func<Vector3, bool> isBorderVertex;
    readonly Vector3[] verticiesWithBorder;
    int vertexWithBorderIndex;
    readonly Vector2[] uvsWithBorder;
    readonly (int a, int b, int c)[] trianglesWithBorder;
    int triangleWithBorderIndex;
    Vector3[] normalsWithBorder;

    public SphericalMeshBuilder(int verticiesPerLineWithBorder, Func<Vector3, bool> isBorderVertex) {
        this.verticiesPerLineWithBorder = verticiesPerLineWithBorder;
        this.isBorderVertex = isBorderVertex;
        verticiesWithBorder = new Vector3[verticiesPerLineWithBorder * verticiesPerLineWithBorder];
        trianglesWithBorder = new (int, int, int)[(verticiesPerLineWithBorder - 1) * (verticiesPerLineWithBorder - 1) * 2];
        uvsWithBorder = new Vector2[verticiesPerLineWithBorder * verticiesPerLineWithBorder];
        vertexWithBorderIndex = 0;
        triangleWithBorderIndex = 0;
    }


}

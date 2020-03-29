using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeshBuilder {
    readonly int verticiesPerLineWithBorder;
    readonly Func<int, bool> isBorderVertex;
    readonly Vector3[] verticiesWithBorder;
    int vertexWithBorderIndex;
    readonly Vector2[] uvsWithBorder;
    readonly (int a, int b, int c)[] trianglesWithBorder;
    int triangleWithBorderIndex;
    Vector3[] normalsWithBorder;

    public MeshBuilder(int verticiesPerLineWithBorder, Func<int, bool> isBorderVertex) {
        this.verticiesPerLineWithBorder = verticiesPerLineWithBorder;
        this.isBorderVertex = isBorderVertex;
        verticiesWithBorder = new Vector3[verticiesPerLineWithBorder * verticiesPerLineWithBorder];
        trianglesWithBorder = new (int, int, int)[(verticiesPerLineWithBorder - 1) * (verticiesPerLineWithBorder - 1) * 2];
        uvsWithBorder = new Vector2[verticiesPerLineWithBorder * verticiesPerLineWithBorder];
        vertexWithBorderIndex = 0;
        triangleWithBorderIndex = 0;
    }

    public void AddVertexAt(Vector3 vertexPosition, Vector2 uv) {
        verticiesWithBorder[vertexWithBorderIndex] = vertexPosition;
        uvsWithBorder[vertexWithBorderIndex] = uv;

        if (
            vertexWithBorderIndex > verticiesPerLineWithBorder && // Not first row
            vertexWithBorderIndex % verticiesPerLineWithBorder > 0 // Not first column
        ) {
            int a = vertexWithBorderIndex - verticiesPerLineWithBorder - 1;
            int b = vertexWithBorderIndex - verticiesPerLineWithBorder;
            int c = vertexWithBorderIndex - 1;
            int d = vertexWithBorderIndex;
            AddTriangle(a, d, c);
            AddTriangle(d, a, b);
        }

        vertexWithBorderIndex++;
    }

    public void AddTriangle(int a, int b, int c) {
        trianglesWithBorder[triangleWithBorderIndex] = (a, b, c);
        triangleWithBorderIndex++;
    }

    private Vector3[] GetNormals() {
        Vector3[] vertexNormals = new Vector3[verticiesWithBorder.Length];
        foreach ((int a, int b, int c) in trianglesWithBorder) {
            Vector3 triangleNormal = SurfaceNormalFromIndicies(a, b, c);
            vertexNormals[a] += triangleNormal;
            vertexNormals[b] += triangleNormal;
            vertexNormals[c] += triangleNormal;
        }
        for (int i = 0; i < vertexNormals.Length; i++) {
            vertexNormals[i].Normalize();
        }
        return vertexNormals;
    }

    private Vector3 SurfaceNormalFromIndicies(int indexA, int indexB, int indexC) {
        Vector3 pointA = verticiesWithBorder[indexA];
        Vector3 pointB = verticiesWithBorder[indexB];
        Vector3 pointC = verticiesWithBorder[indexC];
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    private (Vector3[] vertices, int[] triangles, Vector2[] uvs, Vector3[] normals) Trim() {
        Vector3[] vertices = new Vector3[(verticiesPerLineWithBorder - 2) * (verticiesPerLineWithBorder - 2)];
        int[] triangles = new int[(verticiesPerLineWithBorder - 3) * (verticiesPerLineWithBorder - 3) * 6];
        Vector2[] uvs = new Vector2[(verticiesPerLineWithBorder - 2) * (verticiesPerLineWithBorder - 2)];
        Vector3[] normals = new Vector3[(verticiesPerLineWithBorder - 2) * (verticiesPerLineWithBorder - 2)];
        int index = 0;
        for (int indexWithBorder = 0; indexWithBorder < verticiesWithBorder.Length; indexWithBorder++) {
            if (!isBorderVertex(indexWithBorder)) {
                vertices[index] = verticiesWithBorder[indexWithBorder];
                uvs[index] = uvsWithBorder[indexWithBorder];
                normals[index] = normalsWithBorder[indexWithBorder];
                index++;
            }
        }
        int trianglesIndex = 0;
        for (int trianglesIndexWithBorder = 0; trianglesIndexWithBorder < trianglesWithBorder.Length; trianglesIndexWithBorder++) {
            (int a, int b, int c) = trianglesWithBorder[trianglesIndexWithBorder];
            if (!isBorderVertex(a) && !isBorderVertex(b) && !isBorderVertex(c)) {
                triangles[trianglesIndex] = BorderedToUnborderedVertexIndex(trianglesWithBorder[trianglesIndexWithBorder].a);
                triangles[trianglesIndex + 1] = BorderedToUnborderedVertexIndex(trianglesWithBorder[trianglesIndexWithBorder].b);
                triangles[trianglesIndex + 2] = BorderedToUnborderedVertexIndex(trianglesWithBorder[trianglesIndexWithBorder].c);
                trianglesIndex += 3;
            }
        }
        return (vertices, triangles, uvs, normals);
    }

    private int BorderedToUnborderedVertexIndex(int vertexIndexWithBorder) {
        int rowNumberWithBorder = vertexIndexWithBorder / verticiesPerLineWithBorder;
        return vertexIndexWithBorder - verticiesPerLineWithBorder - 1 - ((rowNumberWithBorder - 1) * 2);
    }

    public MeshData Build() {
        normalsWithBorder = GetNormals();
        (Vector3[] vertices, int[] triangles, Vector2[] uvs, Vector3[] normals) = Trim();
        MeshData meshData = new MeshData {
            vertices = vertices,
            triangles = triangles,
            uvs = uvs,
            normals = normals
        };
        return meshData;
    }
}

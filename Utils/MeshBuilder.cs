using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeshBuilder {
    readonly int verticiesPerLineWithBorder;
    readonly Func<Vector3, bool> isBorderVertex;
    readonly Vector3[] verticiesWithBorder;
    int vertexWithBorderIndex;
    readonly Vector2[] uvsWithBorder;
    readonly MeshTriangle[] trianglesWithBorder; // TODO: make this a Set
    int triangleWithBorderIndex;
    Vector3[] normalsWithBorder;

    public MeshBuilder(int verticiesPerLineWithBorder, Func<Vector3, bool> isBorderVertex) {
        this.verticiesPerLineWithBorder = verticiesPerLineWithBorder;
        this.isBorderVertex = isBorderVertex;
        verticiesWithBorder = new Vector3[verticiesPerLineWithBorder * verticiesPerLineWithBorder];
        trianglesWithBorder = new MeshTriangle[(verticiesPerLineWithBorder - 1) * (verticiesPerLineWithBorder - 1) * 2];
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
        trianglesWithBorder[triangleWithBorderIndex] = new MeshTriangle(a, b, c);
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
            if (!isBorderVertex(verticiesWithBorder[indexWithBorder])) {
                vertices[index] = verticiesWithBorder[indexWithBorder];
                uvs[index] = uvsWithBorder[indexWithBorder];
                normals[index] = normalsWithBorder[indexWithBorder];
                index++;
            }
        }
        int trianglesIndex = 0;
        for (int trianglesIndexWithBorder = 0; trianglesIndexWithBorder < trianglesWithBorder.Length; trianglesIndexWithBorder++) {
            (int a, int b, int c) = trianglesWithBorder[trianglesIndexWithBorder];
            if (!isBorderVertex(verticiesWithBorder[a]) && !isBorderVertex(verticiesWithBorder[b]) && !isBorderVertex(verticiesWithBorder[c])) {
                triangles[trianglesIndex] = BorderedToUnborderedVertexIndex(a);
                triangles[trianglesIndex + 1] = BorderedToUnborderedVertexIndex(b);
                triangles[trianglesIndex + 2] = BorderedToUnborderedVertexIndex(c);
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

    /////// Triangle ///////
    private MeshTriangle[] PlanarGetTrianglesAtVertex(PlanarPoint vertex) {
        List<MeshTriangle> resultTriangles = new List<MeshTriangle>();

        Dictionary<Point, int> pointIndexes = new Dictionary<Point, int>();
        int index, indexE, indexN, indexNW, indexW, indexS, indexSE;
        pointIndexes.TryGetValue(vertex, out index);
        pointIndexes.TryGetValue(vertex.GetNeighbor(PlanarSpace.Direction.E), out indexE);
        pointIndexes.TryGetValue(vertex.GetNeighbor(PlanarSpace.Direction.N), out indexN);
        pointIndexes.TryGetValue(vertex.GetNeighbor(PlanarSpace.Direction.N).GetNeighbor(PlanarSpace.Direction.W), out indexNW);
        pointIndexes.TryGetValue(vertex.GetNeighbor(PlanarSpace.Direction.W), out indexW);
        pointIndexes.TryGetValue(vertex.GetNeighbor(PlanarSpace.Direction.S), out indexS);
        pointIndexes.TryGetValue(vertex.GetNeighbor(PlanarSpace.Direction.S).GetNeighbor(PlanarSpace.Direction.E), out indexSE);

        // TODO: below
        if (
            vertexWithBorderIndex > verticiesPerLineWithBorder && // Not first row
            vertexWithBorderIndex % verticiesPerLineWithBorder > 0 // Not first column
        ) {
            int a = vertexWithBorderIndex - verticiesPerLineWithBorder - 1;
            int b = vertexWithBorderIndex - verticiesPerLineWithBorder;
            int c = vertexWithBorderIndex - 1;
            int d = vertexWithBorderIndex;
            resultTriangles.Add(new MeshTriangle(a, d, c));
            resultTriangles.Add(new MeshTriangle(d, a, b));
        }
        return resultTriangles.ToArray();
    }

    private MeshTriangle[] SphericalGetTrianglesAtVertex(SphericalPoint vertex) {
        List<MeshTriangle> resultTriangles = new List<MeshTriangle>();
        // TODO
        return resultTriangles.ToArray();
    }
}

struct MeshTriangle {
    public readonly int indexA;
    public readonly int indexB;
    public readonly int indexC;

    public MeshTriangle(int indexA, int indexB, int indexC) {
        this.indexA = indexA;
        this.indexB = indexB;
        this.indexC = indexC;
    }

    public void Deconstruct(out int indexA, out int indexB, out int indexC) {
        indexA = this.indexA;
        indexB = this.indexB;
        indexC = this.indexC;
    }
}
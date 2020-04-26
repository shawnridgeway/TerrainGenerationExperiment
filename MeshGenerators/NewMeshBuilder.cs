using System.Collections.Generic;
using UnityEngine;
using System;

public class NewMeshBuilder {
    private readonly int unityMaxVertexCount = 65535;

    // The space that is being rendered
    private readonly TriangleGenerator triangleGenerator;

    // List of verticies included in the chunk in order (chunk only)
    private readonly Vector3[] chunkVerticies;

    // List of verticies included in the border in order (border only)
    private readonly Vector3[] borderVerticies;

    // List of uvs, indicies coorespond to verticies in chunkPoints (chunk only)
    private readonly Vector2[] chunkUvs;

    // List of triangles, in triplets of indicies of the verticies (chunk only)
    private readonly List<int> chunkTriangles;

    // List of normals (chunk only)
    private readonly Vector3[] chunkNormals;

    private int chunkPointsNextIndex = 0;
    private int borderPointsNextIndex = 0;

    public NewMeshBuilder(Space space, int chunkPointCount, int boderPointCount, int interval, int borderSize) {
        if (chunkPointCount > unityMaxVertexCount) {
            throw new Exception(
                string.Format(
                    "The supplied count of points ({0}) is larger than the maximum ({1}) that Unity allows in a single Mesh.",
                    chunkPointCount,
                    unityMaxVertexCount
                )
            );
        }
        triangleGenerator = space.GetTriangleGenerator(interval, borderSize);
        chunkVerticies = new Vector3[chunkPointCount];
        borderVerticies = new Vector3[boderPointCount];
        chunkUvs = new Vector2[chunkPointCount];
        chunkTriangles = new List<int>((chunkPointCount + boderPointCount) * 6); // 2 triangles per vertex * 3 points per triangle
        chunkNormals = new Vector3[chunkPointCount];
    }

    public void AddVertex(Vector3 vertex) {
        if (triangleGenerator.IsInChunk(chunkPointsNextIndex + borderPointsNextIndex)) {
            AddChunkPoint(vertex);
        } else {
            AddBorderPoint(vertex);
        }
    }

    public void AddChunkPoint(Vector3 vertex) {
        chunkVerticies[chunkPointsNextIndex] = vertex;
        chunkUvs[chunkPointsNextIndex] = triangleGenerator.GetUv(chunkPointsNextIndex);
        AddTrianglesForPoint();
        chunkPointsNextIndex++;
    }

    public void AddBorderPoint(Vector3 vertex) {
        borderVerticies[borderPointsNextIndex] = vertex;
        AddTrianglesForPoint();
        borderPointsNextIndex++;
    }

    private void AddTrianglesForPoint() {
        (int, int, int)[] meshTriangles = triangleGenerator.GetTriangleIndiciesForPoint(chunkPointsNextIndex + borderPointsNextIndex);
        foreach ((int a, int b, int c) meshTriangle in meshTriangles) {
            if (
                IsInChunk(meshTriangle.a) &&
                IsInChunk(meshTriangle.b) &&
                IsInChunk(meshTriangle.c)
            ) {
                chunkTriangles.Add(meshTriangle.a);
                chunkTriangles.Add(meshTriangle.b);
                chunkTriangles.Add(meshTriangle.c);
            }
            AddNormals(meshTriangle.a, meshTriangle.b, meshTriangle.c);
        }
    }

    private void AddNormals(int a, int b, int c) {
        Vector3 triangleNormal = GetSurfaceNormalFromPoints(
            GetVertex(a),
            GetVertex(b),
            GetVertex(c)
        );
        if (IsInChunk(a)) {
            chunkNormals[a] += triangleNormal;
        }
        if (IsInChunk(b)) {
            chunkNormals[b] += triangleNormal;
        }
        if (IsInChunk(c)) {
            chunkNormals[c] += triangleNormal;
        }
    }

    private Vector3 GetSurfaceNormalFromPoints(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC) {
        Vector3 sideAB = vertexB - vertexA;
        Vector3 sideAC = vertexC - vertexA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    private Vector3 GetVertex(int index) {
        if (IsInChunk(index)) {
            return chunkVerticies[index];
        }
        return borderVerticies[(index * -1) - 1];
    }

    private bool IsInChunk(int index) {
        return index >= 0;
    }

    private void BuildNormals() {
        foreach (Vector3 normal in chunkNormals) {
            normal.Normalize();
        }
    }

    public MeshData Build() {
        BuildNormals();
        return new MeshData {
            vertices = chunkVerticies,
            normals = chunkNormals,
            uvs = chunkUvs,
            triangles = chunkTriangles.ToArray(),
        };
    }
}

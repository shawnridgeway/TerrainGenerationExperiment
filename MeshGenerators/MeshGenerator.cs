using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public abstract class MeshGenerator {
    private readonly Dictionary<(Chunk, MeshLod), MeshData> meshDataCache =
        new Dictionary<(Chunk, MeshLod), MeshData>();

    private readonly Dictionary<(Chunk, MeshLod), Mesh> meshCache =
        new Dictionary<(Chunk, MeshLod), Mesh>();

    private readonly HashSet<(Chunk, MeshLod)> meshesBeingProcessed =
        new HashSet<(Chunk, MeshLod)>();

    /*
     * Get Mesh if it is created or its MeshData is ready, otherwise begin
     * async creation of MeshData and return null
     */
    public Mesh RequestMesh(Chunk chunk, MeshLod lod) {
        // First try and get mesh
        Mesh mesh;
        bool meshExists = meshCache.TryGetValue((chunk, lod), out mesh);
        if (meshExists) {
            return mesh;
        }
        // Second, see if meshData is ready to build
        MeshData meshData;
        bool meshDataExists = meshDataCache.TryGetValue((chunk, lod), out meshData);
        if (meshDataExists) {
            mesh = CreateMesh(meshData);
            meshCache.Add((chunk, lod), mesh);
            meshDataCache.Remove((chunk, lod));
            return mesh;
        }
        // Else, asynchronously prepare the meshData
        PrepareMeshData(chunk, lod);
        return null;
    }

    /*
     * Asynchronously create MeshData
     */
    private async void PrepareMeshData(Chunk chunk, MeshLod lod) {
        // Bail if mesh is currently being processed
        if (meshesBeingProcessed.Contains((chunk, lod))) {
            return;
        }
        meshesBeingProcessed.Add((chunk, lod));
        MeshData meshData = await Task.Run(() => GetMeshData(chunk, lod));
        meshDataCache.Add((chunk, lod), meshData);
        meshesBeingProcessed.Remove((chunk, lod));
    }

    /*
     * Build Mesh object from MeshData
     */
    private Mesh CreateMesh(MeshData meshData) {
        return new Mesh {
            vertices = meshData.vertices,
            triangles = meshData.triangles,
            uv = meshData.uvs,
            normals = meshData.normals
        };
    }

    /*
     * Get the MeshData for the given chunk and lod
     */
    protected abstract MeshData GetMeshData(Chunk chunk, MeshLod lod);
}

public struct MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Vector3[] normals;
}
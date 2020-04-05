using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public abstract class MeshGenerator {

    private readonly Dictionary<(Chunk, MeshLod), Mesh> meshCache =
        new Dictionary<(Chunk, MeshLod), Mesh>();

    // Mesh building state
    private readonly Dictionary<(Chunk, MeshLod), MeshData> meshDataCache =
        new Dictionary<(Chunk, MeshLod), MeshData>();

    private readonly HashSet<(Chunk, MeshLod)> meshesBeingProcessed =
        new HashSet<(Chunk, MeshLod)>();

    // Collision baking state
    private readonly HashSet<Mesh> meshesWithCollision =
        new HashSet<Mesh>();

    private readonly HashSet<Mesh> collisionsBeingProcessed =
        new HashSet<Mesh>();

    /*
     * Get Mesh if it is created or its MeshData is ready, otherwise begin
     * async creation of MeshData and return null
     */
    public Mesh RequestMesh(Chunk chunk, MeshLod lod, bool includeCollider) {
        // Part 1: get mesh if it exists
        Mesh mesh;
        bool meshExists = meshCache.TryGetValue((chunk, lod), out mesh);
        if (!meshExists) {
            // Check if meshData is ready to build
            MeshData meshData;
            bool meshDataExists = meshDataCache.TryGetValue((chunk, lod), out meshData);
            if (!meshDataExists) {
                // If not ready, asynchronously prepare the meshData
                PrepareMeshData(chunk, lod);
                return null;
            }
            mesh = CreateMesh(meshData);
            meshCache.Add((chunk, lod), mesh);
            meshDataCache.Remove((chunk, lod));
        }
        // Part 2: ensure collision is complete if necessary
        bool meshHasCollision = meshesWithCollision.Contains(mesh);
        if (includeCollider && !meshHasCollision) {
            // If not baked, asynchronously prepare the collision
            PrepareCollision(mesh);
            return null;
        }
        // If mesh and collision are ready, return the requested mesh
        return mesh;
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
     * Bake collision into Mesh asynchronously
     */
    private async void PrepareCollision(Mesh mesh) {
        // Bail if mesh is currently being processed
        if (collisionsBeingProcessed.Contains(mesh)) {
            return;
        }
        collisionsBeingProcessed.Add(mesh);
        int instanceId = mesh.GetInstanceID();
        await Task.Run(() => Physics.BakeMesh(instanceId, false));
        meshesWithCollision.Add(mesh);
        collisionsBeingProcessed.Remove(mesh);
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
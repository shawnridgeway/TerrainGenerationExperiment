using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRenderer {
    private readonly Viewer viewer;
    private readonly Transform parent;
    private readonly MeshGenerator meshGenerator;
    private readonly Material material;

    private readonly HashSet<ViewChunk> vivibleViewChunks = new HashSet<ViewChunk>();
    private readonly Dictionary<ViewChunk, GameObject> viewDict = new Dictionary<ViewChunk, GameObject>();

    public TerrainRenderer(Transform parent, Viewer viewer, MeshGenerator meshGenerator, Material material) {
        this.parent = parent;
        this.viewer = viewer;
        this.meshGenerator = meshGenerator;
        this.material = material;
    }

    public void Render() {
        ViewChunk[] newView = viewer.View();
        UpdateVisible(newView);
    }

    public void UpdateVisible(ViewChunk[] newView) {
        // Remove no longer visible chunks
        HashSet<ViewChunk> nowVisibleViewChunkSet = new HashSet<ViewChunk>(newView);
        HashSet<ViewChunk> noLongerVisibleViewChunkSet = new HashSet<ViewChunk>();
        foreach (ViewChunk visibleViewChunk in vivibleViewChunks) {
            if (!nowVisibleViewChunkSet.Contains(visibleViewChunk)) {
                noLongerVisibleViewChunkSet.Add(visibleViewChunk);
            }
        }
        foreach (ViewChunk noLongerVisibleViewChunk in noLongerVisibleViewChunkSet) {
            SetViewChunkVisible(noLongerVisibleViewChunk, false);
        }

        // Add newly visible chunks
        foreach (ViewChunk nowVisibleViewChunk in nowVisibleViewChunkSet) {
            if (!vivibleViewChunks.Contains(nowVisibleViewChunk)) {
                SetViewChunkVisible(nowVisibleViewChunk, true);
            }
        }
    }

    void SetViewChunkVisible(ViewChunk viewChunk, bool nowVisible) {
        GameObject meshObject;
        bool meshExists = viewDict.TryGetValue(viewChunk, out meshObject);
        if (!meshExists) {
            meshObject = CreateGameObject(viewChunk.chunk, viewChunk.lod, false, 4);
            viewDict.Add(viewChunk, meshObject);
            meshObject.transform.parent = parent;
        }
        if (nowVisible) {
            meshObject.SetActive(true);
            vivibleViewChunks.Add(viewChunk);
        } else {
            meshObject.SetActive(false);
            vivibleViewChunks.Remove(viewChunk);
        }
    }

    public GameObject CreateGameObject(Chunk chunk, int lod, bool hasCollider, int colliderLod) {
        GameObject meshObject = new GameObject("Terrain Chunk");
        meshObject.transform.position = chunk.GetCenterLocation();

        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        meshFilter.mesh = meshGenerator.Process(chunk, lod);

        // TODO: optimize generating, caching, reusing collider meshes
        if (hasCollider) {
            MeshCollider meshCollider = meshObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = meshGenerator.Process(chunk, colliderLod);
        }

        return meshObject;
    }
}

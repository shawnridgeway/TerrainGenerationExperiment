using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRenderer {
    private readonly Viewer viewer;
    private readonly Transform parent;
    private readonly MeshGenerator meshGenerator;

    private readonly HashSet<Chunk> visibleChunks = new HashSet<Chunk>();
    private readonly Dictionary<Chunk, GameObject> chunkDict = new Dictionary<Chunk, GameObject>();

    public TerrainRenderer(Viewer viewer, Transform parent, MeshGenerator meshGenerator) {
        this.viewer = viewer;
        this.parent = parent;
        this.meshGenerator = meshGenerator;
    }

    public void Render() {
        Chunk[] nowVisibleChunks = viewer.View();
        UpdateVisible(nowVisibleChunks);
    }

    public void UpdateVisible(Chunk[] nowVisibleChunks) {
        // Remove no longer visible chunks
        HashSet<Chunk> nowVisibleChunkSet = new HashSet<Chunk>(nowVisibleChunks);
        HashSet<Chunk> noLongerVisibleChunkSet = new HashSet<Chunk>();
        foreach (Chunk visibleChunk in visibleChunks) {
            if (!nowVisibleChunkSet.Contains(visibleChunk)) {
                noLongerVisibleChunkSet.Add(visibleChunk);
            }
        }
        foreach (Chunk noLongerVisibleChunk in noLongerVisibleChunkSet) {
            SetChunkVisible(noLongerVisibleChunk, false);
        }

        // Add newly visible chunks
        foreach (Chunk nowVisibleChunk in nowVisibleChunks) {
            if (!visibleChunks.Contains(nowVisibleChunk)) {
                SetChunkVisible(nowVisibleChunk, true);
            }
        }
    }

    void SetChunkVisible(Chunk chunk, bool nowVisible) {
        GameObject meshObject;
        bool meshExists = chunkDict.TryGetValue(chunk, out meshObject);
        if (!meshExists) {
            meshObject = meshGenerator.Process(chunk, 0);
            chunkDict.Add(chunk, meshObject);
            meshObject.transform.parent = parent;
        }
        if (nowVisible) {
            meshObject.SetActive(true);
            visibleChunks.Add(chunk);
        } else {
            meshObject.SetActive(false);
            visibleChunks.Remove(chunk);
        }
    }
}

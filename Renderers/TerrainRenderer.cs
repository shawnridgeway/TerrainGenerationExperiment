using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRenderer {
    private readonly Viewer viewer;
    private readonly Transform parent;
    private readonly MeshGenerator meshGenerator;
    private readonly Material material;

    private readonly HashSet<ViewChunk> visibleViewChunks = new HashSet<ViewChunk>();
    private readonly Dictionary<ViewChunk, GameObject> objectCache = new Dictionary<ViewChunk, GameObject>();

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
        // Determine which view chunks are now or no longer visible
        HashSet<ViewChunk> nowVisibleViewChunkSet = new HashSet<ViewChunk>(newView);
        HashSet<ViewChunk> noLongerVisibleViewChunkSet = new HashSet<ViewChunk>();
        foreach (ViewChunk visibleViewChunk in visibleViewChunks) {
            if (!nowVisibleViewChunkSet.Contains(visibleViewChunk)) {
                noLongerVisibleViewChunkSet.Add(visibleViewChunk);
            }
        }
        // Remove no longer visible chunks
        foreach (ViewChunk noLongerVisibleViewChunk in noLongerVisibleViewChunkSet) {
            bool hideSuccessful = HideChunk(noLongerVisibleViewChunk);
            if (hideSuccessful) {
                visibleViewChunks.Remove(noLongerVisibleViewChunk);
            }
        }
        // Add newly visible chunks
        foreach (ViewChunk nowVisibleViewChunk in nowVisibleViewChunkSet) {
            if (!visibleViewChunks.Contains(nowVisibleViewChunk)) {
                bool showSuccessful = ShowChunk(nowVisibleViewChunk);
                if (showSuccessful) {
                    visibleViewChunks.Add(nowVisibleViewChunk);
                }
            }
        }
    }

    bool HideChunk(ViewChunk viewChunk) {
        bool hideSuccessful = false;
        GameObject gameObject;
        bool gameObjectExists = objectCache.TryGetValue(viewChunk, out gameObject);
        if (gameObjectExists) {
            gameObject.SetActive(false);
            hideSuccessful = true;
        }
        return hideSuccessful;
    }

    bool ShowChunk(ViewChunk viewChunk) {
        bool showSuccessful = false;
        GameObject gameObject;
        bool gameObjectExists = objectCache.TryGetValue(viewChunk, out gameObject);
        if (gameObjectExists) {
            gameObject.SetActive(true);
            showSuccessful = true;
        } else {
            GameObject maybeGameObject = RequestGameObject(viewChunk);
            if (maybeGameObject != null) {
                objectCache.Add(viewChunk, maybeGameObject);
                maybeGameObject.SetActive(true);
                showSuccessful = true;
            }
        }
        return showSuccessful;
    }

    GameObject RequestGameObject(ViewChunk viewChunk) {
        // Request necessary meshes
        Mesh maybeMesh = meshGenerator.RequestMesh(viewChunk.chunk, viewChunk.lod);
        Mesh maybeColliderMesh = null;
        if (viewChunk.colliderLod is MeshLod colliderLod) {
            maybeColliderMesh = meshGenerator.RequestMesh(viewChunk.chunk, colliderLod);
        }
        // If both are satisfied, return the game object
        if (maybeMesh != null && (maybeColliderMesh != null || !viewChunk.HasCollider())) {
            return CreateGameObject(viewChunk.chunk.GetCenterLocation(), maybeMesh, maybeColliderMesh);
        }
        // Else return null, try again later
        return null;
    }

    public GameObject CreateGameObject(Vector3 position, Mesh mesh, Mesh colliderMesh) {
        GameObject gameObject = new GameObject("Terrain Chunk");
        gameObject.SetActive(false);
        gameObject.transform.position = position;
        gameObject.transform.parent = parent;
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        if (colliderMesh != null) {
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = colliderMesh;
        }
        return gameObject;
    }
}

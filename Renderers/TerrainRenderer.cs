using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TerrainRenderer {
    private readonly Viewer viewer;
    private readonly MeshGenerator meshGenerator;
    private readonly Material material;

    // State of each chunk in view chunk form
    private readonly Dictionary<Chunk, ViewChunk> viewState = new Dictionary<Chunk, ViewChunk>();
    // Cache of all created game objects
    private readonly TerrainObjectCache objectCache;

    private bool initialRenderComplete = false;
    public event Action OnInitialRenderComplete;

    public TerrainRenderer(Transform parent, Viewer viewer, MeshGenerator meshGenerator, Material material) {
        this.viewer = viewer;
        this.meshGenerator = meshGenerator;
        this.material = material;
        objectCache = new TerrainObjectCache(parent);
    }

    /* ===== State Accessors ===== */

    private MeshLod? GetCurrentVisibilityMeshLod(Chunk chunk) {
        ViewChunk viewChunk;
        bool viewChunkExists = viewState.TryGetValue(chunk, out viewChunk);
        if (!viewChunkExists) {
            return null;
        }
        return viewChunk.visibleLod;
    }

    private MeshLod? GetCurrentTangibilityMeshLod(Chunk chunk) {
        ViewChunk viewChunk;
        bool viewChunkExists = viewState.TryGetValue(chunk, out viewChunk);
        if (!viewChunkExists) {
            return null;
        }
        return viewChunk.tangibleLod;
    }

    /* ===== Lifecycle Operations ===== */

    public void Render() {
        ViewChunk[] newView = viewer.View();
        bool updateCompletelyApplied = ApplyNewState(newView);
        TriggerEvents(
            updateCompletelyApplied: updateCompletelyApplied
        );
    }

    private void TriggerEvents(bool updateCompletelyApplied) {
        if (!initialRenderComplete && updateCompletelyApplied) {
            initialRenderComplete = true;
            OnInitialRenderComplete();
        }
    }

    /* ===== State Operations ===== */

    // Commit a new state to the game state and view state
    // NB. sice this relies on meshes being previously built, the final
    // state may not match the proposed new state
    private bool ApplyNewState(ViewChunk[] newView) {
        bool updateCompletelyApplied = true;
        // Remove no longer active chunks
        HashSet<Chunk> newStateChunks = new HashSet<Chunk>(newView.Select(viewChunk => viewChunk.chunk).ToArray());
        HashSet<Chunk> previousStateChunks = new HashSet<Chunk>(viewState.Keys.ToArray());
        foreach (Chunk previousSateChunk in previousStateChunks) {
            if (!newStateChunks.Contains(previousSateChunk)) {
                updateCompletelyApplied &= SetChunkState(new ViewChunk(previousSateChunk, null, null));
            }
        }
        // Add/Update active chunks
        foreach (ViewChunk newViewChunk in newView) {
            updateCompletelyApplied &= SetChunkState(newViewChunk);
        }
        return updateCompletelyApplied;
    }

    /* ===== Chunk Operations ===== */

    // Update visibility & tangibility. Update viewState
    private bool SetChunkState(ViewChunk proposedViewChunk) {
        Chunk chunk = proposedViewChunk.chunk;
        // Here we specifically update the tangibility first to keep gameplay smooth
        bool tangibilitySetSuccess = SetChunkTangibility(chunk, proposedViewChunk.visibleLod);
        bool visibilitySetSuccess = SetChunkVisibility(chunk, proposedViewChunk.visibleLod);
        ViewChunk finalizedViewChunk = new ViewChunk(
            chunk,
            visibilitySetSuccess ? proposedViewChunk.visibleLod : GetCurrentVisibilityMeshLod(chunk),
            tangibilitySetSuccess ? proposedViewChunk.tangibleLod : GetCurrentTangibilityMeshLod(chunk)
        );
        if (viewState.ContainsKey(chunk)) {
            viewState.Remove(chunk);
        }
        if (finalizedViewChunk.tangibleLod != null || finalizedViewChunk.visibleLod != null) {
            viewState.Add(chunk, finalizedViewChunk); // Only add if visible or tangible
        }
        SetGameObjectActivity(chunk); // Update game object activity (optimization)
        return tangibilitySetSuccess && visibilitySetSuccess;
    }

    private bool SetChunkVisibility(Chunk chunk, MeshLod? maybeNewLod) {
        if (maybeNewLod is MeshLod newLod) {
            return AddChunkVisibility(chunk, newLod);
        } else {
            return RemoveChunkVisibility(chunk);
        }
    }

    private bool SetChunkTangibility(Chunk chunk, MeshLod? maybeNewLod) {
        if (maybeNewLod is MeshLod newLod) {
            return AddChunkTangibility(chunk, newLod);
        } else {
            return RemoveChunkTangibility(chunk);
        }
    }

    private bool AddChunkVisibility(Chunk chunk, MeshLod newLod) {
        MeshLod? maybePreviousLod = GetCurrentVisibilityMeshLod(chunk);
        if (maybePreviousLod is MeshLod previousLod) {
            if (newLod == previousLod) {
                return true; // Return true if mesh is unchanged
            }
            return AddVisibleMesh(chunk, newLod);
        } else {
            return AddVisibleMesh(chunk, newLod);
        }
    }

    private bool AddChunkTangibility(Chunk chunk, MeshLod newLod) {
        MeshLod? maybePreviousLod = GetCurrentTangibilityMeshLod(chunk);
        if (maybePreviousLod is MeshLod previousLod) {
            if (newLod == previousLod) {
                return true; // Return true if mesh is unchanged
            }
            return AddTangibleMesh(chunk, newLod);
        } else {
            return AddTangibleMesh(chunk, newLod);
        }
    }

    private bool RemoveChunkVisibility(Chunk chunk) {
        MeshLod? maybePreviousLod = GetCurrentVisibilityMeshLod(chunk);
        if (maybePreviousLod == null) {
            return true; // Return true if already was removed
        }
        return RemoveVisibleMesh(chunk);
    }

    private bool RemoveChunkTangibility(Chunk chunk) {
        MeshLod? maybePreviousLod = GetCurrentTangibilityMeshLod(chunk);
        if (maybePreviousLod == null) {
            return true; // Return true if already was removed
        }
        return RemoveTangibleMesh(chunk);
    }

    /* ===== GameObject Operations ===== */

    // Set game object active or not
    private void SetGameObjectActivity(Chunk chunk) {
        bool shouldBeActive = false;
        ViewChunk viewChunk;
        bool viewChunkExists = viewState.TryGetValue(chunk, out viewChunk);
        if (viewChunkExists && (viewChunk.IsVisible() || viewChunk.IsTangible())) {
            shouldBeActive = true;
        }
        GameObject gameObject = objectCache.GetTerrainObject(chunk);
        if (gameObject.activeSelf != shouldBeActive) {
            gameObject.SetActive(shouldBeActive);
        }
    }

    /* ===== Mesh Operations ===== */

    // Attempt to add a mesh to the game object mesh filter.
    // Returns true if addition was successful,
    // otherwise returns false (due to mesh not being ready yet)
    private bool AddVisibleMesh(Chunk chunk, MeshLod meshLod) {
        Mesh maybeMesh = meshGenerator.RequestMesh(chunk, meshLod);
        if (maybeMesh == null) {
            return false;
        }
        GameObject gameObject = objectCache.GetTerrainObject(chunk);
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null) {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        meshRenderer.material = material;
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null) {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = maybeMesh;
        return true;
    }

    // Attempt to add a mesh to the game object mesh collider.
    // Returns true if addition was successful,
    // otherwise returns false (due to mesh not being ready yet)
    private bool AddTangibleMesh(Chunk chunk, MeshLod meshLod) {
        Mesh maybeMesh = meshGenerator.RequestMesh(chunk, meshLod);
        if (maybeMesh == null) {
            return false;
        }
        GameObject gameObject = objectCache.GetTerrainObject(chunk);
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        if (meshCollider == null) {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = maybeMesh;
        meshCollider.enabled = true;
        return true;
    }

    // Remove a mesh from a game object's mesh filter
    private bool RemoveVisibleMesh(Chunk chunk) {
        GameObject gameObject = objectCache.GetTerrainObject(chunk);
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null) {
            meshFilter.mesh = null;
        }
        return true;
    }

    // Remove a mesh from a game object's mesh collider
    private bool RemoveTangibleMesh(Chunk chunk) {
        GameObject gameObject = objectCache.GetTerrainObject(chunk);
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        if (meshCollider != null) {
            meshCollider.enabled = false;
            meshCollider.sharedMesh = null;
        }
        return true;
    }
}

class TerrainObjectCache {
    private readonly Transform parent;

    // Cache of all created game objects
    private readonly Dictionary<Chunk, GameObject> objectCache = new Dictionary<Chunk, GameObject>();

    public TerrainObjectCache(Transform parent) {
        this.parent = parent;
    }

    public GameObject GetTerrainObject(Chunk chunk) {
        GameObject gameObject;
        bool gameObjectExists = objectCache.TryGetValue(chunk, out gameObject);
        if (!gameObjectExists) {
            gameObject = CreateGameObject(chunk);
            objectCache.Add(chunk, gameObject);
        }
        return gameObject;
    }

    // Create a new empty game object
    private GameObject CreateGameObject(Chunk chunk) {
        GameObject gameObject = new GameObject("Terrain Chunk");
        gameObject.SetActive(false);
        gameObject.transform.position = chunk.GetCenterLocation();
        gameObject.transform.parent = parent;
        return gameObject;
    }
}
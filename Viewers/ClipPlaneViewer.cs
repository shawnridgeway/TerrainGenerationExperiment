using System.Linq;
using UnityEngine;

public class ClipPlaneViewer : Viewer {
    private readonly ChunkedSpace space;
    private readonly Transform observer;
    private readonly float clipDistace;
    private readonly MeshLod lod;
    private readonly MeshLod? colliderLod;
    private readonly float updateDistaceSqr; // Distance traveled from previousObserverPosition before next update.
    private Vector3 previousObserverPosition;
    private ViewChunk[] view = null;

    public ClipPlaneViewer(
        ChunkedSpace space,
        Transform observer,
        float clipDistace,
        MeshLod? lod = null,
        MeshLod? colliderLod = null
    ) {
        this.space = space;
        this.observer = observer;
        this.clipDistace = clipDistace;
        this.lod = lod ?? new MeshLod(0);
        this.colliderLod = colliderLod;
        this.updateDistaceSqr = space.GetChunkSize() * space.GetChunkSize() / (10f * 10f);
    }

    public ViewChunk[] View() {
        Vector3 observerPosition = space.GetClosestPointTo(observer.position).GetLocation();
        if ((previousObserverPosition - observerPosition).sqrMagnitude > updateDistaceSqr || view == null) {
            previousObserverPosition = observerPosition;
            UpdateVisible(observerPosition);
        }
        return view;
    }

    private void UpdateVisible(Vector3 observerPosition) {
        view = space
            .GetChunksWithin(observerPosition, clipDistace)
            .Select(chunk => new ViewChunk(chunk, lod, colliderLod))
            .ToArray();
    }
}

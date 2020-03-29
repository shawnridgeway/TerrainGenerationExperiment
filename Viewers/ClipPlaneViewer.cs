using System.Linq;
using UnityEngine;

public class ClipPlaneViewer : Viewer {
    private readonly ChunkedSpace space;
    private readonly Transform observer;
    private readonly float clipDistace;
    private readonly MeshLod lod;
    private readonly MeshLod? colliderLod;
    private readonly float updateDistace; // Distance traveled from previousObserverPosition before next update.
    private Point previousObserverPoint;
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
        this.updateDistace = space.GetChunkSize() / 10f;
    }

    public ViewChunk[] View() {
        Point observerPoint = space.GetPointInSpace(observer.position);
        if (view == null || space.IsPointInRange(observerPoint, previousObserverPoint, updateDistace)) {
            previousObserverPoint = observerPoint;
            view = GetVisible(observerPoint);
        }
        return view;
    }

    private ViewChunk[] GetVisible(Point observerPoint) {
        return space
            .GetChunksWithin(observerPoint, clipDistace)
            .Select(chunk => new ViewChunk(chunk, lod, colliderLod))
            .ToArray();
    }
}

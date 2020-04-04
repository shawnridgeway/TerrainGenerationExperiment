using System.Linq;
using UnityEngine;

public class ClipPlaneViewer : Viewer {
    private readonly ChunkedSpace space;
    private readonly Transform observer;
    private readonly float clipDistace;
    private readonly MeshLod visibleLod;
    private readonly MeshLod? tangibleLod;
    private readonly float updateDistace; // Distance traveled from previousObserverPosition before next update.
    private Point previousObserverPoint;
    private ViewChunk[] view = null;

    public ClipPlaneViewer(
        ChunkedSpace space,
        Transform observer,
        float clipDistace,
        MeshLod? visibleLod = null,
        MeshLod? tangibleLod = null
    ) {
        this.space = space;
        this.observer = observer;
        this.clipDistace = clipDistace;
        this.visibleLod = visibleLod ?? new MeshLod(0);
        this.tangibleLod = tangibleLod;
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
            .Select(chunk => new ViewChunk(chunk, visibleLod, tangibleLod))
            .ToArray();
    }
}

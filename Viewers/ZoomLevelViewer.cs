using System.Linq;
using UnityEngine;

public class ZoomLevelViewer : Viewer {
    private readonly ChunkedSpace space;
    private readonly Transform observer;
    private readonly float ratio;
    private readonly float updateDistaceSqr; // Distance traveled from previousObserverPosition before next update.
    private Vector3 previousObserverPosition;
    private ViewChunk[] view = null;

    public ZoomLevelViewer(ChunkedSpace space, Transform observer, float ratio = 1) {
        this.space = space;
        this.observer = observer;
        this.ratio = ratio;
        this.updateDistaceSqr = space.GetChunkSize() * space.GetChunkSize() / (10f * 10f);
    }

    public ViewChunk[] View() {
        Vector3 observerPosition = observer.position;
        if ((previousObserverPosition - observerPosition).sqrMagnitude > updateDistaceSqr || view == null) {
            previousObserverPosition = observerPosition;
            UpdateVisible(observerPosition);
        }
        return view;
    }

    private void UpdateVisible(Vector3 observerPosition) {
        Vector3 closestPointToObserver = space.GetClosestPointTo(observerPosition).GetLocation();
        float observerToClosestPointDistance = Vector3.Distance(
            observerPosition,
            closestPointToObserver
        );
        float clipRadius = ratio * observerToClosestPointDistance;
        int lod = GetLod(space.GetChunkSize(), clipRadius);
        view = space
            .GetChunksWithin(closestPointToObserver, clipRadius)
            .Select(chunk => new ViewChunk(chunk, lod))
            .ToArray();
    }

    private int GetLod(int chunkSize, float clipRadius) {
        if (clipRadius < chunkSize * 0.5) {
            return 0;
        }
        if (clipRadius < chunkSize) {
            return 1;
        }
        if (clipRadius < chunkSize * 1.5) {
            return 2;
        }
        if (clipRadius < chunkSize * 2) {
            return 3;
        }
        if (clipRadius < chunkSize * 2.5) {
            return 4;
        }
        if (clipRadius < chunkSize * 3) {
            return 5;
        }
        return 6;
    }
}

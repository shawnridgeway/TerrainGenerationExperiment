using System.Linq;
using UnityEngine;

public class ZoomLevelViewer : Viewer {
    private readonly ChunkedSpace space;
    private readonly Transform observer;
    private readonly float ratio;
    private readonly float updateDistace; // Distance traveled from previousObserverPosition before next update.
    private Point previousObserverPoint;
    private float previousObserverAltitude;
    private ViewChunk[] view = null;

    public ZoomLevelViewer(ChunkedSpace space, Transform observer, float ratio = 1f) {
        this.space = space;
        this.observer = observer;
        this.ratio = ratio;
        this.updateDistace = space.GetChunkScale() / 10f;
    }

    public ViewChunk[] View() {
        Point observerPoint = space.GetPointFromPosition(observer.position);
        float altitude = space.GetDistanceFromSurface(observer.position);
        if (view == null) {
            previousObserverPoint = observerPoint;
            previousObserverAltitude = altitude;
            view = GetVisible(observerPoint);
            return view;
        }
        bool pointIsOutsideUpdateDistance = !space.IsPointInRange(observerPoint, previousObserverPoint, updateDistace);
        // In this viewer we are also interesed in movement along the space normal
        bool normalDimensionIsOutsideUpdateDistance = Mathf.Abs(altitude - previousObserverAltitude) > updateDistace;
        if (pointIsOutsideUpdateDistance || normalDimensionIsOutsideUpdateDistance) {
            previousObserverPoint = observerPoint;
            previousObserverAltitude = altitude;
            view = GetVisible(observerPoint);
        }
        return view;
    }

    private ViewChunk[] GetVisible(Point observerPoint) {
        Vector3 pointOnGround = observerPoint.GetPosition();
        float distanceFromObserverToGround = Vector3.Distance(
            observer.position,
            pointOnGround
        );
        float clipRadius = ratio * distanceFromObserverToGround;
        MeshLod visibleLod = GetLod(space.GetChunkScale(), clipRadius);
        return space
            .GetChunksWithin(observerPoint, clipRadius)
            .Select(chunk => new ViewChunk(chunk, visibleLod))
            .ToArray();
    }

    private MeshLod GetLod(float chunkScale, float clipRadius) {
        if (clipRadius < chunkScale * 0.5f) {
            return new MeshLod(0);
        }
        if (clipRadius < chunkScale) {
            return new MeshLod(1);
        }
        if (clipRadius < chunkScale * 1.5f) {
            return new MeshLod(2);
        }
        if (clipRadius < chunkScale * 2f) {
            return new MeshLod(3);
        }
        if (clipRadius < chunkScale * 2.5f) {
            return new MeshLod(4);
        }
        if (clipRadius < chunkScale * 3f) {
            return new MeshLod(5);
        }
        return new MeshLod(6);
    }
}

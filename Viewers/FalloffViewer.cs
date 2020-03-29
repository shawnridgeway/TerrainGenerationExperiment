using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class FalloffViewer : Viewer {
    private readonly ChunkedSpace space;
    private readonly Transform observer;
    private readonly SortedList<MeshLod, float> visibilityLodPlanes; // TODO: extract into struct, add validation on raising values
    private readonly SortedList<MeshLod, float> collisionLodPlanes; // TODO: extract into struct, add validation on raising values
    private readonly float updateDistace; // Distance traveled from previousObserverPosition before next update.
    private Point previousObserverPoint;
    private ViewChunk[] view = null;

    public FalloffViewer(
        ChunkedSpace space,
        Transform observer,
        SortedList<MeshLod, float> visibilityLodPlanes = null,
        SortedList<MeshLod, float> collisionLodPlanes = null
    ) {
        this.space = space;
        this.observer = observer;
        this.visibilityLodPlanes = visibilityLodPlanes ?? GetDefaultVisibilityLodPlanes();
        this.collisionLodPlanes = collisionLodPlanes ?? GetDefaultCollisionLodPlanes();
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
        float furthestVisiblePlane = visibilityLodPlanes.Last().Value;
        float furthestCollisionPlane = collisionLodPlanes.Last().Value;
        float furthestPlane = Mathf.Max(furthestVisiblePlane, furthestCollisionPlane);
        Chunk[] chunks = space.GetChunksWithin(observerPoint, furthestPlane);
        return chunks
            .Select(chunk => {
                MeshLod lod = GetMaxLod(visibilityLodPlanes, chunk);
                MeshLod colliderLod = GetMaxLod(collisionLodPlanes, chunk);
                return new ViewChunk(chunk, lod, colliderLod);
            })
            .ToArray();
    }

    private MeshLod GetMaxLod(SortedList<MeshLod, float> lodPlanes, Chunk chunk) {
        MeshLod maxLod = lodPlanes.First().Key;
        Point closestPoint = space.GetPointInSpace(observer.position);
        foreach (var entry in lodPlanes) {
            MeshLod meshLod = entry.Key;
            float distanceThreshold = entry.Value;
            if (!space.IsChunkInRange(closestPoint, chunk, distanceThreshold)) {
                maxLod = meshLod;
            } else {
                break;
            }
        }
        return maxLod;
    }

    private SortedList<MeshLod, float> GetDefaultVisibilityLodPlanes() {
        SortedList<MeshLod, float> defaultLodPlanes = new SortedList<MeshLod, float>();
        defaultLodPlanes.Add(new MeshLod(0), 20);
        defaultLodPlanes.Add(new MeshLod(2), 100);
        defaultLodPlanes.Add(new MeshLod(4), 200);
        defaultLodPlanes.Add(new MeshLod(6), 300);
        return defaultLodPlanes;
    }

    private SortedList<MeshLod, float> GetDefaultCollisionLodPlanes() {
        SortedList<MeshLod, float> defaultLodPlanes = new SortedList<MeshLod, float>();
        defaultLodPlanes.Add(new MeshLod(2), 30);
        return defaultLodPlanes;
    }
}

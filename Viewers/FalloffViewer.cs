using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class FalloffViewer : Viewer {
    private readonly ChunkedSpace space;
    private readonly Transform observer;
    private readonly SortedList<MeshLod, float> visiblityLodPlanes; // TODO: extract into struct, add validation on raising values
    private readonly SortedList<MeshLod, float> collisionLodPlanes; // TODO: extract into struct, add validation on raising values
    private readonly float updateDistaceSqr; // Distance traveled from previousObserverPosition before next update.
    private Vector3 previousObserverPosition;
    private ViewChunk[] view = null;

    public FalloffViewer(
        ChunkedSpace space,
        Transform observer,
        SortedList<MeshLod, float> visiblityLodPlanes = null,
        SortedList<MeshLod, float> collisionLodPlanes = null
    ) {
        this.space = space;
        this.observer = observer;
        this.visiblityLodPlanes = visiblityLodPlanes ?? GetDefaultLodPlanes();
        this.collisionLodPlanes = collisionLodPlanes ?? GetDefaultCollisionLodPlanes();
        this.updateDistaceSqr = space.GetChunkSize() * space.GetChunkSize() / (10f * 10f);
    }

    public ViewChunk[] View() {
        Vector3 observerPosition = space.GetClosestPointTo(observer.position).GetLocation();
        if ((previousObserverPosition - observerPosition).sqrMagnitude > updateDistaceSqr || view == null) {
            previousObserverPosition = observerPosition;
            view = GetVisible(observerPosition);
        }
        return view;
    }

    private ViewChunk[] GetVisible(Vector3 observerPosition) {
        float furthestVisiblePlane = visiblityLodPlanes.Last().Value;
        float furthestCollisionPlane = collisionLodPlanes.Last().Value;
        float furthestPlane = Mathf.Max(furthestVisiblePlane, furthestCollisionPlane);
        Chunk[] chunks = space.GetChunksWithin(observerPosition, furthestPlane);
        return chunks
            .Select(chunk => {
                MeshLod lod = GetMaxLod(visiblityLodPlanes, chunk.GetCenterLocation());
                MeshLod colliderLod = GetMaxLod(collisionLodPlanes, chunk.GetCenterLocation());
                return new ViewChunk(chunk, lod, colliderLod);
            })
            .ToArray();
    }

    private SortedList<MeshLod, float> GetDefaultLodPlanes() {
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

    private MeshLod GetMaxLod(SortedList<MeshLod, float> lodPlanes, Vector3 chunkCenter) {
        MeshLod maxLod = lodPlanes.First().Key;
        float distance = Vector3.Distance(observer.position, chunkCenter);
        foreach (var entry in lodPlanes) {
            MeshLod meshLod = entry.Key;
            float distanceThreshold = entry.Value;
            if (distance > distanceThreshold) {
                maxLod = meshLod;
            } else {
                break;
            }
        }
        return maxLod;
    }
}

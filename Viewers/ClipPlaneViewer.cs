using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipPlaneViewer : Viewer {
    private readonly ChunkedSpace space;
    private readonly Transform observer;
    private readonly float clipDistace;
    private readonly float updateDistaceSqr; // Distance traveled from previousObserverPosition before next update.
    private Vector3 observerPosition;
    private Vector3 previousObserverPosition;
    private Chunk[] visibleChunks;

    public ClipPlaneViewer(ChunkedSpace space, Transform observer, float clipDistace) {
        this.space = space;
        this.observer = observer;
        this.clipDistace = clipDistace;
        this.updateDistaceSqr = (clipDistace * clipDistace) / (10f * 10f);
    }

    public Chunk[] View() {
        UpdateObserverPostion();
        if ((previousObserverPosition - observerPosition).sqrMagnitude > updateDistaceSqr || visibleChunks == null) {
            previousObserverPosition = observerPosition;
            UpdateVisible();
        }
        return visibleChunks;
    }

    void UpdateObserverPostion() {
        observerPosition = new Vector3(observer.position.x, 0, observer.position.z);
    }

    void UpdateVisible() {
        visibleChunks = space.GetChunksWithin(observerPosition, clipDistace);
    }
}

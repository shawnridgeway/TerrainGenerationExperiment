using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedViewer : Viewer {
    private readonly Chunk[] chunks;

    public FixedViewer(ChunkedSpace space, float distace) {
        chunks = space.GetChunksWithin(Vector3.zero, distace);
    }

    public Chunk[] View() {
        return chunks;
    }
}

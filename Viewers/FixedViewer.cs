using System.Linq;
using UnityEngine;

public class FixedViewer : Viewer {
    private readonly ViewChunk[] view;

    public FixedViewer(ChunkedSpace space, Vector3 origin, float distace, int lod = 0) {
        Chunk[] chunks = space.GetChunksWithin(space.GetClosestPointTo(origin).GetLocation(), distace);
        view = chunks
            .Select(chunk => new ViewChunk(chunk, lod))
            .ToArray();
    }

    public ViewChunk[] View() {
        return view;
    }
}

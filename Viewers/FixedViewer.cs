using System.Linq;
using UnityEngine;

public class FixedViewer : Viewer {
    private readonly ViewChunk[] view;

    public FixedViewer(ChunkedSpace space, Vector3 origin, float distace, MeshLod? visibleLod = null) {
        Point originPoint = space.GetPointFromPosition(origin);
        Chunk[] chunks = space.GetChunksWithin(originPoint, distace);
        view = chunks
            .Select(chunk => new ViewChunk(chunk, visibleLod ?? new MeshLod(0)))
            .ToArray();
    }

    public ViewChunk[] View() {
        return view;
    }
}

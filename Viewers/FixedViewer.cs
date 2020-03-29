using System.Linq;
using UnityEngine;

public class FixedViewer : Viewer {
    private readonly ViewChunk[] view;

    public FixedViewer(ChunkedSpace space, Vector3 origin, float distace, MeshLod? lod = null) {
        Chunk[] chunks = space.GetChunksWithin(space.GetClosestPointTo(origin).GetLocation(), distace);
        view = chunks
            .Select(chunk => new ViewChunk(chunk, lod ?? new MeshLod(0)))
            .ToArray();
    }

    public ViewChunk[] View() {
        return view;
    }
}

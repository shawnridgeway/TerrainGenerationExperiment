using System.Linq;
using UnityEngine;

public class FixedViewer : Viewer {
    private readonly ViewChunk[] view;
    private bool firstIter = true;

    public FixedViewer(ChunkedSpace space, Vector3 origin, float distace, MeshLod? visibleLod = null) {
        Point originPoint = space.GetPointFromPosition(origin);
        Chunk[] chunks = space.GetChunksWithin(originPoint, distace);
        view = chunks
            .Select(chunk => new ViewChunk(chunk, visibleLod ?? new MeshLod(0)))
            .ToArray();
        Debug.Log(chunks.Length);
        if (firstIter) {
            foreach (Chunk chunk in chunks) {
                if (chunk is SphericalChunk sChunk) {
                    Debug.Log(sChunk.GetCenterCoordinate());
                }
            }
        }
        firstIter = false;
    }

    public ViewChunk[] View() {
        return view;
    }
}

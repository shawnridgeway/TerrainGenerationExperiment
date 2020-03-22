using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ViewChunk {
    public readonly Chunk chunk;
    public readonly int lod;

    public ViewChunk(Chunk chunk, int lod) {
        this.chunk = chunk;
        this.lod = lod;
	}
}

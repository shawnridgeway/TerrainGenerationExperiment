public struct ViewChunk {
    public readonly Chunk chunk;
    public readonly MeshLod? visibleLod;
    public readonly MeshLod? tangibleLod;

    public ViewChunk(
        Chunk chunk,
        MeshLod? visibleLod = null,
        MeshLod? tangibleLod = null
    ) {
        this.chunk = chunk;
        this.visibleLod = visibleLod;
        this.tangibleLod = tangibleLod;
    }

    public bool IsVisible() {
        return !(visibleLod is null);
    }

    public bool IsTangible() {
        return !(tangibleLod is null);
    }
}

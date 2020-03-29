public struct ViewChunk {
    public readonly Chunk chunk;
    public readonly MeshLod lod;
    public readonly MeshLod? colliderLod;

    public ViewChunk(Chunk chunk, MeshLod lod, MeshLod? colliderLod = null) {
        this.chunk = chunk;
        this.lod = lod;
        this.colliderLod = colliderLod;
    }

    public bool HasCollider() {
        return colliderLod != null;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class MeshDataGenerator {
    private readonly ChunkedSpace space;
    private readonly TerrainTransform terrainTransform;

    public MeshDataGenerator(ChunkedSpace space, TerrainTransform terrainTransform) {
        this.space = space;
        this.terrainTransform = terrainTransform;
    }

    public MeshData GetMeshData(Chunk chunk, MeshLod meshLod) {
        Vector3 chunkCenterLocation = chunk.GetCenterPosition();

        // The number of points to skip for each mesh vertex
        int borderSize = 1;
        int vertexInterval = meshLod.GetInterval();

        // Get points necessary for mesh, including a border of one borderSize unit
        IEnumerable<Point> allPoints = chunk.GetPoints(vertexInterval, borderSize);

        int chunkPointCount = space.GetChunkCount(vertexInterval, 0);
        int totalPointCount = space.GetChunkCount(vertexInterval, borderSize);
        int borderPointCount = totalPointCount - chunkPointCount;

        MeshDataBuilder meshDataBuilder = new MeshDataBuilder(space, chunkPointCount, borderPointCount, vertexInterval, borderSize);

        foreach (Point point in allPoints) {
            // Get the height value for each point
            float value = terrainTransform.Process(point);

            // Verticies are centered in space give limitations on space size of meshes
            Vector3 vertex = point.GetPosition()
                + space.GetNormalFromPosition(point.GetPosition()) * value
                - chunkCenterLocation;

            meshDataBuilder.AddVertex(vertex);
        }

        return meshDataBuilder.Build();
    }
}

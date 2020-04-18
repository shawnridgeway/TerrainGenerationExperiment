using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CartesianMeshGenerator : MeshGenerator {
    private readonly ChunkedSpace space;
    private readonly TerrainTransform terrainTransform;

    public CartesianMeshGenerator(ChunkedSpace space, TerrainTransform terrainTransform) {
        this.space = space;
        this.terrainTransform = terrainTransform;
    }

    protected override MeshData GetMeshData(Chunk chunk, MeshLod meshLod) {
        Vector3 chunkCenterLocation = chunk.GetCenterPosition();
        int chunkWidth = PlanarSpace.chunkWidth;
        int originalVerticiesPerLine = PlanarSpace.chunkPointCount;

        // The number of points to skip for each mesh vertex
        int borderSize = 1;
        int vertexInterval = meshLod.GetInterval(); // 1,2,4,6,8,10,12
        int verticiesPerLine = ((originalVerticiesPerLine - 1) / vertexInterval) + 1; // 240, 120, 60, 40, 30, 24, 20
        int verticiesPerLineWithBorder = verticiesPerLine + (borderSize * 2);
        
        // Get points necessary for mesh, including a border of one vertexInterval unit
        IEnumerable<Point> borderedChunkPoints = chunk.GetPoints(vertexInterval, borderSize);

        // Get height values as floats
        IEnumerable<(Point, float)> processedPoints = terrainTransform.Process(borderedChunkPoints)
            .Zip(borderedChunkPoints, (value, point) => (point, value));

        MeshBuilder meshDataBuilder = new MeshBuilder(
            verticiesPerLineWithBorder,
            (Vector3 position) => !chunk.IsPositionInChunk(position + chunkCenterLocation)
        );

        foreach ((Point, float) processedPoint in processedPoints) {
            (Point point, float value) = processedPoint;

            Vector3 vertexPosition = point.GetPosition()
                + space.GetNormalFromPosition(point.GetPosition()) * value
                - chunkCenterLocation;

            // TODO: adapt for general use
            Vector2 uv = new Vector2(
                0.5f + ((point.GetPosition().x - chunkCenterLocation.x) / chunkWidth),
                0.5f - ((point.GetPosition().z - chunkCenterLocation.z) / chunkWidth)
            );

            meshDataBuilder.AddVertexAt(vertexPosition, uv);
        }

        return meshDataBuilder.Build();
    }


}

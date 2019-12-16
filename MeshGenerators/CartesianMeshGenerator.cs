using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CartesianMeshGenerator : MeshGenerator {
    private readonly float altitudeScale;
    private readonly TerrainTransform terrainTransform;

    public CartesianMeshGenerator(TerrainTransform terrainTransform, float altitudeScale = 240) {
        this.terrainTransform = terrainTransform;
        this.altitudeScale = altitudeScale;
    }

    public Mesh Process(Chunk chunk, int levelOfDetail) {
        Vector3 chunkCenterLocation = chunk.GetCenterLocation();
        int chunkSize = chunk.GetSize();

        int originalVerticiesPerLine = chunkSize + 1; // +1 for verticies, not squares
        // The number of points to skip for each mesh vertex
        int vertexInterval = levelOfDetail == 0 ? 1 : levelOfDetail * 2; // 1,2,4,6,8,10,12
        int verticiesPerLine = ((originalVerticiesPerLine - 1) / vertexInterval) + 1; // 240, 120, 60, 40, 30, 24, 20
        int verticiesPerLineWithBorder = verticiesPerLine + 2;
        
        // Get points necessary for mesh, including a border of one vertexInterval unit
        IEnumerable<Point> borderedChunkPoints = chunk.GetPoints(vertexInterval, 1);

        // Get height values as floats
        IEnumerable<(Point, float)> processedPoints = terrainTransform.Process(borderedChunkPoints)
            .Zip(borderedChunkPoints, (heightValue, point) => (point, heightValue));

        MeshBuilder meshBuilder = new MeshBuilder(verticiesPerLineWithBorder, (int index) => index < verticiesPerLineWithBorder || // First row
                index >= (verticiesPerLineWithBorder - 1) * verticiesPerLineWithBorder || // Last row
                index % verticiesPerLineWithBorder == 0 || // First column
                index % verticiesPerLineWithBorder == verticiesPerLineWithBorder - 1 // Last column
        );

        foreach ((Point, float) processedPoint in processedPoints) {
            (Point point, float heightValue) = processedPoint;

            Vector3 vertexPosition = new Vector3(
                point.GetLocation().x - chunkCenterLocation.x,
                heightValue * altitudeScale,
                point.GetLocation().z - chunkCenterLocation.z
            );

            Vector2 uv = new Vector2(
                (float)0.5 + ((point.GetLocation().x - chunkCenterLocation.x) / chunkSize),
                (float)0.5 - ((point.GetLocation().z - chunkCenterLocation.z) / chunkSize)
            );

            meshBuilder.AddVertexAt(vertexPosition, uv);
        }

        return meshBuilder.Build();
    }
}

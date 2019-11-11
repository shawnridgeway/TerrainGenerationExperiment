using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTerrain : MonoBehaviour {
    public Material material;
    public Transform observer;
    public AnimationCurve animationCurve;
    TerrainRenderer terrainRenderer;

    void Start() {
        ChunkedSpace space = new CartesianSpace();
        //Chunk[] chunks = space.GetChunksWithin(new Vector3(0, 0, 0), 200);
        //Point[] points = chunks[0].GetPoints();
        //print(chunks.Length);
        //Vector3 centerLoaction = new Vector3(1, 2, 3);
        //print(centerLoaction.ToString().GetHashCode());
        //print(new Vector3(1, 2, 3).ToString());
        
        TerrainTransform noise = new Noise(new NoiseOptions(scale: 250, octaves: 7, persistance: .4f, seed: 19));
        TerrainTransform addition = new Addition(
            new Constant(new ConstantOptions(0)),
            noise
        );
        TerrainTransform curve = new Curve(addition, new CurveOptions(animationCurve));
        TerrainTransform erode = new LocalErosion(curve, new LocalErosionOptions());
        //IEnumerable<float> result = addition.Process(new Point[] {
        //    new CartesianPoint(new Vector3(1,1,1)),
        //    new CartesianPoint(new Vector3(0,0,0))
        //});
        CartesianMeshGenerator meshGenerator = new CartesianMeshGenerator(1, 200, erode, material);
        ClipPlaneViewer viewer = new ClipPlaneViewer(space, observer, 200);
        terrainRenderer = new TerrainRenderer(viewer, transform, meshGenerator);
    }

    void Update() {
        terrainRenderer.Render();
    }
}

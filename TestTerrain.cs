﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTerrain : MonoBehaviour {
    public Material material;
    public Transform observer;
    public AnimationCurve animationCurve;
    public Texture2D image;
    TerrainRenderer terrainRenderer;
    TerrainRenderer terrainRenderer2;

    void Start() {
        ChunkedSpace space = new CartesianSpace();
        //ChunkedSpace space2 = new CartesianSpace(.1f);
        //Chunk[] chunks = space.GetChunksWithin(new Vector3(0, 0, 0), 200);
        //Point[] points = chunks[0].GetPoints();
        //print(chunks.Length);
        //Vector3 centerLoaction = new Vector3(1, 2, 3);
        //print(centerLoaction.ToString().GetHashCode());
        //print(new Vector3(1, 2, 3).ToString());

        TerrainTransform custom = new Custom(new CustomOptions(image));
        TerrainTransform noise = new Noise(new NoiseOptions(scale: 250, octaves: 7, persistance: .4f, seed: 19));
        TerrainTransform addition = new Addition(
            new Constant(new ConstantOptions(0)),
            noise
        );
        TerrainTransform curve = new Curve(noise, new CurveOptions(animationCurve));
        TerrainTransform exaggeration = new Exaggeration(noise, new ExaggerationOptions(1.75f));
        TerrainTransform exaggeration2 = new Exaggeration(noise, new ExaggerationOptions(.5f));
        //TerrainTransform erode = new LocalErosion(curve, new LocalErosionOptions());
        //IEnumerable<float> result = addition.Process(new Point[] {
        //    new CartesianPoint(new Vector3(1,1,1)),
        //    new CartesianPoint(new Vector3(0,0,0))
        //});
        TerrainTransform voronoi = new VoronoiTesselation(new VoronoiTesselationOptions(new[]{
            new Constant(new ConstantOptions(.2f)),
            new Constant(new ConstantOptions(-.2f)),
            new Constant(new ConstantOptions(.1f)),
            new Constant(new ConstantOptions(-.1f)),
            new Constant(new ConstantOptions(0f))
        }, 7, 300, seed: 11));

        CartesianMeshGenerator meshGenerator = new CartesianMeshGenerator(noise, 200);
        CartesianMeshGenerator meshGenerator2 = new CartesianMeshGenerator(voronoi, 200);
        ClipPlaneViewer viewer = new ClipPlaneViewer(space, observer, 1);
        terrainRenderer = new TerrainRenderer(transform, viewer, meshGenerator, material);
        terrainRenderer2 = new TerrainRenderer(transform, viewer, meshGenerator2, material);
    }

    void Update() {
        //terrainRenderer.Render();
        terrainRenderer2.Render();
    }
}

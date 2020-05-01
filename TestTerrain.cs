using UnityEngine;
using System;

public class TestTerrain : MonoBehaviour {
    public Material material;
    public Transform observer;
    public AnimationCurve animationCurve;
    //public Texture2D image;
    TerrainRenderer terrainRenderer;
    //TerrainRenderer terrainRenderer2;
    private ChunkedSpace space;

    private bool initialRenderComplete = false;
    private bool previousRenderComplete = true;
    private DateTime renderBeginTime;
    private bool isDebugOn = false;

    void Start() {
        space = new PlanarSpace();
        //ChunkedSpace space2 = new CartesianSpace(.1f);
        //Chunk[] chunks = space.GetChunksWithin(new Vector3(0, 0, 0), 200);
        //Point[] points = chunks[0].GetPoints();
        //print(chunks.Length);
        //Vector3 centerLoaction = new Vector3(1, 2, 3);
        //print(centerLoaction.ToString().GetHashCode());
        //print(new Vector3(1, 2, 3).ToString());

        //TerrainTransform custom = new Custom(new CustomOptions(image));
        //TerrainGenerator noise = new Noise(new NoiseOptions(scale: 250, octaves: 7, persistance: .4f, seed: 19));
        //TerrainGenerator scaledNoise = new Scalar(noise, new ScalarOptions(5f));
        //TerrainTransform addition = new Addition(
        //    new Constant(new ConstantOptions(0)),
        //    noise
        //);
        //TerrainTransform curve = new Curve(noise, new CurveOptions(animationCurve));
        //TerrainTransform exaggeration = new Exaggeration(noise, new ExaggerationOptions(1.75f));
        //TerrainTransform exaggeration2 = new Exaggeration(noise, new ExaggerationOptions(.5f));
        //TerrainTransform erode = new LocalErosion(curve, new LocalErosionOptions());
        //IEnumerable<float> result = addition.Process(new Point[] {
        //    new CartesianPoint(new Vector3(1,1,1)),
        //    new CartesianPoint(new Vector3(0,0,0))
        //});
        //Voronoi voronoiModel = new Voronoi(new VoronoiOptions(3, new Vector3(300, 0, 300), seed: 11));
        //TerrainTransform voronoi = new VoronoiTesselation(new VoronoiTesselationOptions(new[]{
        //    new Constant(new ConstantOptions(.2f)),
        //    new Constant(new ConstantOptions(-.2f)),
        //    new Constant(new ConstantOptions(.1f)),
        //    new Constant(new ConstantOptions(-.1f)),
        //    new Constant(new ConstantOptions(0f))
        //}, voronoiModel));

        //TerrainTransform gradient = new RadialGradient(
        //    new RadialGradientOptions(
        //        100,
        //        new Vector2(0,0),
        //        //Mathf.PI * 0.2f,
        //        new GradientPattern(
        //            new GradientPatternOptions(
        //                new GradientPatternKey[] {
        //                    new GradientPatternKey(0, 0),
        //                    new GradientPatternKey(.5f, 5),
        //                    new GradientPatternKey(1, 0)
        //                },
        //                GradientPatternMode.Repeat
        //            )
        //        )
        //    )
        //);

        //TerrainTransform plates = new Scalar(
        //    new Samples.TectonicPlates(),
        //    new ScalarOptions(2f)
        //);
        //TerrainGenerator mask = new Mask(
        //    new Gain(scaledNoise, new ExaggerationOptions(1.5f)),
        //    new Gain(scaledNoise, new ExaggerationOptions(.9f)),
        //    new MaskOptions(
        //        mask: new Curve(
        //            new InverseLerp(
        //                new SimpleVoronoi(
        //                    new SimpleVoronoiOptions(
        //                        voronoiModel,
        //                        (VoronoiRegion region, Point point) => {
        //                            return region.GetDistanceToClosestBorder(point.GetPosition());
        //                        }
        //                    )
        //                ),
        //                new InverseLerpOptions(0, 50)
        //            ),
        //            new CurveOptions(null)
        //        ),
        //        0f,
        //        1f
        //    )
        //);
        //TerrainGenerator m = new Curve(
        //    scaledNoise,
        //    new CurveOptions(animationCurve)
        //);
        //TerrainTransform voro = new SimpleVoronoi(
        //    new SimpleVoronoiOptions(
        //        voronoiModel,
        //        (VoronoiRegion region, Point point) => {
        //            return region.GetDistanceToClosestBorder(point.GetLocation());
        //        }
        //    )
        //);
        //TerrainTransform vt = new VoronoiTesselation(new VoronoiTesselationOptions(new TerrainTransform[] {
        //    //new Constant(new ConstantOptions(0)),
        //    //new Constant(new ConstantOptions(1)),
        //    //new Constant(new ConstantOptions(2)),
        //    //new Constant(new ConstantOptions(3)),
        //    new Constant(new ConstantOptions(4)),
        //    new Constant(new ConstantOptions(5)),
        //    new Constant(new ConstantOptions(6))
        //}, voronoiModel));
        //TerrainTransform voro2 = new SimpleVoronoi(new SimpleVoronoiOptions(voronoiModel, (VoronoiRegion region, Point point) => {
        //    return Mathf.Min(region.GetDistanceFromCenter(point.GetLocation()), 20f);
        //}));
        //TerrainTransform testTerr = new LocalErosion(new Scalar(noise, new ScalarOptions(5f)), new LocalErosionOptions());

        //TerrainGenerator largeNoise = new Add(new Scalar(
        //    new Noise(new NoiseOptions(scale: 800, octaves: 8, persistance: .4f, seed: 19)),
        //    new ScalarOptions(10f)
        //), new Constant(new ConstantOptions(3)));
        //TerrainGenerator largeNoiseEx = new Scalar(
        //    new Gain(
        //        largeNoise,
        //        new ExaggerationOptions(2f)
        //    ),
        //    new ScalarOptions(25)
        //);

        TerrainGenerator terrain = new BillowNoise() * 30f;

        //MeshGenerator meshGenerator = new MeshGenerator(space, largeNoiseEx);
        //MeshGenerator meshGenerator2 = new CartesianMeshGenerator(space, largeNoiseEx);
        //CartesianMeshGenerator meshGenerator2 = new CartesianMeshGenerator(scaledNoise, 20);
        CutoffViewer viewer = new CutoffViewer(space, observer, clipDistace: 1000, visibleLod: new MeshLod(2));
        //Viewer viewer = new FalloffViewer(space, observer);
        //ZoomLevelViewer zoomViewer = new ZoomLevelViewer(space, observer);
        terrainRenderer = new TerrainRenderer(transform, space, viewer, terrain, material);
        //terrainRenderer2 = new TerrainRenderer(transform, viewer, meshGenerator2, material);
        terrainRenderer.OnRenderFinished += HandleRenderComplete;
        //terrainRenderer2 = new TerrainRenderer(transform, viewer, meshGenerator2, material);

        float startPosition = terrain.GetValue(space.GetPointFromPosition(Vector3.zero));
        SetStartPosition(startPosition);
    }

    void Update() {
        terrainRenderer.Render();
        //terrainRenderer2.Render();
    }

    private void SetStartPosition(float startPosition) {
        float observerHeight = observer.transform.localScale.y;
        observer.transform.position = new Vector3(0, startPosition + observerHeight + 2, 0);
    }

    private void StartGame() {
        Debug.Log("Game Start!");
        observer.GetComponent<ObserverController>().isActive = true;
    }

    private void HandleRenderComplete(bool updateCompletelyApplied) {
        if (!initialRenderComplete && updateCompletelyApplied) {
            StartGame();
            initialRenderComplete = true;
        }

        if (!isDebugOn) {
            return;
        }
        if (previousRenderComplete && !updateCompletelyApplied) {
            Debug.Log("Beginning Render...");
            renderBeginTime = DateTime.Now;
        }
        if (!previousRenderComplete && updateCompletelyApplied) {
            Debug.Log("Render Complete in " + (DateTime.Now - renderBeginTime));
        }
        previousRenderComplete = updateCompletelyApplied;
    }

    public Vector3 GetSurfaceNormal(Vector3 position) {
        return space.GetNormalFromPosition(position);
    }
}

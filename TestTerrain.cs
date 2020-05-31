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
    private TerrainGenerator terrain;

    private bool initialRenderComplete = false;
    private bool previousRenderComplete = true;
    private DateTime renderBeginTime;
    private bool isDebugOn = false;

    void Awake() {
        space = new SphericalSpace(150, 4);
        //space = new PlanarSpace();

        terrain = new Gain(new RidgeNoise() / 2f, 0.3f) * 50f;
        //terrain = new Constant(10);

        //CutoffViewer viewer = new CutoffViewer(space, observer, clipDistace: 1000, visibleLod: new MeshLod(2));

        Viewer viewer = new FixedViewer(space, observer.transform.position, 50f, new MeshLod(2));
        //ZoomLevelViewer zoomViewer = new ZoomLevelViewer(space, observer);
        terrainRenderer = new TerrainRenderer(transform, space, viewer, terrain, material);
        //terrainRenderer2 = new TerrainRenderer(transform, viewer, meshGenerator2, material);
        terrainRenderer.OnRenderFinished += HandleRenderComplete;
        //terrainRenderer2 = new TerrainRenderer(transform, viewer, meshGenerator2, material);
    }

    void Update() {
        terrainRenderer.Render();
        //terrainRenderer2.Render();
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

        if (isDebugOn) {
            if (previousRenderComplete && !updateCompletelyApplied) {
                Debug.Log("Beginning Render...");
                renderBeginTime = DateTime.Now;
            }
            if (!previousRenderComplete && updateCompletelyApplied) {
                Debug.Log("Render Complete in " + (DateTime.Now - renderBeginTime));
            }
            previousRenderComplete = updateCompletelyApplied;
        }
    }

    public Vector3 GetSurfaceNormal(Vector3 position) {
        return space.GetNormalFromPosition(position);
    }

    public Vector3 GetHeightAtPosition(Vector3 targetStartPosition) {
        Point closestPoint = space.GetPointFromPosition(targetStartPosition);
        float heightValue = terrain.GetValue(closestPoint);
        Vector3 surfaceNormal = space.GetNormalFromPosition(targetStartPosition);
        return closestPoint.GetPosition() + surfaceNormal * heightValue;
    }
}

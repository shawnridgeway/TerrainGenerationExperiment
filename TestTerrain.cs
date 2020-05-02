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

        TerrainGenerator terrain = new RidgeNoise() * 30f;

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

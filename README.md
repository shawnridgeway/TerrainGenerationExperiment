# Terrain Generation

An extensible, composable, declarative framework for terrain generation.

TerrainRenderer
├── Viewer
│   └── Space
└── MeshGenerator
    └── TerrainTransform

## Example

```
using UnityEngine;

public class TestTerrain : MonoBehaviour {
    public Material material;
    public Transform observer;
    TerrainRenderer terrainRenderer;

    void Start() {
        ChunkedSpace space = new CartesianSpace();
        TerrainTransform noise = new Noise(new NoiseOptions(scale: 250, octaves: 7, persistance: .4f, seed: 19));
        TerrainTransform scaledNoise = new Scalar(noise, new ScalarOptions(5f));
        MeshGenerator meshGenerator = new CartesianMeshGenerator(scaledNoise, 20);
        Viewer viewer = new ClipPlaneViewer(space, observer, 1);
        terrainRenderer = new TerrainRenderer(transform, viewer, meshGenerator, material);
    }

    void Update() {
        terrainRenderer.Render();
    }
}

```

## Components

### TerrainTransform

TerrainsTransforms are composable nodes that generate the terrain information. In 2.5D thes are height values, in 3D these would be voxel values.

#### Constant

#### Custom

#### Linear Gradient

#### Radial Gradient

### Spaces

The rules for which points are included in the space, how they relate to eachother and how they are chunked for rendering. 

### Models

Generic models not related to the core of this project.

### Viewers

Viewers contain the logic for deciding which chunks should be rendered at what level of detail (lod) given the input world state.

### Renderers

Renderers are in charge of mutating the game state, creating new game objects and caching created objects.

### MeshGenerators

MeshGenerators do the actual building of meshes given a chunk and its computed values.


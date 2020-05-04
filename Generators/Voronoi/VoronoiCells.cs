using UnityEngine;
using System;

public class VoronoiCells : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    // 3D Constructor
    public VoronoiCells(
        Func<int, int, int, float> cellValueSource,
        int seed = 0,
        int period = 0,
        float frequency = 1 / 240f
    ) {
        _generator = new CoherentNoise.Generation.Voronoi.VoronoiCells(seed, cellValueSource) {
            Frequency = frequency,
            Period = period
        };
    }

    // 2D Constructor
    public VoronoiCells(
        Func<int, int, float> cellValueSource,
        int seed = 0,
        int period = 0,
        float frequency = 1 / 240f
    ) {
        _generator = new SwapYZGenerator(
            new CoherentNoise.Generation.Voronoi.VoronoiCells2D(seed, cellValueSource) {
                Frequency = frequency,
                Period = period
            }
        );
    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}

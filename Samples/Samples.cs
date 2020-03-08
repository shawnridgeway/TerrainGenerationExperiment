using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public static class Samples {
    public static TerrainTransform SimpleVolcano(float scale) {
        //TerrainTransform curve = new Curve(noise, new CurveOptions(animationCurve));
        //TerrainTransform exaggeration = new Exaggeration(noise, new ExaggerationOptions(1.75f));
        //TerrainTransform exaggeration2 = new Exaggeration(noise, new ExaggerationOptions(.5f));
        TerrainTransform gradient = new RadialGradient(
            new RadialGradientOptions(
                scale,
                new Vector2(0f, 0f),
                //0f,
                new GradientPattern(
                    new GradientPatternOptions(
                        new GradientPatternKey[] {
                            new GradientPatternKey(0f, 3f),
                            new GradientPatternKey(.4f, 5f),
                            new GradientPatternKey(.7f, 2f),
                            new GradientPatternKey(1f, 0f)
                        },
                        GradientPatternMode.Clamp
                    )
                )
            )
        );
        TerrainTransform noise = new Noise(new NoiseOptions(scale: 250, octaves: 7, persistance: .4f, seed: 19));
        TerrainTransform addition = new Addition(
            gradient,
            new Scalar(noise, new ScalarOptions(2f))
        );
        return addition;
    }


    public static TerrainTransform TectonicPlates() {
        int count = 10;
        int scale = 100;
        Voronoi voronoi = new Voronoi(
            new VoronoiOptions(count: count, scale: scale, seed: 4)
        );
        Vector3[] sites = voronoi.GetSites();
        GradientPattern gradientPattern = new GradientPattern(
            new GradientPatternOptions(new[] {
                new GradientPatternKey(0, 1),
                new GradientPatternKey(1, 0)
            })
        );
        IEnumerable<TerrainTransform> fills = new List<TerrainTransform>();
        IEnumerable<Vector2> centers = new List<Vector2>();
        for (int m = -1; m <= 1; m++) {
            for (int n = -1; n <= 1; n++) {
                foreach (Vector3 site in sites) {
                    Vector2 center = new Vector2(site.x + m * (float)scale, site.z + n * (float)scale);
                    centers = centers.Append(center);
                    fills = fills.Append(
                        new RadialGradient(
                            new RadialGradientOptions(
                                scale: 30f,
                                position: center,
                                gradientPattern: gradientPattern
                            )
                        )
                    );
                }
            }
        }
        TerrainTransform[] gradients = fills.ToArray();
        Func<VoronoiResult, int> mapFunction = (result) => centers
            .Select((center, index) => new { center, index })
            .First(candidate => {
                return candidate.center.x == MathUtils.CanonicalModulus(result.closestSite.x + scale, scale * 3) - scale &&
                    candidate.center.y == MathUtils.CanonicalModulus(result.closestSite.z + scale, scale * 3) - scale;
            })
            .index;

        TerrainTransform noise = new Scalar(new Noise(new NoiseOptions(scale: 250, octaves: 7, persistance: .4f, seed: 19)), new ScalarOptions(5));

        return new Addition(noise, new Modulus(
            new VoronoiTesselation(
                new VoronoiTesselationOptions(
                    gradients,
                    voronoi,
                    mapFunction
                )
            ),
            new ModulusOptions(scale * 1f)
        ));
    }
}

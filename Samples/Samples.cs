using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Samples {
    public class SimpleVolcano : TerrainTransform {
        private readonly TerrainTransform transform;

        public SimpleVolcano(float scale) {
            TerrainTransform gradient = new RadialGradient(
                new RadialGradientOptions(
                    scale,
                    new Vector2(0f, 0f),
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
            //TerrainTransform curve = new Curve(noise, new CurveOptions(animationCurve));
            //TerrainTransform exaggeration = new Exaggeration(noise, new ExaggerationOptions(1.75f));
            //TerrainTransform exaggeration2 = new Exaggeration(noise, new ExaggerationOptions(.5f));
            transform = addition;
        }

        public override TerrainInformation GetTerrainInformation() {
            return transform.GetTerrainInformation();
        }

        protected override float Evaluate(Point point) {
            return transform.Process(point);
        }
    }


    public class VoronoiMountains : TerrainTransform {
        private readonly TerrainTransform transform;

        public VoronoiMountains() {
            int count = 10;
            Vector3 scale = new Vector3(100f, 100f, 100f);
            Voronoi voronoi = new Voronoi(
                new VoronoiOptions(count: count, scale: scale, seed: 19)
            );
            VoronoiRegion[] regions = voronoi.GetCanonicalRegions();
            Vector3[] sites = regions.Select(region => region.GetCenter()).ToArray();
            GradientPattern gradientPattern = new GradientPattern(
                new GradientPatternOptions(new[] {
                    new GradientPatternKey(0, 1),
                    new GradientPatternKey(1, 0)
                })
            );
            //float[] rands = new float[] {
            //    1, 2, 3, 4, 5, 6, 7, 8, 9, 0
            //};
            IEnumerable<TerrainTransform> fills = new List<TerrainTransform>();
            int index = 0;
            foreach (Vector3 site in sites) {
                fills = fills.Append(
                    new RadialGradient(
                        new RadialGradientOptions(
                            scale: 30f,
                            center: new Vector2(site.x, site.z),
                            gradientPattern: gradientPattern
                        )
                    )
                //new Constant(new ConstantOptions(rands[index]))
                );
                index++;
            }
            TerrainTransform[] gradients = fills.ToArray();
            //TerrainTransform noise = new Scalar(
            //    new Noise(
            //        new NoiseOptions(scale: 250, octaves: 7, persistance: .4f, seed: 19)
            //    ),
            //    new ScalarOptions(scalar: 5f)
            //);

            transform = new VoronoiTesselation(
                new VoronoiTesselationOptions(
                    fills: gradients,
                    voronoiModel: voronoi
                )
            );
        }

        public override TerrainInformation GetTerrainInformation() {
            return transform.GetTerrainInformation();
        }

        protected override float Evaluate(Point point) {
            return transform.Process(point);
        }
    }


    public class TectonicPlates : TerrainTransform {
        private readonly TerrainTransform transform;

        public TectonicPlates() {
            int count = 10;
            Vector3 scale = new Vector3(100f, 100f, 100f);
            Voronoi voronoi = new Voronoi(
                new VoronoiOptions(count: count, scale: scale, seed: 4)
            );
            VoronoiRegion[] regions = voronoi.GetCanonicalRegions();
            Vector3[] sites = regions.Select(region => region.GetCenter()).ToArray();
            GradientPattern gradientPattern = new GradientPattern(
                new GradientPatternOptions(new[] {
                    new GradientPatternKey(0, 1),
                    new GradientPatternKey(1, 0)
                })
            );
            float[] rotations = new float[] {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0
            };
            IEnumerable<TerrainTransform> fills = new List<TerrainTransform>();
            int index = 0;
            foreach (Vector3 site in sites) {
                fills = fills.Append(
                    new LinearGradient(
                        new LinearGradientOptions(
                            scale: 50f,
                            center: new Vector2(site.x, site.z),
                            rotation: rotations[index],
                            gradientPattern: gradientPattern
                        )
                    )
                );
                index++;
            }
            TerrainTransform[] gradients = fills.ToArray();
            transform = new Modulus(
                new VoronoiTesselation(
                    new VoronoiTesselationOptions(
                        gradients,
                        voronoi
                    )
                ),
                new ModulusOptions(scale * 1f)
            );
        }

        public override TerrainInformation GetTerrainInformation() {
            return transform.GetTerrainInformation();
        }

        protected override float Evaluate(Point point) {
            return transform.Process(point);
        }
    }
}

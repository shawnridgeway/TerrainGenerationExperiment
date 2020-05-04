using UnityEngine;

public class Samples {
    public class BrokenLands : TerrainGenerator {
        private readonly CoherentNoise.Generator _generator;

        public BrokenLands(
            AnimationCurve animationCurve
        ) {
            TerrainGenerator ridgeNoise = new RidgeNoise(seed: 23, frequency: 1 / 3000f, octaveCount: 11) * 10;
            TerrainGenerator cliffs = new Curve(new Gain(new PinkNoise(seed: 11, frequency: 1 / 800f, octaveCount: 5), .35f), animationCurve) * 3f;
            TerrainGenerator cliffs2 = new Curve(new PinkNoise(seed: 13, frequency: 1 / 1000f, octaveCount: 6), animationCurve) * 1.5f;
            TerrainGenerator terrain = (ridgeNoise + cliffs + cliffs2) * 25f;
            _generator = terrain.GetGenerator();
        }

        public override CoherentNoise.Generator GetGenerator() {
            return _generator;
        }

        /*
        m_Curve:
      - serializedVersion: 3
        time: 0
        value: -0.989504
        inSlope: 0.090682924
        outSlope: 0.090682924
        tangentMode: 0
        weightedMode: 2
        inWeight: 0
        outWeight: 0.2920854
      - serializedVersion: 3
        time: 0.40023
        value: -0.92734504
        inSlope: 1.0822589
        outSlope: 1.0822589
        tangentMode: 0
        weightedMode: 3
        inWeight: 0.2915386
        outWeight: 0.47119117
      - serializedVersion: 3
        time: 0.41493613
        value: -0.5771493
        inSlope: 9.339295
        outSlope: 9.339295
        tangentMode: 0
        weightedMode: 3
        inWeight: 0.16911885
        outWeight: 0.34702414
      - serializedVersion: 3
        time: 0.4851663
        value: 0.33266628
        inSlope: 1.5720689
        outSlope: 1.5720689
        tangentMode: 0
        weightedMode: 3
        inWeight: 0.36394837
        outWeight: 0.06572236
      - serializedVersion: 3
        time: 0.53681916
        value: 0.87448305
        inSlope: 0.334625
        outSlope: 0.334625
        tangentMode: 0
        weightedMode: 3
        inWeight: 0.66720116
        outWeight: 0.15810065
      - serializedVersion: 3
        time: 0.7915795
        value: 0.7843162
        inSlope: 0.28946114
        outSlope: 0.28946114
        tangentMode: 0
        weightedMode: 3
        inWeight: 0.44045857
        outWeight: 0.33551508
      - serializedVersion: 3
        time: 1
        value: 1
        inSlope: 0.0000003698277
        outSlope: 0.0000003698277
        tangentMode: 0
        weightedMode: 1
        inWeight: 0.30998546
        outWeight: 0
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
         */
    }

    //    public class SimpleVolcano : TerrainGenerator {
    //        private readonly TerrainGenerator transform;

    //        public SimpleVolcano(float scale) {
    //            TerrainGenerator gradient = new RadialGradient(
    //                new RadialGradientOptions(
    //                    scale,
    //                    new Vector2(0f, 0f),
    //                    new GradientPattern(
    //                        new GradientPatternOptions(
    //                            new GradientPatternKey[] {
    //                                    new GradientPatternKey(0f, 3f),
    //                                    new GradientPatternKey(.4f, 5f),
    //                                    new GradientPatternKey(.7f, 2f),
    //                                    new GradientPatternKey(1f, 0f)
    //                            },
    //                            GradientPatternMode.Clamp
    //                        )
    //                    )
    //                )
    //            );
    //            TerrainGenerator noise = new Noise(new NoiseOptions(scale: 250, octaves: 7, persistance: .4f, seed: 19));
    //            TerrainGenerator addition = new Add(
    //                gradient,
    //                new Scalar(noise, new ScalarOptions(2f))
    //            );
    //            //TerrainTransform curve = new Curve(noise, new CurveOptions(animationCurve));
    //            //TerrainTransform exaggeration = new Exaggeration(noise, new ExaggerationOptions(1.75f));
    //            //TerrainTransform exaggeration2 = new Exaggeration(noise, new ExaggerationOptions(.5f));
    //            transform = addition;
    //        }

    //        public override TerrainInformation GetTerrainInformation() {
    //            return transform.GetTerrainInformation();
    //        }

    //        protected override float Evaluate(Point point) {
    //            return transform.GetValue(point);
    //        }
    //    }


    //    public class VoronoiMountains : TerrainGenerator {
    //        private readonly TerrainGenerator transform;

    //        public VoronoiMountains() {
    //            int count = 10;
    //            Vector3 scale = new Vector3(100f, 100f, 100f);
    //            Voronoi voronoi = new Voronoi(
    //                new VoronoiOptions(count: count, scale: scale, seed: 19)
    //            );
    //            VoronoiRegion[] regions = voronoi.GetCanonicalRegions();
    //            Vector3[] sites = regions.Select(region => region.GetCenter()).ToArray();
    //            GradientPattern gradientPattern = new GradientPattern(
    //                new GradientPatternOptions(new[] {
    //                    new GradientPatternKey(0, 1),
    //                    new GradientPatternKey(1, 0)
    //                })
    //            );
    //            //float[] rands = new float[] {
    //            //    1, 2, 3, 4, 5, 6, 7, 8, 9, 0
    //            //};
    //            IEnumerable<TerrainGenerator> fills = new List<TerrainGenerator>();
    //            int index = 0;
    //            foreach (Vector3 site in sites) {
    //                fills = fills.Append(
    //                    new RadialGradient(
    //                        new RadialGradientOptions(
    //                            scale: 30f,
    //                            center: new Vector2(site.x, site.z),
    //                            gradientPattern: gradientPattern
    //                        )
    //                    )
    //                //new Constant(new ConstantOptions(rands[index]))
    //                );
    //                index++;
    //            }
    //            TerrainGenerator[] gradients = fills.ToArray();
    //            //TerrainTransform noise = new Scalar(
    //            //    new Noise(
    //            //        new NoiseOptions(scale: 250, octaves: 7, persistance: .4f, seed: 19)
    //            //    ),
    //            //    new ScalarOptions(scalar: 5f)
    //            //);

    //            transform = new VoronoiTesselation(
    //                new VoronoiTesselationOptions(
    //                    fills: gradients,
    //                    voronoiModel: voronoi
    //                )
    //            );
    //        }

    //        public override TerrainInformation GetTerrainInformation() {
    //            return transform.GetTerrainInformation();
    //        }

    //        protected override float Evaluate(Point point) {
    //            return transform.GetValue(point);
    //        }
    //    }


    //    public class TectonicPlates : TerrainGenerator {
    //        private readonly TerrainGenerator transform;

    //        public TectonicPlates() {
    //            int count = 10;
    //            Vector3 scale = new Vector3(100f, 100f, 100f);
    //            Voronoi voronoi = new Voronoi(
    //                new VoronoiOptions(count: count, scale: scale, seed: 4)
    //            );
    //            VoronoiRegion[] regions = voronoi.GetCanonicalRegions();
    //            Vector3[] sites = regions.Select(region => region.GetCenter()).ToArray();
    //            GradientPattern gradientPattern = new GradientPattern(
    //                new GradientPatternOptions(new[] {
    //                    new GradientPatternKey(0, 1),
    //                    new GradientPatternKey(1, 0)
    //                })
    //            );
    //            float[] rotations = new float[] {
    //                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0
    //            };
    //            IEnumerable<TerrainGenerator> fills = new List<TerrainGenerator>();
    //            int index = 0;
    //            foreach (Vector3 site in sites) {
    //                fills = fills.Append(
    //                    new LinearGradient(
    //                        new LinearGradientOptions(
    //                            scale: 50f,
    //                            center: new Vector2(site.x, site.z),
    //                            rotation: rotations[index],
    //                            gradientPattern: gradientPattern
    //                        )
    //                    )
    //                );
    //                index++;
    //            }
    //            TerrainGenerator[] gradients = fills.ToArray();
    //            transform = new Modulus(
    //                new VoronoiTesselation(
    //                    new VoronoiTesselationOptions(
    //                        gradients,
    //                        voronoi
    //                    )
    //                ),
    //                new ModulusOptions(scale * 1f)
    //            );
    //        }

    //        public override TerrainInformation GetTerrainInformation() {
    //            return transform.GetTerrainInformation();
    //        }

    //        protected override float Evaluate(Point point) {
    //            return transform.GetValue(point);
    //        }
    //    }
}

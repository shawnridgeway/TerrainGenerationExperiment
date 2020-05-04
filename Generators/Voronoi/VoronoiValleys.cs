using UnityEngine;

public class VoronoiValleys : TerrainGenerator {
    private readonly CoherentNoise.Generator _generator;

    public VoronoiValleys(
        int seed = 0,
        int period = 0,
        float frequency = 1 / 240f,
        bool use2D = false
    ) {
        _generator = ChooseGenerator(seed, period, frequency, use2D);
    }

    private CoherentNoise.Generator ChooseGenerator(int seed, int period, float frequency, bool use2D) {
        if (use2D) {
            return new SwapYZGenerator(new CoherentNoise.Generation.Voronoi.VoronoiValleys2D(seed) {
                Frequency = frequency,
                Period = period
            });
        }
        return new CoherentNoise.Generation.Voronoi.VoronoiValleys(seed) {
            Frequency = frequency,
            Period = period
        };

    }

    public override CoherentNoise.Generator GetGenerator() {
        return _generator;
    }
}


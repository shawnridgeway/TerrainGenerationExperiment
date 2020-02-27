using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Noise : TerrainTransform {
	private readonly NoiseOptions options;
	private readonly Vector3[] octaveOffsets;
    private readonly float maxPossibleValue;

    public Noise(NoiseOptions options) {
		this.options = options;
        System.Random prng = new System.Random(options.seed);
        float amplitude = 1;
        maxPossibleValue = 0;
        octaveOffsets = new Vector3[options.octaves];
        for (int i = 0; i < options.octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + options.offset.x;
            float offsetZ = prng.Next(-100000, 100000) - options.offset.z;
            octaveOffsets[i] = new Vector3(offsetX, 0, offsetZ);
            maxPossibleValue += amplitude;
            amplitude *= options.persistance;
        }
    }

    protected override float Evaluate(Point point) {
		float value = 0;
        float amplitude = 1;
        float frequency = 1;
        for (int i = 0; i < options.octaves; i++) {
            float sampleX = (point.GetLocation().x + octaveOffsets[i].x) / options.scale * frequency;
            float sampleZ = (point.GetLocation().z + octaveOffsets[i].z) / options.scale * frequency;
            float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
            value += perlinValue * amplitude;
            amplitude *= options.persistance;
            frequency *= options.lacunarity;
        }
        return value;
    }

    public override TerrainInformation GetTerrainInformation() {
        return new TerrainInformation(-maxPossibleValue, maxPossibleValue);
    }
}

public class NoiseOptions {
	public readonly float scale; //[Min(0.0001f)]
	public readonly int octaves; //[Min(0)]
	public readonly float persistance; //[Range(0, 1)] 
	public readonly float lacunarity; //[Min(1)]
	public readonly Vector3 offset;
	public readonly int seed;

	public NoiseOptions(
        float scale = 50,
        int octaves = 6,
        float persistance = 0.5f,
        float lacunarity = 2f,
        Vector3 offset = new Vector3(),
        int seed = 0
    ) {
		this.scale = scale;
		this.octaves = octaves;
		this.persistance = persistance;
		this.lacunarity = lacunarity;
        this.offset = offset;
        this.seed = seed;
	}
}

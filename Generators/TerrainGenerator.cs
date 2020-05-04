using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainGenerator {

    public abstract CoherentNoise.Generator GetGenerator();

    public float GetValue(Point point) {
        CoherentNoise.Generator gen = GetGenerator();
        return gen.GetValue(point.GetPosition());
    }

    //public IEnumerable<float> GetValues(IEnumerable<Point> points) {
    //    // Optimization, some getters can be slow, so cache it here
    //    CoherentNoise.Generator gen = GetGenerator();
    //    foreach (Point point in points) {
    //        yield return gen.GetValue(point.GetPosition());
    //    }
    //}

    public static TerrainGenerator operator +(TerrainGenerator source1, TerrainGenerator source2) {
        return new Addition(source1, source2);
    }

    public static TerrainGenerator operator +(TerrainGenerator source1, float value) {
        return new Addition(source1, value);
    }

    public static TerrainGenerator operator -(TerrainGenerator source1, TerrainGenerator source2) {
        return new Addition(source1, source2 * -1);
    }

    public static TerrainGenerator operator -(TerrainGenerator source1, float value) {
        return new Addition(source1, value * -1);
    }

    public static TerrainGenerator operator -(float value, TerrainGenerator source1) {
        return new Addition(value, source1 * -1);
    }

    public static TerrainGenerator operator *(TerrainGenerator source1, TerrainGenerator source2) {
        return new Multiplication(source1, source2);
    }

    public static TerrainGenerator operator *(TerrainGenerator source1, float value) {
        return new Multiplication(source1, value);
    }

    public static TerrainGenerator operator /(TerrainGenerator source1, TerrainGenerator source2) {
        return new Multiplication(source1, 1 / source2);
    }

    public static TerrainGenerator operator /(TerrainGenerator source1, float value) {
        return new Multiplication(source1, 1 / value);
    }

    public static TerrainGenerator operator /(float value, TerrainGenerator source1) {
        return new Multiplication(value, 1 / source1);
    }

    public static implicit operator TerrainGenerator(float value) {
        return new Constant(value);
    }
}
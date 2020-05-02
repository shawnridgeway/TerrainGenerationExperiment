using UnityEngine;
using System;
using System.Collections.Concurrent;
using System.Threading;

class ThreadSafeGenerator : CoherentNoise.Generator {
    private readonly ConcurrentDictionary<int, CoherentNoise.Generator> generatorsByThread =
        new ConcurrentDictionary<int, CoherentNoise.Generator>();

    private readonly Func<CoherentNoise.Generator> createGenerator;

    public ThreadSafeGenerator(
        Func<CoherentNoise.Generator> createGenerator
    ) {
        this.createGenerator = createGenerator;
    }

    public override float GetValue(float x, float y, float z) {
        return GetGenerator().GetValue(x, y, z);
    }

    private CoherentNoise.Generator GetGenerator() {
        // Get a separate clone per thread so there are no concurrency issues
        int currentThreadHash = Thread.CurrentThread.GetHashCode();
        CoherentNoise.Generator generator;
        bool curveExists = generatorsByThread.TryGetValue(currentThreadHash, out generator);
        if (!curveExists) {
            generator = createGenerator();
            generatorsByThread.TryAdd(currentThreadHash, generator);
        }
        return generator;
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;

////
//// |-- 3 --|-- 2 --|---- 1 ----|-- 2 --|-- 3 --|
//// 
//// 1: Points needing processing (values returned)
//// 2: Points included which may effect 1s (values in start positions)
//// 3: Points included which may be effected by 2s (values fed to compute)
////

//public class LocalErosion : TerrainGenerator {
//    private readonly TerrainGenerator a;
//    private readonly LocalErosionOptions options;

//    private readonly List<float> brushWeights;

//    public LocalErosion(TerrainGenerator a, LocalErosionOptions options) {
//        this.a = a;
//        this.options = options;

//        // Create brush
//        brushWeights = new List<float>();
//        int erosionBrushRadius = options.erosionRadius;
//        float weightSum = 0;
//        for (int brushY = -erosionBrushRadius; brushY <= erosionBrushRadius; brushY++) {
//            for (int brushX = -erosionBrushRadius; brushX <= erosionBrushRadius; brushX++) {
//                float sqrDst = brushX * brushX + brushY * brushY;
//                if (sqrDst <= erosionBrushRadius * erosionBrushRadius) {
//                    float brushWeight = 1 - Mathf.Sqrt(sqrDst) / erosionBrushRadius;
//                    weightSum += brushWeight;
//                    brushWeights.Add(brushWeight);
//                }
//            }
//        }
//        for (int i = 0; i < brushWeights.Count; i++) {
//            brushWeights[i] /= weightSum;
//        }
//    }

//    protected override float Evaluate(Point point) {
//        // This method is not used
//        throw new System.NotImplementedException();
//    }

//    public override IEnumerable<float> GetValues(IEnumerable<Point> points) {
//        return EvaluateSet(points);
//    }

//    public override TerrainInformation GetTerrainInformation() {
//        return a.GetTerrainInformation();
//    }

//    IEnumerable<float> EvaluateSet(IEnumerable<Point> points) {
//        // Check cache?

//        // Get all points necessary for errosion, including border points
//        int borderSize = options.maxDropletLifetime + options.erosionRadius;

//        // Keep track of which index points are in
//        Dictionary<Point, int> pointIndexPairs = new Dictionary<Point, int>();
//        List<Point> borderedPoints = new List<Point>();

//        // Points in class 1
//        IncludePoints(pointIndexPairs, borderedPoints, points);
//        int countOfPointsToReturn = borderedPoints.Count();

//        // Points in class 2
//        ExpandPoints(pointIndexPairs, borderedPoints, borderSize);
//        int countOfPointsToInclude = borderedPoints.Count();

//        // Points in class 3
//        ExpandPoints(pointIndexPairs, borderedPoints, borderSize);
//        int countOfPointsAffected = borderedPoints.Count();

//        // Get neighbors array
//        int cardinality = 4;
//        int[] neighborsArray = new int[countOfPointsAffected * cardinality];
//        int index = 0;
//        foreach (Point borderedPoint in borderedPoints) {
//            foreach (Point neighbor in borderedPoint.GetNeighbors(1)) {
//                bool hasKey = pointIndexPairs.TryGetValue(neighbor, out int pointIndex);
//                neighborsArray[index++] = hasKey ? pointIndex : -1; // Use index of -1 for out of bounds
//            }
//        }

//        // Height Map
//        IEnumerable<float> previousValues = a.GetValues(borderedPoints);

//        // Generate random drop sites
//        int[] randomIndices = new int[countOfPointsToInclude];
//        for (int i = 0; i < randomIndices.Length; i++) {
//            randomIndices[i] = Random.Range(0, countOfPointsToInclude);
//        }

//        // Feed into compute
//        ComputeShader erosionCompute = options.erosionCompute;
//        int numThreads = Mathf.Max(countOfPointsAffected / 512, 1);

//        try {
//            float[] valuesArray = previousValues.ToArray();
//            ComputeBuffer mapBuffer = new ComputeBuffer(valuesArray.Length, sizeof(float));
//            mapBuffer.SetData(valuesArray);
//            erosionCompute.SetBuffer(0, "map", mapBuffer);

//            ComputeBuffer neighborsBuffer = new ComputeBuffer(neighborsArray.Length, sizeof(int));
//            neighborsBuffer.SetData(neighborsArray);
//            erosionCompute.SetBuffer(0, "neighbors", neighborsBuffer);

//            ComputeBuffer randomIndexBuffer = new ComputeBuffer(randomIndices.Length, sizeof(int));
//            randomIndexBuffer.SetData(randomIndices);
//            erosionCompute.SetBuffer(0, "randomIndices", randomIndexBuffer);

//            ComputeBuffer brushWeightBuffer = new ComputeBuffer(brushWeights.Count, sizeof(float));
//            brushWeightBuffer.SetData(brushWeights);
//            erosionCompute.SetBuffer(0, "brushWeights", brushWeightBuffer);


//            erosionCompute.SetInt("erosionRadius", options.erosionRadius);
//            erosionCompute.SetInt("maxLifetime", options.maxDropletLifetime);
//            erosionCompute.SetFloat("inertia", options.inertia);
//            erosionCompute.SetFloat("sedimentCapacityFactor", options.maxSedimentCapacity);
//            erosionCompute.SetFloat("minSedimentCapacity", options.minSedimentCapacity);
//            erosionCompute.SetFloat("depositSpeed", options.depositSpeed);
//            erosionCompute.SetFloat("erodeSpeed", options.erosionSpeed);
//            erosionCompute.SetFloat("evaporateSpeed", options.evaporateSpeed);
//            erosionCompute.SetFloat("gravity", options.gravity);
//            erosionCompute.SetFloat("startSpeed", options.initialSpeed);
//            erosionCompute.SetFloat("startWater", options.initialWaterVolume);

//            // Run compute shader
//            erosionCompute.Dispatch(0, numThreads, 1, 1);

//            // Get results
//            mapBuffer.GetData(valuesArray);

//            // Release buffers
//            mapBuffer.Release();
//            neighborsBuffer.Release();
//            randomIndexBuffer.Release();
//            brushWeightBuffer.Release();

//            // Update cache?

//            // Return result
//            return valuesArray;
//        } catch (System.Exception error) {
//            return new float[0];
//        }
//    }

//    void ExpandPoints(Dictionary<Point, int> pointIndexPairs, List<Point> points, int borderSize) {
//        int initialCount = points.Count;
//        for (int i = 0; i < initialCount; i++) {
//            Point point = points.ElementAt(i);
//            // Optimization, dont get border points if all neighbors are in set
//            IEnumerable<Point> neighbors = point.GetNeighbors(1);
//            bool allNeighborsInSet = true;
//            foreach (Point neighbor in neighbors) {
//                allNeighborsInSet &= pointIndexPairs.ContainsKey(neighbor);
//            }
//            if (!allNeighborsInSet) {
//                IEnumerable<Point> borderPoints = point.GetBorderPoints(borderSize, 1);
//                IncludePoints(pointIndexPairs, points, borderPoints);
//            }
//        }
//    }

//    void IncludePoints(Dictionary<Point, int> pointIndexPairs, List<Point> points, IEnumerable<Point> pointsToAdd) {
//        foreach (Point pointToAdd in pointsToAdd) {
//            if (!pointIndexPairs.ContainsKey(pointToAdd)) {
//                pointIndexPairs.Add(pointToAdd, points.Count);
//                points.Add(pointToAdd);
//            }
//        }
//    }
//}

//public class LocalErosionOptions {
//    public readonly ComputeShader erosionCompute;
//    public readonly int erosionRadius; // 2 - 8
//    public readonly float minSedimentCapacity;
//    public readonly float maxSedimentCapacity;
//    public readonly float erosionSpeed;
//    public readonly float depositSpeed;
//    public readonly float initialWaterVolume;
//    public readonly int maxDropletLifetime;
//    public readonly float evaporateSpeed;
//    public readonly float initialSpeed;
//    public readonly float inertia;
//    public readonly float gravity;
//    public readonly int iterations;
//    public readonly int seed;

//    public LocalErosionOptions(
//        int erosionRadius = 3,
//        float minSedimentCapacity = .01f,
//        float maxSedimentCapacity = 4,
//        float erosionSpeed = .3f,
//        float depositSpeed = .3f,
//        float initialWaterVolume = 1,
//        int maxDropletLifetime = 30,
//        float evaporateSpeed = .01f,
//        float initialSpeed = 1,
//        float inertia = .05f,
//        float gravity = 4,
//        int iterations = 50000,
//        int seed = 0
//    ) {
//        erosionCompute = (ComputeShader)Resources.Load("Erosion");
//        this.erosionRadius = erosionRadius;
//        this.minSedimentCapacity = minSedimentCapacity;
//        this.maxSedimentCapacity = maxSedimentCapacity;
//        this.erosionSpeed = erosionSpeed;
//        this.depositSpeed = depositSpeed;
//        this.initialWaterVolume = initialWaterVolume;
//        this.maxDropletLifetime = maxDropletLifetime;
//        this.evaporateSpeed = evaporateSpeed;
//        this.initialSpeed = initialSpeed;
//        this.inertia = inertia;
//        this.gravity = gravity;
//        this.iterations = iterations;
//        this.seed = seed;
//    }
//}

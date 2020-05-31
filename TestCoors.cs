using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoors : MonoBehaviour
{
    List<Vector3> coors;

    // Start is called before the first frame update
    void Start()
    {
        //List<float> points = new List<float>();
        //for (float i = -180; i <= 180; i += 10) {
        //    points.Add(i);
        //}

        //coors = new List<Vector3>();
        //foreach (float i in points) {
        //    foreach (float j in points) {
        //        foreach (float k in points) {
        //            coors.Add(new Vector3(i, j, k));
        //        }
        //    }
        //}
        //foreach (float i in points) {
        //    coors.Add(new Vector3(i, 0, 0));
        //}
        //foreach (float i in points) {
        //    coors.Add(new Vector3(0, i, 0));
        //}
        //foreach (float i in points) {
        //    coors.Add(new Vector3(0, 0, i));
        //}
        //Test();

    }

    // Best yet:
    //new Vector3(
    //            x: Mathf.Cos(coor.y) * Mathf.Sin(coor.z),
    //            y: Mathf.Sin(coor.x) * Mathf.Cos(coor.z),
    //            z: Mathf.Cos(coor.x) * Mathf.Sin(coor.y)

        // Maybe even better?
    // new Vector3(
    //        x: Mathf.Cos(Mathf.PI / 4 - coor.y) * Mathf.Sin(Mathf.PI / 4 - coor.z), 
    //        y: Mathf.Sin(Mathf.PI / 4 - coor.x) * Mathf.Cos(Mathf.PI / 4 - coor.z),
    //        z: Mathf.Cos(Mathf.PI / 4 - coor.x) * Mathf.Sin(Mathf.PI / 4 - coor.y)
    //    )


    //        )

    //public Vector3 GetPointFromCoordinates(Vector3 coor) {
    //    //return new Vector3(
    //    //    x: Mathf.Cos(Mathf.PI / 4 - coor.y) * Mathf.Sin(Mathf.PI / 4 - coor.z),
    //    //    y: Mathf.Sin(Mathf.PI / 4 - coor.x) * Mathf.Cos(Mathf.PI / 4 - coor.z),
    //    //    z: Mathf.Cos(Mathf.PI / 4 - coor.x) * Mathf.Sin(Mathf.PI / 4 - coor.y)
    //    //).normalized * 10;

    //    //    scale* Mathf.Cos(coordinate.x) * Mathf.Cos(coordinate.y),
    //    //    scale* Mathf.Cos(coordinate.x) * Mathf.Sin(coordinate.y),
    //    //    scale* Mathf.Sin(coordinate.x)

    //    // z - yaw,
    //    //return Quaternion.LookRotation(coor.normalized);
    //    //return Quaternion.Euler(coor) * Vector3.forward; // Note, uses latitude style coors, not longitude

    //    //return new Vector3(
    //    //    x: Mathf.Cos(coor.y) * Mathf.Cos(coor.z), // Sin here
    //    //    y: Mathf.Cos(coor.x) * Mathf.Sin(coor.z),
    //    //    z: Mathf.Cos(coor.x) * Mathf.Sin(coor.y)
    //    //).normalized * 10;
    //}

    // Update is called once per frame
    void Update()
    {
        //foreach (Vector3 coor in coors) {
        //    Vector3 pos = GetPointFromCoordinates(coor);
        //    if (true) {
        //        Debug.DrawLine(Vector3.zero, pos);
        //    }

        //}
        //Vector3 target = new Vector3(-37, 35, 112) * -1;
        ////Debug.DrawLine(Vector3.zero, target, Color.green);
        //SphericalSpace space = new SphericalSpace();
        //SphericalPoint closestPoint = (SphericalPoint)space.GetPointFromPosition(target);
        //Debug.Log(space.GetChunkCount(240, 0)); // 3
        //Debug.Log(space.GetChunkCount(120, 0)); // 6
        //Debug.Log(space.GetChunkCount(60, 0)); // 15
        //Debug.Log(space.GetChunkCount(240, 1)); // 15

        ////Debug.Log("closestPoint");
        ////Debug.Log(closestPoint.GetCoordinate());
        ////Debug.Log(closestPoint.GetPosition());
        ////Debug.Log(space.GetCanonicalCoordinates(new Vector2(Mathf.PI / 2, Mathf.PI)));

        //Chunk chunk = space.GetClosestChunkTo(closestPoint);
        //if (chunk is SphericalChunk sChunk) {

        //    Debug.Log("Chunk Center");
        //    Debug.Log(sChunk.GetCenterCoordinate());

        //    foreach (SphericalPoint point in chunk.GetPoints(60, 0)) {
        //        Debug.Log(point.GetCoordinate());
        //        Debug.DrawLine(Vector3.zero, point.GetPosition(), Color.red);
        //    }

        //}
        //for (int i = -6; i < 6; i++) {
        //    for (int j = -3; j <= 3; j++) {
        //        Debug.DrawLine(Vector3.zero, space.GetPositionFromCoordinate(new Vector2(j * Mathf.PI / 6, i * Mathf.PI / 6)));
        //    }
        //}
    }

    void Test() {
        //SphericalSpace space = new SphericalSpace();
        //SphericalPoint closestPoint = (SphericalPoint)space.GetPointFromPosition(Vector3.one);

        //Debug.Log("Expected: (0.0, 0.0)");
        //Debug.Log(space.GetCanonicalCoordinates((new SphericalPoint(new Vector2(0, 0), space)).GetCoordinate()));

        //Debug.Log("Expected: (1.6, 0.0)");
        //Debug.Log(space.GetCanonicalCoordinates((new SphericalPoint(new Vector2(Mathf.PI / 2, 0), space)).GetCoordinate()));

        //Debug.Log("Expected: (-1.6, 0.0)");
        //Debug.Log(space.GetCanonicalCoordinates((new SphericalPoint(new Vector2(-Mathf.PI / 2, 0), space)).GetCoordinate()));

        //Debug.Log("Expected: (0.0, -3.1)");
        //Debug.Log(space.GetCanonicalCoordinates((new SphericalPoint(new Vector2(Mathf.PI, 0), space)).GetCoordinate()));
    }


}

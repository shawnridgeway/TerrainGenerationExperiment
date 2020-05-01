using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoors : MonoBehaviour
{
    List<Vector3> coors;

    // Start is called before the first frame update
    void Start()
    {
        List<float> points = new List<float>();
        for (float i = -180; i <= 180; i += 10) {
            points.Add(i);
        }

        coors = new List<Vector3>();
        foreach (float i in points) {
            foreach (float j in points) {
                foreach (float k in points) {
                    coors.Add(new Vector3(i, j, k));
                }
            }
        }
        //foreach (float i in points) {
        //    coors.Add(new Vector3(i, 0, 0));
        //}
        //foreach (float i in points) {
        //    coors.Add(new Vector3(0, i, 0));
        //}
        //foreach (float i in points) {
        //    coors.Add(new Vector3(0, 0, i));
        //}


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

    public Vector3 GetPointFromCoordinates(Vector3 coor) {
        //return new Vector3(
        //    x: Mathf.Cos(Mathf.PI / 4 - coor.y) * Mathf.Sin(Mathf.PI / 4 - coor.z),
        //    y: Mathf.Sin(Mathf.PI / 4 - coor.x) * Mathf.Cos(Mathf.PI / 4 - coor.z),
        //    z: Mathf.Cos(Mathf.PI / 4 - coor.x) * Mathf.Sin(Mathf.PI / 4 - coor.y)
        //).normalized * 10;

        //    scale* Mathf.Cos(coordinate.x) * Mathf.Cos(coordinate.y),
        //    scale* Mathf.Cos(coordinate.x) * Mathf.Sin(coordinate.y),
        //    scale* Mathf.Sin(coordinate.x)

        // z - yaw,
        //return Quaternion.LookRotation(coor.normalized);
        return Quaternion.Euler(coor) * Vector3.forward; // Note, uses latitude style coors, not longitude

        //return new Vector3(
        //    x: Mathf.Cos(coor.y) * Mathf.Cos(coor.z), // Sin here
        //    y: Mathf.Cos(coor.x) * Mathf.Sin(coor.z),
        //    z: Mathf.Cos(coor.x) * Mathf.Sin(coor.y)
        //).normalized * 10;
    }

    // Update is called once per frame
    void Update()
    {
        //foreach (Vector3 coor in coors) {
        //    Vector3 pos = GetPointFromCoordinates(coor);
        //    if (true) {
        //        Debug.DrawLine(Vector3.zero, pos);
        //    }

        //}
    }


}

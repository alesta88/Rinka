using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

    public Material mat;

    float width = 1;
    float height = 1;

    public GameObject newmesh;

    // Use this for initialization
    void Start()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[3];

        vertices[0] = new Vector3(newmesh.transform.GetChild(0).position.x, newmesh.transform.GetChild(0).position.y);
        vertices[1] = new Vector3(newmesh.transform.GetChild(1).position.x, newmesh.transform.GetChild(1).position.y);
        vertices[2] = new Vector3(newmesh.transform.GetChild(2).position.x, newmesh.transform.GetChild(2).position.y);
        //  vertices[3] = new Vector3(width, -height);

        mesh.vertices = vertices;

        mesh.triangles = new int[] { 0, 1, 2 };

        GetComponent<MeshRenderer>().material = mat;

        GetComponent<MeshFilter>().mesh = mesh;

        Debug.Log(newmesh.transform.GetChild(0).position.x);
    }
}
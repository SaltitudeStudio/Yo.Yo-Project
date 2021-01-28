using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_BakeMesh_Len : MonoBehaviour
{

    public LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh, true);
        meshCollider.sharedMesh = mesh;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_YoyoRope_Len : MonoBehaviour
{

    [SerializeField]
    List<GameObject> ropeSegments;

    [SerializeField]
    GameObject firstSegment;
    [SerializeField]
    GameObject lastSegment;

    // Start is called before the first frame update
    void Start()
    {
        ropeSegments.Add(firstSegment);
        ropeSegments.Add(lastSegment);
    }

    // Update is called once per frame
    void Update()
    {
        //              /!\

        // J'ai ajout� un line render vite fait, mais j'ai surtout test� le distance joint qui reste assez elastique
        // Du coup il y a de l'�nergie qui ne se transmet pas et c'est g�nant!
        //  le spring joint �tait prometteur au niveau du r�alisme, mais l'elasticit� est difficile � controler, faudrait plus de test. 
        // Le hinge joint me semble �tre le plus prometteur en l'�tat, il a un comportement assez correct et s'ajuste plus ou moins bien, � voir.
        // ====> du coup j'ai test� une version alternative avec des hinge joints h�site pas � tester pour mixer => par contre si ca va trop vite, �a fucked up
        //j'ai reverse 98% de mes changements sauf 2-3 ptis trucs pour te faire gagner du temps
        //Cy

        //PS => J'ai boost� la physique comme un gros porc, que �a soit niveau des it�ration autant qu'au niveau du fixed update qui s'effectue 10X plus vite. 
        //Full d�j� vue. C'est ptet trop mais juste sinon CA NE MARCHAIT PAS. tout simplement. Genre ta ficelle elle partait en live d�s qu'on la d�pla�ait.

        UpdateLineRenderer();
    }

    public void AddRopeSegment()
    {

        Debug.Log("AddRopeSegment");

        //
        GameObject _newSegment = new GameObject("Segment_" + ropeSegments.Count.ToString());
        //spawn a une position plus cool pour la physique
        _newSegment.transform.position = lastSegment.transform.position;
        _newSegment.transform.parent = this.transform;


        for (int i = 0; i < ropeSegments.Count; i++)
        {
            if (ropeSegments[i] == lastSegment)
            {
                ropeSegments.Insert(i, _newSegment);
                break;
            }
        }

        Rigidbody2D _curSegmentRb = _newSegment.AddComponent<Rigidbody2D>();
        //piti poids pour moins de soucis
        _curSegmentRb.mass = 0.1f;


        DistanceJoint2D _curSegmentJoint = _newSegment.AddComponent<DistanceJoint2D>();

        _curSegmentJoint.autoConfigureDistance = false;
        _curSegmentJoint.distance = 0.1f;

        for (int i = 0; i < ropeSegments.Count; i++)
            if (i > 0)
                ropeSegments[i].GetComponent<DistanceJoint2D>().connectedBody = ropeSegments[i - 1].GetComponent<Rigidbody2D>();

    }

    public void RemoveRopeSegment()
    {
        Debug.Log("RemoveRopeSegment");
    }


    public void UpdateLineRenderer()
    {
        LineRenderer lineRender = GetComponent<LineRenderer>();

        lineRender.positionCount = ropeSegments.Count;

        for (int i = 0; i < ropeSegments.Count; i++)
        {
            lineRender.SetPosition(i, ropeSegments[i].transform.position);
        }

    }

}

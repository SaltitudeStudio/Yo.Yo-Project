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

        // J'ai ajout� un line render vite fait, mais j'ai surtout test� le distance joint qui fait nimp du coup, j'ai l'impression qu'il transmet pas la force dans tous
        // les cas o� on d�place la position autrement qu'en physique => ^super g�nant, le spring joint �tait prometteur au niveau du r�alisme, mais l'elasticit� est trop dure � controler. 
        // Le hinge joint me semble �tre le plus prometteur en l'�tat, il a un comportement assez correct et s'ajuste plus ou moins bien, � voir.
        //Cy

        UpdateLineRenderer();
    }

    public void AddRopeSegment()
    {

        Debug.Log("AddRopeSegment");

        //
        GameObject _newSegment = new GameObject("Segment_" + ropeSegments.Count.ToString());
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

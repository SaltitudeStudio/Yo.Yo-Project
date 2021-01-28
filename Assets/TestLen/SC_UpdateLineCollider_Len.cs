using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_UpdateLineCollider_Len : MonoBehaviour
{

    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject yoyo;

    LineRenderer lr;
    EdgeCollider2D ec;
    List<Vector2> ropePoints;

    void Start()
    {
        lr = this.GetComponent<LineRenderer>();
        ec = this.GetComponent<EdgeCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        //UpdateEc();

    }

    void UpdateLr()
    {

    }

    void UpdateEc()
    {

        ropePoints.Clear();
        ropePoints.Add(player.transform.position);
        ropePoints.Add(yoyo.transform.position);

        ec.SetPoints(ropePoints);

        lr.positionCount = ropePoints.Count;

        for (int i = 0; i < ropePoints.Count; i++)
        {
            Vector3 curPos = new Vector3(ropePoints[i].x, 0, ropePoints[i].y);
            lr.SetPosition(i, curPos);
        }

    }

    void ApplyLrToEc()
    {

        Vector3[] lrPos = new Vector3[lr.positionCount];
        lr.GetPositions(lrPos);

        //Vector2 lrConvertPos;

        //ec.SetPoints();

    }




}

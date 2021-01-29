using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CySegment : MonoBehaviour
{

    public GameObject BeforeIt;
    public GameObject AfterIt;


    // Start is called before the first frame update
    void Start()
    {
        BeforeIt = GetComponent<HingeJoint2D>().connectedBody.gameObject;

        CySegment BeforeSeg = BeforeIt.GetComponent<CySegment>();

        if(BeforeSeg != null)
        {
            BeforeSeg.AfterIt = gameObject;
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, -1f);
        }
        else
        {
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

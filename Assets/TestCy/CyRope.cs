using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyRope : MonoBehaviour
{

    /// <summary>
    /// LES HINGE JOINTS CASSENT D'UN RIEN!
    /// </summary>



    [SerializeField]
    GameObject PrefabSeg;

    Rigidbody2D ropeAnchor;

    [SerializeField]
    GameObject Yoyo;

    [SerializeField]
    int segNumb = 10;

    // Start is called before the first frame update
    void Start()
    {

        ropeAnchor = GetComponent<Rigidbody2D>();
        Generate();
    }

    public void Generate()
    {
        Rigidbody2D previous = ropeAnchor;

        for (int i = 0; i < segNumb; i++)
        {
            GameObject newSeg = Instantiate(PrefabSeg);
            newSeg.transform.position = transform.position;
            HingeJoint2D hingeJoint = newSeg.GetComponent<HingeJoint2D>();
            hingeJoint.connectedBody = previous;

            previous = newSeg.GetComponent<Rigidbody2D>();

            //quand on arrive au dernier
            if(i == segNumb-1)
            {
                HingeJoint2D yoyoHinge = Yoyo.GetComponent<HingeJoint2D>();
                yoyoHinge.connectedBody = previous;
                previous.gameObject.GetComponent<CySegment>().AfterIt = Yoyo;
                yoyoHinge.connectedAnchor = new Vector2(0, -1f);
            }
        }

        


    }





        // Update is called once per frame
    void Update()
    {
        
    }
}

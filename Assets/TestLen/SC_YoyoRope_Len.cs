using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_YoyoRope_Len : MonoBehaviour
{

    [Header("Rope Parameters")]
    [SerializeField, Range(2, 100)]
    int maxRopeLength = 50;
    private int minRopeLength = 2;
    [SerializeField]
    float ropeElasticityLength = 0.1f;
    [SerializeField]
    float ropeSegmentLength = 0.1f;
    [SerializeField]
    float ropeSegmentMass = 0.1f;
    [SerializeField]
    float ropeSegmentRadius = 0.04f;

    [Header("Rope References")]
    [SerializeField]
    GameObject firstSegment;
    [SerializeField]
    GameObject lastSegment;

    [Header("Debug DO NOT TOUCH")]
    [SerializeField]
    List<GameObject> ropeSegments;

    // Start is called before the first frame update
    void Start()
    {
        ropeSegments.Add(firstSegment);
        ropeSegments.Add(lastSegment);
        //UpdateGlobaDist();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLineRenderer();
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

    #region Rope Enlargement Functions

    public bool IsRopeAtMaxLength()
    {
        if (ropeSegments.Count < maxRopeLength)
        {
            return false;
        }
        else
        {
            Debug.Log("MaxLength");
            return true;
        }
    }

    public bool canAddNewSegment()
    {

        Vector2 distBetween2LastSegment = ropeSegments[ropeSegments.Count - 2].transform.position - lastSegment.transform.position;

        if (distBetween2LastSegment.magnitude > ropeSegmentLength * 0.75f)
            return true;
        else
            return false;

    }

    public void AddRopeSegment()
    {

        //Debug.Log("AddRopeSegment");

        if (ropeSegments.Count < maxRopeLength)
        {

            //Creation du segment de rope
            GameObject _newSegment = new GameObject("Segment_" + ropeSegments.Count.ToString());
            //spawn a une position plus cool pour la physique
            _newSegment.transform.position = lastSegment.transform.position;
            _newSegment.transform.parent = this.transform;
            _newSegment.layer = 9;

            //Insertion du segment  dans la Liste a l'avant derniere position (derniere pos = Last Segment)
            for (int i = 0; i < ropeSegments.Count; i++)
            {
                if (ropeSegments[i] == lastSegment)
                {
                    ropeSegments.Insert(i, _newSegment);
                    break;
                }
            }

            //Physics
            Rigidbody2D _curSegmentRb = _newSegment.AddComponent<Rigidbody2D>();
            _curSegmentRb.mass = ropeSegmentMass;

            //Collisions
            CircleCollider2D _curSegmentCol = _newSegment.AddComponent<CircleCollider2D>();
            _curSegmentCol.radius = ropeSegmentRadius;

            // How To Switch Dist/Hinge : 
            // Garde que le paragraphe voulu ci-dessous
            // Remplacer le type de connected body ci-ci-dessous
            // Activer le component correspondant sur LastSegment et desactiver l'autre

            //DistantJoint2D
            DistanceJoint2D _curSegmentJoint = _newSegment.AddComponent<DistanceJoint2D>();
            _curSegmentJoint.autoConfigureDistance = false;
            _curSegmentJoint.distance = ropeSegmentLength;
            _curSegmentJoint.maxDistanceOnly = true;

            /*
            // HingeJoint2D
            HingeJoint2D _curSegmentJoint = _newSegment.AddComponent<HingeJoint2D>();
            _curSegmentJoint.autoConfigureConnectedAnchor = false;
            _curSegmentJoint.connectedAnchor = new Vector2(0, -ropeSegmentLength);
            */

            //Refait la corde de joint
            for (int j = 0; j < ropeSegments.Count; j++)
                if (j > 0)
                    ropeSegments[j].GetComponent<DistanceJoint2D>().connectedBody = ropeSegments[j - 1].GetComponent<Rigidbody2D>();
            //return true;
        }
        //return false;
    }

    #endregion Rope Enlargement Functions

    #region Rope Narrowing Functions

    public bool IsRopeAtMinLength()
    {
        if (ropeSegments.Count == 2)
        {
            Debug.Log("MinLength");
            return true;
        }
        else
            return false;
    }

    public void RemoveRopeSegment()
    {

        //Debug.Log("RemoveRopeSegment");

        if (ropeSegments.Count > minRopeLength)
        {

            for (int i = 0; i < ropeSegments.Count; i++)
            {

                if (ropeSegments[i + 1] == lastSegment)
                {

                    GameObject _segmentToRemove = ropeSegments[i];

                    ropeSegments.RemoveAt(i);

                    for (int j = 0; j < ropeSegments.Count; j++)
                        if (j > 0)
                            ropeSegments[j].GetComponent<DistanceJoint2D>().connectedBody = ropeSegments[j - 1].GetComponent<Rigidbody2D>();

                    Destroy(_segmentToRemove);

                    //return true;

                }

            }

        }

        //return false;

    }

    #endregion Rope Narrowing Functions

    void UpdateGlobaDist()
    {
        firstSegment.GetComponent<DistanceJoint2D>().distance = (ropeSegments.Count * ropeSegmentLength) + ropeElasticityLength;
    }

}

//Notes

// J'ai ajouté un line render vite fait, mais j'ai surtout testé le distance joint qui reste assez elastique
// Du coup il y a de l'énergie qui ne se transmet pas et c'est gênant!
// le spring joint était prometteur au niveau du réalisme, mais l'elasticité est difficile à controler, faudrait plus de test. 
// Le hinge joint me semble être le plus prometteur en l'état, il a un comportement assez correct et s'ajuste plus ou moins bien, à voir.
// ====> du coup j'ai testé une version alternative avec des hinge joints hésite pas à tester pour mixer => par contre si ca va trop vite, ça fucked up
// j'ai reverse 98% de mes changements sauf 2-3 ptis trucs pour te faire gagner du temps
// Cy

// PS => J'ai boosté la physique comme un gros porc, que ça soit niveau des itération autant qu'au niveau du fixed update qui s'effectue 10X plus vite. 
// Full déjà vue. C'est ptet trop mais juste sinon CA NE MARCHAIT PAS. tout simplement. Genre ta ficelle elle partait en live dès qu'on la déplaçait.
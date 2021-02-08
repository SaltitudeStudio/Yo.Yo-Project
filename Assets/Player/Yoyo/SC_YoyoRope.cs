using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_YoyoRope : MonoBehaviour
{

    [Header("Rope Parameters")]
    [SerializeField, Range(2, 100)]
    int maxRopeLength = 50;
    private int minRopeLength = 2;
    [SerializeField]
    float ropeSegmentLength = 0.1f;
    [SerializeField]
    float ropeSegmentRadius = 0.04f;
    [SerializeField]
    float ropeSegmentMass = 0.1f;
    private float curSegmentMass = 0;
    [SerializeField]
    float ropeGravityScale = 1f;
    private float curGravityScale = 0;

    [Header("Rope AddPhysics Parameters")]
    [SerializeField]
    bool transHandForceToRope = true;
    [SerializeField, Range(1,20)]
    float forceMultiplicator = 10f;

    [Header("Rope References")]
    [SerializeField]
    GameObject ropeContainer;
    [SerializeField]
    LineRenderer ropeRenderer;
    [SerializeField]
    GameObject lastSegment;

    [Header("Public References (Needed by 'Hand')")]
    public Rigidbody2D rbYoyoWeight;
    public GameObject firstSegment;

    [Header("Debug DO NOT TOUCH")]
    [SerializeField]
    List<GameObject> ropeSegments;

    // Start is called before the first frame update
    void Start()
    {

        //Ajout des segments principaux a la liste
        ropeSegments.Add(firstSegment);
        ropeSegments.Add(lastSegment);

        //Init de ces vars
        curSegmentMass = ropeSegmentMass;
        curGravityScale = ropeGravityScale;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateLineRenderer();
    }

    public void UpdateLineRenderer()
    {

        //Le nombre de points du renderer s'aligne sur la longueur de la corde
        ropeRenderer.positionCount = ropeSegments.Count;

        //les positions s'aligne
        for (int i = 0; i < ropeSegments.Count; i++)
            ropeRenderer.SetPosition(i, ropeSegments[i].transform.position);

    }

    #region Rope Enlargement Functions

    public bool IsRopeAtMaxLength()
    {
        //retourne vrai si la corde a atteind sa longueur maximale
        if (ropeSegments.Count < maxRopeLength)
            return false;
        else
            return true;
    }

    public bool canAddNewSegment()
    {

        //distance entre les deux dernier segment du tableau (le dernier est a la même position que le poid du yoyo)
        Vector2 distBetween2LastSegment = ropeSegments[ropeSegments.Count - 2].transform.position - lastSegment.transform.position;

        //si cette distance est superieur a la moitier de la longueur d'un segment autorise l'allongement de la corde
        if (distBetween2LastSegment.magnitude > ropeSegmentLength * 0.5f)
            return true;
        else
            return false;

    }

    public int segmentQuantityToAdd(Vector2 velocity)
    {
        //Calcule la quantité de segment a ajouter pour correspondra a la distance parcourue par le poids du yoyo (en une frame)
        int nQuantity = Mathf.CeilToInt((velocity.magnitude * Time.deltaTime) / ropeSegmentLength);
        return nQuantity;
    }

    public void AddRopeSegment(int nSegmentToCreate)
    {

        //Créé X Segments
        for (int i = 0; i < nSegmentToCreate; i++)
        {

            //Ne crée pas de segments si la longueur max de la corde est atteinte
            if (ropeSegments.Count < maxRopeLength)
            {

                //Creation du segment de rope
                GameObject _newSegment = new GameObject("Segment_" + ropeSegments.Count.ToString());
                _newSegment.transform.position = lastSegment.transform.position; //spawn a une position plus cool pour la physique
                _newSegment.transform.parent = ropeContainer.transform; //set le parent dans la scene
                _newSegment.layer = 9; //set la layer pour les collisions

                //Insertion du segment  dans la Liste a l'avant derniere position (derniere pos = Last Segment)
                for (int j = 0; j < ropeSegments.Count; j++)
                {
                    if (ropeSegments[j] == lastSegment)
                    {
                        ropeSegments.Insert(j, _newSegment);
                        break;
                    }
                }

                //Physics
                Rigidbody2D _curSegmentRb = _newSegment.AddComponent<Rigidbody2D>();
                _curSegmentRb.mass = curSegmentMass;
                _curSegmentRb.gravityScale = curGravityScale;

                //Collisions
                CircleCollider2D _curSegmentCol = _newSegment.AddComponent<CircleCollider2D>();
                _curSegmentCol.radius = ropeSegmentRadius;

                //DistantJoint2D
                DistanceJoint2D _curSegmentJoint = _newSegment.AddComponent<DistanceJoint2D>();
                _curSegmentJoint.autoConfigureDistance = false;
                _curSegmentJoint.distance = ropeSegmentLength;
                _curSegmentJoint.maxDistanceOnly = true;

                //Re Set les Connected Rigidbody
                for (int j = 0; j < ropeSegments.Count; j++)
                    if (j > 0)
                        ropeSegments[j].GetComponent<DistanceJoint2D>().connectedBody = ropeSegments[j - 1].GetComponent<Rigidbody2D>();

            }

        }

    }

    #endregion Rope Enlargement Functions

    #region Rope Narrowing Functions


    public bool IsRopeAtMinLength()
    {
        //retourne vrai si la corde a atteind sa longueur minimale
        if (ropeSegments.Count == 2)
            return true;
        else
            return false;
    }

    public void RemoveRopeSegment()
    {

        //Enleve un segment si la cordre et assez longue seulement
        if (ropeSegments.Count > minRopeLength)
        {

            for (int i = 0; i < ropeSegments.Count; i++)
            {

                //Si c'est l'avant dernier segment (ne jamais retirer LastSegment)
                if (ropeSegments[i + 1] == lastSegment)
                {

                    GameObject _segmentToRemove = ropeSegments[i];  //Stock le segment

                    ropeSegments.RemoveAt(i); //Retirer le segment de la Liste

                    //Re Set les Connected Rigidbody
                    for (int j = 0; j < ropeSegments.Count; j++)
                        if (j > 0)
                            ropeSegments[j].GetComponent<DistanceJoint2D>().connectedBody = ropeSegments[j - 1].GetComponent<Rigidbody2D>();

                    Destroy(_segmentToRemove);

                }

            }

        }

    }

    #endregion Rope Narrowing Functions

    #region Rope Physics Functions

    public void EnableSegmentPhysics(bool enable)
    {

        //Change la physique voulue stocké (si jamais des segment sont crée il seront set avec ces parametres)
        if (enable)
        {
            curGravityScale = ropeGravityScale;
            curSegmentMass = ropeSegmentMass;
        }
        else if (!enable)
        {
            curGravityScale = 0;
            curSegmentMass = 0;
        }

        //Set la physique voulue pour tout les segments
        for (int i = 0; i < ropeSegments.Count; i++)
        {
            ropeSegments[i].GetComponent<Rigidbody2D>().gravityScale = curGravityScale;
            ropeSegments[i].GetComponent<Rigidbody2D>().mass = curSegmentMass;
        }

    }

    public void OnAddForceToRope(Vector2 oldPos, Vector2 newPos)
    {
        if (transHandForceToRope)
        {

            Vector2 dir = newPos - oldPos;

            float d = dir.magnitude;
            float v = d / Time.deltaTime;
            float F = rbYoyoWeight.mass * v / Time.deltaTime;

            firstSegment.GetComponent<Rigidbody2D>().AddForce(F * forceMultiplicator * dir.normalized);

        }

    }

    #endregion Rope Physics Functions

}

//Notes

//28/01
// J'ai ajouté un line render vite fait, mais j'ai surtout testé le distance joint qui reste assez elastique
// Du coup il y a de l'énergie qui ne se transmet pas et c'est gênant!
// le spring joint était prometteur au niveau du réalisme, mais l'elasticité est difficile à controler, faudrait plus de test. 
// Le hinge joint me semble être le plus prometteur en l'état, il a un comportement assez correct et s'ajuste plus ou moins bien, à voir.
// ====> du coup j'ai testé une version alternative avec des hinge joints hésite pas à tester pour mixer => par contre si ca va trop vite, ça fucked up
// j'ai reverse 98% de mes changements sauf 2-3 ptis trucs pour te faire gagner du temps
// Cy

// PS => J'ai boosté la physique comme un gros porc, que ça soit niveau des itération autant qu'au niveau du fixed update qui s'effectue 10X plus vite. 
// Full déjà vue. C'est ptet trop mais juste sinon CA NE MARCHAIT PAS. tout simplement. Genre ta ficelle elle partait en live dès qu'on la déplaçait.
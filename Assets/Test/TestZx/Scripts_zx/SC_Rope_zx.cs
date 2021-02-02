
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_Rope_zx : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegLen = 0.25f;
    private int segmentLength = 20;
    private float lineWidth = 0.1f;
    private Vector2 RopeNewPosition = new Vector2(0,0);
    private Vector2 DeltaPosition = new Vector2(0,0);
    private int segmentLenghtDelta = 0;
    private Coroutine addRopeSegment;
    private Coroutine removeRopeSegment;
    private int maxSegments = 30;

    // Use this for initialization
    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
        Vector2 ropeStartPoint = this.RopeNewPosition;

        for (int i = 0; i < segmentLength; i++)
        {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.DrawRope();
    }

    private void FixedUpdate()
    {
        this.Simulate();
    }

    private void Simulate()
    {
        // SIMULATION
        Vector2 forceGravity = new Vector2(0f, -1.5f);

        for (int i = 1; i < this.segmentLength; i++)
        {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            this.ropeSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            this.ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        //Constrant to Mouse
        RopeSegment firstSegment = this.ropeSegments[0];
        RopeNewPosition += DeltaPosition;
        firstSegment.posNow = RopeNewPosition;
        this.ropeSegments[0] = firstSegment;

        for (int i = 0; i < this.segmentLength - 1; i++)
        {
            RopeSegment firstSeg = this.ropeSegments[i];
            RopeSegment secondSeg = this.ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector2 changeDir = Vector2.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                this.ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                this.ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[this.segmentLength];
        for (int i = 0; i < this.segmentLength; i++)
        {
            ropePositions[i] = this.ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }

    public void DeltaPostion(InputAction.CallbackContext Context)
    {

        DeltaPosition = Context.ReadValue<Vector2>();
        DeltaPosition /= 200;        
    }

    public void AddRopeLenght(InputAction.CallbackContext Context)
    {
        if(Context.ReadValueAsButton())
        {
            if (addRopeSegment == null)
            addRopeSegment = StartCoroutine(AddRopeSegment());
        }
        else if(addRopeSegment != null)
        {

            StopCoroutine(addRopeSegment);
            addRopeSegment = null;
        }
    }

    public void RemoveRopeLenght(InputAction.CallbackContext Context)
    {
        Debug.Log(Context.ReadValueAsButton());
        if (Context.ReadValueAsButton())
        {
            if(removeRopeSegment == null)
            removeRopeSegment = StartCoroutine(RemoveRopeSegment());
        }
        else if(removeRopeSegment != null)
        {
            StopCoroutine(removeRopeSegment);
            removeRopeSegment = null;
        }
    }

    IEnumerator AddRopeSegment()
    {
        while(true && segmentLength < maxSegments)
        {
            this.ropeSegments.Add(new RopeSegment(ropeSegments[ropeSegments.Count-1].posNow - new Vector2(0, ropeSegLen)));
            segmentLength += 1;
            yield return new WaitForSeconds(0.002f);

        }
     
    }
    IEnumerator RemoveRopeSegment()
    {
        while (true && segmentLength > 1)
        {
            segmentLength -= 1;
            this.ropeSegments.RemoveAt(ropeSegments.Count - 1);
            yield return new WaitForSeconds(0.002f);
        }
    }
}
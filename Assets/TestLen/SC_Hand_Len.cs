using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_Hand_Len : MonoBehaviour
{

    public enum YoyoState { InHand, Thrown, Away }
    Coroutine curThrowCoro;
    Coroutine curBringBackCoro;
    [SerializeField]
    float throwForce = 10;
    Vector2 throwDir;
    private FixedJoint2D handToYoyoJoint;

    [Header("Yoyo References")]
    [SerializeField]
    SC_YoyoRope_Len scYoyoRope;
    [SerializeField]
    Rigidbody2D rbYoyoWeight;

    [Header("Debug DO NOT TOUCH")]
    [SerializeField]
    YoyoState curYoyoState;
    Coroutine curAddRopeCoro;

    private void Start()
    {
        handToYoyoJoint = this.GetComponent<FixedJoint2D>();
    }

    public void OnMoveHand(InputAction.CallbackContext Context)
    {
        throwDir = Context.ReadValue<Vector2>();          
    }

    public void OnYoyoAction(InputAction.CallbackContext Context)
    {

        switch (curYoyoState)
        {

            default:
                break;

            case YoyoState.InHand:
                OnThrowYoyo(Context);
                break;

            case YoyoState.Thrown:
                StopYoyoThrow(Context);
                break;

            case YoyoState.Away:
                OnBringBackYoyo(Context);
                break;

        }

    }

    void OnThrowYoyo(InputAction.CallbackContext Context)
    {
        if (Context.performed)
        {
            handToYoyoJoint.enabled = false;
            curThrowCoro = StartCoroutine(ThrowYoyo());
            rbYoyoWeight.AddForce(GetThrowForce());
            curYoyoState = YoyoState.Thrown;
        }
    }

    Vector2 GetThrowForce()
    {
        return throwDir.normalized * throwForce;
    }

    IEnumerator ThrowYoyo()
    {

        while (!scYoyoRope.IsRopeAtMaxLength())
        {
            if (scYoyoRope.canAddNewSegment())
                scYoyoRope.AddRopeSegment(1);
            yield return null;
        }

        curYoyoState = YoyoState.Away;

    }

    void StopYoyoThrow(InputAction.CallbackContext Context)
    {
        if (Context.performed)
        {
            if (curThrowCoro != null)
                StopCoroutine(curThrowCoro);
            curYoyoState = YoyoState.Away;
        }
    }

    public void OnBringBackYoyo(InputAction.CallbackContext Context)
    {

        if (Context.performed)
        {

            if (curBringBackCoro != null)
                StopCoroutine(curBringBackCoro);

            curBringBackCoro = StartCoroutine(BringBackYoyo());

        }

        if (Context.canceled)
        {
            if (curBringBackCoro != null)
                StopCoroutine(curBringBackCoro);
        }

    }

    IEnumerator BringBackYoyo()
    {
        while (!scYoyoRope.IsRopeAtMinLength())
        {
            scYoyoRope.RemoveRopeSegment();
            yield return null;
        }

        curYoyoState = YoyoState.InHand;
        handToYoyoJoint.enabled = true;

    }

    #region Debugs / Tests Functions

    public void OnAddRope(InputAction.CallbackContext Context)
    {

        if (Context.performed)
        {

            //Debug.Log("OnPerformed");

            if (curAddRopeCoro != null)
                StopCoroutine(curAddRopeCoro);

            curAddRopeCoro = StartCoroutine(AddRope());

        }

        if (Context.canceled)
        {

            //Debug.Log("OnCanceled");

            if (curAddRopeCoro != null)
                StopCoroutine(curAddRopeCoro);

            //Debug.Log("curAddRopeCoro = " + curAddRopeCoro);

        }

    }

    IEnumerator AddRope()
    {
        while (!scYoyoRope.IsRopeAtMaxLength())
        {
            scYoyoRope.AddRopeSegment(1);
            yield return null;
        }
    }

    #endregion Debugs / Tests Functions

}

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

    [Header("Yoyo References")]
    [SerializeField]
    SC_YoyoRope_Len scYoyoRope;
    [SerializeField]
    Rigidbody2D rbYoyoWeight;

    [Header("Debug DO NOT TOUCH")]
    [SerializeField]
    YoyoState curYoyoState;
    Coroutine curAddRopeCoro;

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
            curThrowCoro = StartCoroutine(ThrowYoyo());
            rbYoyoWeight.AddForce(GetThrow());
            curYoyoState = YoyoState.Thrown;
        }

    }

    Vector2 GetThrow()
    {

        //Plus tard Direction choisie par le joueur
        Vector2 _ThrowDirection = new Vector2(1, 1);

        return _ThrowDirection.normalized * throwForce;

    }

    IEnumerator ThrowYoyo()
    {
        //Aggrandir la chaine au fur et a mesure
        return null;
    }

    void StopYoyoThrow(InputAction.CallbackContext Context)
    {

        if (Context.performed)
        {
            StopCoroutine(curThrowCoro);
            curYoyoState = YoyoState.Away;
        }

    }

    public void OnBringBackYoyo(InputAction.CallbackContext Context)
    {

        if (Context.performed)
            curBringBackCoro = StartCoroutine(BringBackYoyo());

        if (Context.canceled)
            StopCoroutine(curBringBackCoro);

    }

    IEnumerator BringBackYoyo()
    {
        while (scYoyoRope.RemoveRopeSegment())
            yield return new WaitForEndOfFrame();
    }

    public void OnAddRope(InputAction.CallbackContext Context)
    {

        if (Context.performed)
            curAddRopeCoro = StartCoroutine(AddRope());

        if (Context.canceled)
            StopCoroutine(curAddRopeCoro);

    }

    IEnumerator AddRope()
    {
        while (scYoyoRope.AddRopeSegment())
            yield return new WaitForEndOfFrame();
    }

}

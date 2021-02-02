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
    float handDistFromBody = 0.7f;

    [SerializeField]
    float throwForce = 20;
    Vector2 throwDir;

    private FixedJoint2D bodyToYoyoJoint;
    private FixedJoint2D handToYoyoJoint;
    private SC_YoyoRope_Len scYoyoRope;
    private Rigidbody2D rbYoyoWeight;
    private Rigidbody2D rbFirstSegment;

    [Header("References")]
    [SerializeField]
    GameObject body;

    [Header("Yoyo References")]
    [SerializeField]
    GameObject curEquippedYoyo;

    [Header("Debug DO NOT TOUCH")]
    [SerializeField]
    YoyoState curYoyoState;
    Coroutine curAddRopeCoro;

    private void Start()
    {
        GetReferences();
        OnHand();
    }

    //Recupere les références sur le yoyo equipé
    void GetReferences()
    {

        scYoyoRope = curEquippedYoyo.GetComponent<SC_YoyoRope_Len>();
        rbYoyoWeight = scYoyoRope.rbYoyoWeight;
        rbFirstSegment = scYoyoRope.firstSegment.GetComponent<Rigidbody2D>();

        handToYoyoJoint = this.GetComponent<FixedJoint2D>();
        handToYoyoJoint.connectedBody = rbYoyoWeight;

        bodyToYoyoJoint = body.GetComponent<FixedJoint2D>();
        bodyToYoyoJoint.connectedBody = rbFirstSegment;
        bodyToYoyoJoint.anchor = this.transform.localPosition;

    }

    public void OnMoveHand(InputAction.CallbackContext Context)
    {
        throwDir = Context.ReadValue<Vector2>(); //Stock l'angle du RStick  
        this.transform.localPosition = throwDir * handDistFromBody;
        bodyToYoyoJoint.anchor = this.transform.localPosition;
    }

    //Appeler par la touche d'action
    public void OnYoyoAction(InputAction.CallbackContext Context)
    {

        switch (curYoyoState)
        {

            default:
                break;

            case YoyoState.InHand: //Quand le yoyo est dans la main le jette
                OnThrowYoyo(Context);
                break;

            case YoyoState.Thrown: //Quand le yoyo est jeté le stoppe
                StopYoyoThrow(Context);
                break;

            case YoyoState.Away: //Quand le yoyo est stoppé le ramene vers la main 
                OnBringBackYoyo(Context);
                break;

        }

    }

    void OnHand()
    {

        curYoyoState = YoyoState.InHand; //Change la State
        handToYoyoJoint.enabled = true; //La main "tien" le yoyo

        scYoyoRope.EnableSegmentPhysics(false); //On desactive la physique de la corde
        rbYoyoWeight.gravityScale = 0; // On desactive le gravité sur le yoyo pour pas que la main tombe

    }

    void OnThrowYoyo(InputAction.CallbackContext Context)
    {
        if (Context.performed) //Quand le btn est appuyé =>
        {

            //Lance la Coroutine du lancer
            if (curThrowCoro != null)
                StopCoroutine(curThrowCoro);
            curThrowCoro = StartCoroutine(ThrowingYoyo());
          
            curYoyoState = YoyoState.Thrown; //Change la State

        }
    }

    IEnumerator ThrowingYoyo() 
    {

        handToYoyoJoint.enabled = false; //La main "lache" le yoyo

        scYoyoRope.EnableSegmentPhysics(false); //On desactive la physique de la corde pour eviter de gener le lancé au maximum

        rbYoyoWeight.gravityScale = 1; // On active la gravité sur le poid du yoyo
        rbYoyoWeight.velocity = GetThrowForce(); //On "lance" le poids du yoyo

        scYoyoRope.AddRopeSegment(scYoyoRope.segmentQuantityToAdd(rbYoyoWeight.velocity)); //Pré ajoute un certain nombre de segment a la corde

        //Agrandit la corde au fur et a mesure que le yoyo part
        while (!scYoyoRope.IsRopeAtMaxLength())
        {
            if (scYoyoRope.canAddNewSegment())
                scYoyoRope.AddRopeSegment(scYoyoRope.segmentQuantityToAdd(rbYoyoWeight.velocity));
            yield return null;
        }

        scYoyoRope.EnableSegmentPhysics(true); //On reactive la physique de la corde mtn que la corde a atteind sa longueur maximale

        curYoyoState = YoyoState.Away; //Change la State

    }

    Vector2 GetThrowForce() 
    {
        return throwDir.normalized * throwForce;
    }

    //Stop le deroulement de la corde pendant que le yoyo est jeté 
    void StopYoyoThrow(InputAction.CallbackContext Context)
    {
        if (Context.performed) //Click pour arreter
        {

            //Coupe la coroutine et donc le deroulement de la corde
            if (curThrowCoro != null)
                StopCoroutine(curThrowCoro);

            scYoyoRope.EnableSegmentPhysics(true); //On reactive la physique de la corde mtn que la corde a arreter de s'aggrandir

            curYoyoState = YoyoState.Away; //Change la State

        }
    }

    //Ramene le yoyo a la main (maintenir pour ramener)
    public void OnBringBackYoyo(InputAction.CallbackContext Context)
    {

        if (Context.performed) //Au press active la coroutine qui ramene le yoyo
        {

            if (curBringBackCoro != null)
                StopCoroutine(curBringBackCoro);

            curBringBackCoro = StartCoroutine(BringBackYoyo());

        }

        if (Context.canceled) //relacher coupe la coroutine
        {
            if (curBringBackCoro != null)
                StopCoroutine(curBringBackCoro);
        }

    }

    IEnumerator BringBackYoyo()
    {

        //Tant que la corde n'est pas a taille minimale enleve un segment de corde
        while (!scYoyoRope.IsRopeAtMinLength())
        {
            scYoyoRope.RemoveRopeSegment();
            yield return null;
        }

        OnHand(); //Fonction qui "met" le yoyo dans la main

    }

    #region Debugs / Tests Functions

    //Tant qu'on maintient la touche ajoute de la corde
    public void OnAddRope(InputAction.CallbackContext Context)
    {

        if (Context.performed) //Au press active la coroutine qui allonge la corde
        {

            if (curAddRopeCoro != null)
                StopCoroutine(curAddRopeCoro);

            curAddRopeCoro = StartCoroutine(AddRope());

        }

        if (Context.canceled) // relacher coupe la coroutine
        {
            if (curAddRopeCoro != null)
                StopCoroutine(curAddRopeCoro);
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

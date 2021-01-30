using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_Yoyo_Len : MonoBehaviour
{

    public enum YoyoState { InHand, Thrown, Away, BringBack }
    Coroutine curBringBackCoro;

    [SerializeField]
    SC_YoyoRope_Len scYoyoRope;

    [Header("Debug DO NOT TOUCH")]
    [SerializeField]
    YoyoState curYoyoState;
    Coroutine curAddRopeCoro;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void ThrowYoyo()
    {

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

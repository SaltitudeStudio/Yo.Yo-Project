using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_ebo : MonoBehaviour
{
    Player_Gameplay_Controls player_Controls = null; //Schema contenant les controles du joueur
    Rigidbody2D playerRigidBody = null; //Rigidbody du player

    [SerializeField] float moveMultiplierVelocity = 0; //Multiplicateur de force
    [SerializeField] float moveMultiplierForce = 0; //Multiplicateur de force
    [SerializeField] float jumpMultiplier = 0; //Multiplicateur de force
    [SerializeField] AnimationCurve curveMoveGround = null;
    [SerializeField] AnimationCurve curveMoveGroundForce = null;
    [SerializeField] bool ModeVelocityOrForce = true;
    float joysticeValue = 0; //Velocité du joueur
    float distToGround = 1.5f;

    private void Awake()
    {
        player_Controls = new Player_Gameplay_Controls(); //Initialisation des controles
        playerRigidBody = this.GetComponent<Rigidbody2D>(); //Initialisation du RigidBody
        CreateControls(); //Initialisation et bind des controles
    }
    private void OnEnable()
    {
        player_Controls.Enable();
    }
    private void OnDisable()
    {
        player_Controls.Disable();
    }
    void CreateControls()
    {
        //----- MOVE
        player_Controls.Ground.move.performed += context => joysticeValue = context.ReadValue<float>(); //Quand le stick de gauche bouge, modifie velocity [-1,1]
        //player_Controls.Ground.move.canceled += context => StopPlayer(); //Quand le stick est relaché, remet velocity à 0

        //---- JUMP
        player_Controls.Ground.jump.performed += context => JumpPlayer();
    }

    private void MovePlayer(float value)
    {
        float valueWithCurve = ModeVelocityOrForce ? curveMoveGround.Evaluate(value) : curveMoveGroundForce.Evaluate(value);
        if (ModeVelocityOrForce) playerRigidBody.velocity = new Vector2(value * moveMultiplierVelocity * valueWithCurve, playerRigidBody.velocity.y); //MODE VELOCITY
        else playerRigidBody.AddForce( new Vector2 (value * moveMultiplierForce * valueWithCurve, 0), ForceMode2D.Force); //MODE FORCE
    }
    private void StopPlayer()
    {
        Debug.Log("Stop Player");
        playerRigidBody.velocity = new Vector2(0, playerRigidBody.velocity.y);
    }

    private void JumpPlayer()
    {
        if (IsGrounded()) playerRigidBody.AddForce(new Vector2(0, jumpMultiplier));
        else Debug.Log("Can't Jump");
    }

    private bool IsGrounded()
    {
        int layerMask = 1 << 10;
        Vector3 dir = transform.TransformDirection(Vector3.down);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir,distToGround,layerMask);
        Debug.DrawRay(transform.position, dir * distToGround, Color.yellow);
        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Start()
    {

    }

    //---------------------------------- UPDATE ------------------------------------------
    void Update()
    {
        if (joysticeValue != 0f)
        {
            if (IsGrounded()) MovePlayer(joysticeValue);
            else MovePlayer(joysticeValue / 4f);
        }
        //else
        //{
        //    StopPlayer();
        //}
    }
    private void FixedUpdate()
    {

    }
}

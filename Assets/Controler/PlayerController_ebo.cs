using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_ebo : MonoBehaviour
{
    Player_Gameplay_Controls player_Controls = null; //Schema contenant les controles du joueur
    Rigidbody2D playerRigidBody = null; //Rigidbody du player

    [SerializeField] float moveMultiplier = 0; //Multiplicateur de force
    [SerializeField] float jumpMultiplier = 0; //Multiplicateur de force
    float velocity = 0; //Velocité du joueur
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
        player_Controls.Ground.move.performed += context => velocity = context.ReadValue<float>(); //Quand le stick de gauche bouge, modifie velocity
        player_Controls.Ground.move.canceled += context => StopPlayer(); //Quand le stick est relaché, remet velocity à 0

        //---- JUMP
        player_Controls.Ground.jump.performed += context => JumpPlayer();
    }

    private void MovePlayer(float value)
    {
        Debug.Log("Move");
        playerRigidBody.AddForce(new Vector2(value * moveMultiplier, 0)); //Ajoute de la force en fonction du sesns
    }
    private void StopPlayer()
    {
        velocity = 0;
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
        if (velocity != 0f)
        {
            MovePlayer(velocity);
        }
        else
        {
            StopPlayer();
        }
    }
    private void FixedUpdate()
    {

    }
}

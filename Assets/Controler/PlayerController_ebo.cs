using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_ebo : MonoBehaviour
{
    Player_Gameplay_Controls player_Controls = null; //Schema contenant les controles du joueur
    Rigidbody2D playerRigidBody = null; //Rigidbody du player

    [Header("Les Trucs utilisés")]

    [SerializeField] float moveMultiplierVelocity = 0;
    [SerializeField] float jumpMultiplierVelocity = 5;
    [SerializeField] AnimationCurve curveMoveGround = null;
    //qui gère aussi pour l'instant la vitesse de transistion de 'lacceleration
    [SerializeField] float BrakeSpeed = 50;

    //curve de velocité(y) de saut(y) par rapport au temps(x)
    [SerializeField] AnimationCurve curveJumpVelocity = null;


    float joysticeValue = 0; //Velocité du joueur
    float distToGround = 0.2f;

    bool isGrounded_b;
    bool isJumping = false;





    [Header("Les Trucs pas utilisés")]
    [SerializeField] float moveMultiplierForce = 0; //Multiplicateur de force
    [SerializeField] float jumpMultiplier = 0; //Multiplicateur de force
    
    [SerializeField] AnimationCurve curveMoveGroundForce = null;
    [SerializeField] bool ModeVelocityOrForce = true;

    
    

    

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
    {/*
        float valueWithCurve = ModeVelocityOrForce ? curveMoveGround.Evaluate(value) : curveMoveGroundForce.Evaluate(value);
        if (ModeVelocityOrForce) playerRigidBody.velocity = new Vector2(value * moveMultiplierVelocity * valueWithCurve, playerRigidBody.velocity.y); //MODE VELOCITY
        else playerRigidBody.AddForce( new Vector2 (value * moveMultiplierForce * valueWithCurve, 0), ForceMode2D.Force); //MODE FORCE
        
     */

        //playerRigidBody.AddForce(new Vector2(value * moveMultiplierForce, 0), ForceMode2D.Force);

        //--------------------------------------------------------------------------------------------------------------------
        //Finalement on va faire avec la velocité parce que pour le faire avec une courbe ca a plus de sens avec la velocité
        //--------------------------------------------------------------------------------------------------------------------

        Vector2 lateralVelo = playerRigidBody.velocity;

        lateralVelo.x = Mathf.Lerp(lateralVelo.x, Mathf.Sign(value)*curveMoveGround.Evaluate(Mathf.Abs(value))*moveMultiplierVelocity, Time.deltaTime * BrakeSpeed);
        playerRigidBody.velocity = lateralVelo;

    }
    private void StopPlayer()
    {

        //Debug.Log("Stop Player");
        Vector2 lateralVelo = playerRigidBody.velocity;

        lateralVelo.x = Mathf.Lerp(lateralVelo.x, 0, Time.deltaTime * BrakeSpeed);
        playerRigidBody.velocity = lateralVelo;
    }

    private void JumpPlayer()
    {
        if (isGrounded_b)
        {
            //playerRigidBody.AddForce(new Vector2(0, jumpMultiplier));

            StartCoroutine(Jumping(0));
        }
            
        else Debug.Log("Can't Jump");
    }


    //NE GERE PAS ENCORE LA VELOCITE QUAND LE JOUEUR TOMBE SANS AVOIR SAUTE!
    IEnumerator Jumping(float startCounterValue)
    {
        isJumping = true;

        float _counter = startCounterValue;

        Vector2 verticalVelo;

        //cette valeur de condition devrait ^^etre récupérée de la position de la clé de al courbe ou qqchose comme ça et sert à ignorer le raycast de isGrounded le temps de décoler ==> il faudra aussi
        //réduire la longueur du raycast pour moins d'emmerdes
        while(_counter < 0.3f || !isGrounded_b && _counter <= curveJumpVelocity.length)
        {
            _counter += Time.deltaTime;

            verticalVelo = playerRigidBody.velocity;
            Debug.Log(curveJumpVelocity.Evaluate(_counter));
            verticalVelo.y = curveJumpVelocity.Evaluate(_counter) * jumpMultiplierVelocity;

            playerRigidBody.velocity = verticalVelo;

           yield return null;

        }

        isJumping = false;

    }


    // En vrai la methode du raycast est top pour les détection de hauteur, mais pour le isgrounded il vaut mieux un trigger, sinon quand on a que 1 pied sur la plateforme on est bloqué =/ oups =x
    /*
        private bool IsGrounded()
    {
        int layerMask = 1 << 10;
        Vector3 dir = transform.TransformDirection(Vector3.down);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir,distToGround,layerMask);
        Debug.DrawRay(transform.position, dir * distToGround, Color.yellow);
        if (hit.collider != null)
        {

            isGrounded_b = true;
            return true;
        }
        else
        {
            isGrounded_b = false;
            return false;
        }
    }*/


    private void OnTriggerStay2D(Collider2D other)
    {
        isGrounded_b = true;
        Debug.Log("isGrounded_b true");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isGrounded_b = false;

        //ici on lance la courbe de velocité verticale à partir de la clé se trouvant en son sommet puisqu'il s'agit du cas où on se laisse tomber
        if (!isJumping)
            StartCoroutine(Jumping(curveJumpVelocity.keys[1].time));
    }

    void Start()
    {

    }

    //---------------------------------- UPDATE ------------------------------------------
    void Update()
    {

        if (joysticeValue != 0f)
        {
            if (isGrounded_b) 
                MovePlayer(joysticeValue);
            else 
                MovePlayer(joysticeValue / 4f);
        }

        else if (joysticeValue == 0f && isGrounded_b)
        {
           StopPlayer();
        }

    }
    private void FixedUpdate()
    {

    }
}

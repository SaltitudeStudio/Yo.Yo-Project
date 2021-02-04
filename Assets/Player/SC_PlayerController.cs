using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// ////////////////////////////////////////////////////////////////////////////////////////////////////////
/// /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
/// /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
///                         SCRIPT DES CONTROLES DU PERSONNAGE ET GESTION DES ACTIONS
/// ////////////////////////////////////////////////////////////////////////////////////////////////////////
/// /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
/// /// ////////////////////////////////////////////////////////////////////////////////////////////////////////


public class SC_PlayerController : MonoBehaviour
{
    Player_Gameplay_Controls player_Controls = null; //Schema contenant les controles du joueur
    Rigidbody2D playerRigidBody = null; //Rigidbody du player

    [Header("Les Trucs utilisés")]

    [SerializeField] AnimationCurve curveMoveGround = null;
    [SerializeField] float moveMultiplierVelocity = 0;

    [SerializeField] float AccelSpeed = 50;
    [SerializeField] float BrakeSpeed = 15;

    //curve de velocité(y) de saut(y) par rapport au temps(x)
    [SerializeField] AnimationCurve curveJumpVelocity = null;
    [SerializeField] float jumpMultiplierVelocity = 5;

    [Header("Checks de corners")]
    [SerializeField] Transform feetCheck_l;
    [SerializeField] Transform kneeCheck_l;
    [SerializeField] Transform feetCheck_r;
    [SerializeField] Transform kneeCheck_r;
    //valeur de boost en velocité en y pour permettre le passage si les pieds butent dans une plateforme
    [SerializeField] float velocity_Y_boost = 2000;
    Coroutine boostCoro;
    bool goBoost = false;

    [Header("Buffer")]
    //JumpBuffer
    bool jumpBuffered = false;



    float joysticeValue = 0; //Velocité du joueur
    float distToGround = 0.2f;

    bool isGrounded_b;
    bool isJumping = false;


    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////OLD//CODE//////////////////////////////////////////////////////
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////

    [Header("Les Trucs pas utilisés")]
    [SerializeField] float moveMultiplierForce = 0; //Multiplicateur de force
    [SerializeField] float jumpMultiplier = 0; //Multiplicateur de force
    
    [SerializeField] AnimationCurve curveMoveGroundForce = null;
    [SerializeField] bool ModeVelocityOrForce = true;

    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// ///////////////////////////////////////ZONE D'EXPRESSION LIBRE//////////////////////////////////////////
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// Coucou Tienou, j'ai bougé pas mal de trucs dans le script, je 'ai essayé de faire des déplacements
    /// avec un petit caractère, du coup j'ai essayé avec des courbes et du coup avec la vélocité (tu verras plus bas)
    /// 
    /// Forcément ça perd en réalisme, mais le côté arcade peut être marrant.
    /// 
    /// J'ai également taillé une première version de la corner correction avec ses pitis raycasts et mis un buffer pour le jump
    /// 
    /// J'ai au passage ajouté le package cinemachine pour nous faire gagner du temps plus tard
    /// C'est règlé à l'arrache ça par contre, je te préviens.
    /// 
    /// Des bisouxxx <3 
    ///
    /// Cycy
    /// 
    /// PS : J'ai laissé presque tout le code que j'ai pas utilisé en commentaire. A toi de voir si tu veux le nettoyer ou le garder plus longtemps
    /// just in case.
    /// 
    /// Cycy.Rigidbody.sleepMode = true;
    /// break;
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////





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
    {/* CODE MIS EN RESERVE
        float valueWithCurve = ModeVelocityOrForce ? curveMoveGround.Evaluate(value) : curveMoveGroundForce.Evaluate(value);
        if (ModeVelocityOrForce) playerRigidBody.velocity = new Vector2(value * moveMultiplierVelocity * valueWithCurve, playerRigidBody.velocity.y); //MODE VELOCITY
        else playerRigidBody.AddForce( new Vector2 (value * moveMultiplierForce * valueWithCurve, 0), ForceMode2D.Force); //MODE FORCE
        
     */


        //--------------------------------------------------------------------------------------------------------------------
        //Finalement j'ai fait déplacements et saut avec la velocité pour avoir un controle absolu. C'est un peu plus rigide et en théorie appliquer la courbe avec un addForce serait plus cool/clean
        //Faire avec la velo est surtout plus rapide et facile pour : la vitesse de chute entièrement maîtrisée et pareil pour le freinage, qui demandent autrement des applications de forces supplémentaires
        // et potentiellement compliquées à régler.
        //--------------------------------------------------------------------------------------------------------------------

        Vector2 lateralVelo = playerRigidBody.velocity;

        lateralVelo.x = Mathf.Lerp(lateralVelo.x, Mathf.Sign(value)*curveMoveGround.Evaluate(Mathf.Abs(value))*moveMultiplierVelocity, Time.deltaTime * AccelSpeed);
        playerRigidBody.velocity = lateralVelo;

        
        //Ici on va gérer la corner correction, permettant un petit step-up en cas de prise des pieds dans la plateforme.

        Vector2 _dir;
        Transform _feetCheck;
        Transform _kneeCheck;

        //on établie quels points de départ et direction de raycast utiliser
        _dir = (value > 0) ? Vector2.right : Vector2.left;
        _feetCheck = (value>0)? feetCheck_r : feetCheck_l;
        _kneeCheck = (value>0)? kneeCheck_r : kneeCheck_l;


        //si les pieds sont bloqués mais pas les genoux on booste
        if (LateralRaycastCheck(isGrounded_b? _feetCheck.position: _feetCheck.position+-Vector3.up/5, _dir) == true && LateralRaycastCheck(_kneeCheck.position, _dir) == false)
        {
            goBoost = true;
            if (boostCoro == null)
                StartCoroutine(VerticalBoosting(0.1f, velocity_Y_boost));
        }
        else
            goBoost = false;



    }

    //fonction pour les checks raycast
    bool LateralRaycastCheck(Vector3 _startPoint, Vector2 _dir)
    {
        int layerMask = 1 << 10;
        RaycastHit2D _hit = Physics2D.Raycast(_startPoint, _dir, 0.3f, layerMask);
        
        if (_hit.collider != null)
        {
            Debug.DrawRay(_startPoint, _dir * 1, Color.red,1) ;

            return true;
        }
        else
        {
            Debug.DrawRay(_startPoint, _dir * 1, Color.green);
            return false;
        }

    }

    //La corner correction, pas encore smooth du tout dans les escaliers x]
    IEnumerator VerticalBoosting(float _duration, float _boostVelocityStrength)
    {
        float _counter = 0;

        Debug.Log("boost");

        //la duration est là par sécurité pour éviter de partir dans l'espace parce qu'on a rrêté d'avancé devant une marche d'escalier
        while (goBoost && _counter <= _duration)
        {
            _counter += Time.deltaTime;
            yield return new WaitForFixedUpdate();
            ///// on annule la velocité verticale présente ======> /!\/!\/!\/!\/!\ Mais ca cause des comportements saccadés contre les bords de plateformes.
            Vector2 vertiVelo = playerRigidBody.velocity;

            vertiVelo.y = 0;
            playerRigidBody.velocity = vertiVelo;
            /////
            playerRigidBody.AddForce(Vector2.up * _boostVelocityStrength, ForceMode2D.Force);
        }

        boostCoro = null;
    }


    private void StopPlayer()
    {

        //Debug.Log("Stop Player");
        Vector2 lateralVelo = playerRigidBody.velocity;

        lateralVelo.x = Mathf.Lerp(lateralVelo.x, 0, Time.deltaTime * (BrakeSpeed/5));
        playerRigidBody.velocity = lateralVelo;
    }

    

    private void JumpPlayer()
    {
        if (isGrounded_b)
        {
            //playerRigidBody.AddForce(new Vector2(0, jumpMultiplier));

            StartCoroutine(Jumping(0));
        }
        else
            StartCoroutine(BufferingJump(0.20f));

            
        //else Debug.Log("Can't Jump");
    }

    //fonctionne pas encore parfaitement puisqu'il faudrait vérifier si la coro est déjà lancée et l'arrêter/relancer dans ce cas.
    IEnumerator BufferingJump(float _bufferLengthMemory)
    {
        float _counter = 0;

        jumpBuffered = true;

        while(_counter <= _bufferLengthMemory)
        {
            _counter += Time.deltaTime;
            yield return null;
        }
        jumpBuffered = false;
    }


    //------------------------------------------------------------------
    // J'ai tenté un saut un peu particulier puisque le personnage retombe plus vite qu'il ne monte, un peu à la mario. L'application du truc est pas encore fou
    //------------------------------------------------------------------
    IEnumerator Jumping(float startCounterValue)
    {
        isJumping = true;

        float _counter = startCounterValue;

        Vector2 verticalVelo;

        //cette valeur de condition devrait être récupérée de la position de la clé de la courbe ou qqchose comme ça et sert à ignorer le raycast de isGrounded le temps de décoler
        //On arrête d'appliquer une velocité si on est en situation de boost vertical pour *corner correction*
        while(_counter < 0.3f || !isGrounded_b && _counter <= curveJumpVelocity.length )
        {
            if (!goBoost)
            {
                _counter += Time.deltaTime;

                verticalVelo = playerRigidBody.velocity;
                //Debug.Log(curveJumpVelocity.Evaluate(_counter));
                //On règle la velocité suivant la position sur la courbe en fonction du temps
                verticalVelo.y = curveJumpVelocity.Evaluate(_counter) * jumpMultiplierVelocity;

                playerRigidBody.velocity = verticalVelo;

            }
            

           yield return null;

        }

        isJumping = false;

    }


    // En vrai la methode du raycast est top pour les détection de hauteur, mais pour le isgrounded il vaut mieux un trigger, sinon quand on a que 1 pied sur la plateforme on est bloqué =/ 
    // oups déso j'avais dit de la merde =x

    /*  CODE MIS EN RESERVE
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
        //Debug.Log("isGrounded_b true");

        if (jumpBuffered)
        {
            JumpPlayer();
            jumpBuffered = false;
        }
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
                MovePlayer(joysticeValue / 2f);
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

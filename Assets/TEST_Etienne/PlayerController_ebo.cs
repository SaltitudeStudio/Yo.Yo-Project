using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_ebo : MonoBehaviour
{
    Player_Gameplay_Controls player_Controls = null; //Schema contenant les controles du joueur
    Rigidbody2D playerRigidBody = null; //Rigidbody du player

    [SerializeField] float forceMultiplier = 0; //Multiplicateur de force
    float velocity = 0; //Velocité du joueur

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
        player_Controls.Ground.move.performed += context => velocity = context.ReadValue<float>(); //Quand le stick de gauche bouge, modifie velocity
        player_Controls.Ground.move.canceled += context => StopPlayer(); //Quand le stick est relaché, remet velocity à 0
    }

    private void MovePlayer(float value)
    {
        playerRigidBody.AddForce(new Vector2(value * forceMultiplier, 0)); //Ajoute de la force en fonction du sesns
    }
    private void StopPlayer()
    {
        velocity = 0;
    }

    void Start()
    {
        
    }

    //---------------------------------- UPDATE ------------------------------------------
    void Update()
    {
        if(velocity != 0f)
        {
            MovePlayer(velocity);
        }
        else
        {
            StopPlayer();
        }
    }
}

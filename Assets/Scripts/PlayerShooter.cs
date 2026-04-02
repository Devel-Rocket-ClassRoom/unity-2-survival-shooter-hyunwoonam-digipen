using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using System.Collections;

public class PlayerShooter : MonoBehaviour
{
    public Gun gun;

    private PlayerInput playerInput;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

    }

    private void Update()
    {
        if (playerInput.Fire)
        {
            gun.Fire();
        }
    }

}

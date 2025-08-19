/******************************************************************
 * This script is attached to the Player.
 * It listens for the player to press VR buttons to call 
 * corresponding method in appropriate classes to cause actions. 
 * The thumbstick is handled directly by the XRRigMovement
 * 
 * 
 * Author: Bruce Gustin
 * Date Written: July 8, 2025
 * Version 1.1 - Fixed VR movement direction
 *****************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private PlayerOrbOptions playerOrbOptions;
    private GameManager gameManager;
    private bool isFiring;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerOrbOptions = GetComponent<PlayerOrbOptions>();
        gameManager = GameManager.Instance;
    }


    // Listens for right trigger input then call fire (pressed) and StopFiring (release) in Player Atack.
    public void OnFireInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isFiring)
        {
            isFiring = true;
            playerOrbOptions.Fire();
        }
        else if (ctx.canceled && isFiring)
        {
            isFiring = false;
            playerOrbOptions.StopFiring();
        }
    }

    // Listens for right primary buttom (A) input then call fire (pressed) and StopFiring (release) in Player Atack.
    public void OnToggleInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            playerOrbOptions.SwitchAttack();
        }
    }

    // Listens for right primary buttom (B) input then starts game
    public void OnStartInput(InputAction.CallbackContext ctx) => gameManager.StartGame();

    // Listens for right grip input then turns page of Book of Lore
    public void OnPageTurnInput(InputAction.CallbackContext ctx)
    {

        if (ctx.performed)
        {
            gameManager.TurnPage();
        }
    }
}


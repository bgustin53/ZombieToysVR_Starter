/**********************************************************************************
 * This script is attached to the XRRig.
 * This script uses the InputAction manager to get the move action.  With that 
 * it determines the movement of the XRRig which is the parent of the main camera 
 * which is the parent of the player
 * 
 * Author: Bruce Gustin
 * Date Written: July 8, 2025
 * Version 2.0 Added vertical speed
 ***********************************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

public class XRRigMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float speed = 6.75f;

    private Animator animator;
    private CharacterController characterController;
    private float verticalSpeed;
    private float yPos;
    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        yPos = transform.position.y;

        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                cameraTransform = mainCam.transform;
        }
    }

    private void OnEnable()
    {
        moveAction?.action.Enable();
    }

    void Update() // Use Update instead of FixedUpdate for CharacterController
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (cameraTransform == null || !canMove) return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        Vector3 direction = (forward * input.y + right * input.x).normalized;
        Vector3 horizontalVelocity = direction * speed;

        // Handle vertical speed
        if (transform.position.y > yPos)
        {
            verticalSpeed = -0.9f; // slight downward push
        }
        else
        {
            verticalSpeed = 0f;
        }

        Vector3 motion = (horizontalVelocity + Vector3.up * verticalSpeed) * Time.deltaTime;
        characterController.Move(motion);

        // Animator logic
        if (animator != null && animator.avatar != null)
        {
            bool isMoving = input.magnitude > 0.01f;
            animator.SetBool("isMoving", isMoving);
        }
    }

    // Called when the player is defeated
    public void Defeated()
    {
        canMove = false;
    }
}

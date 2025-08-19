using UnityEngine;
using UnityEngine.InputSystem;

public class XRRigMovement : MonoBehaviour
{

    [SerializeField] private InputActionReference moveAction;  //Attach the move action of the input action
    [SerializeField] private Transform cameraTransform; // Assign your VR camera here
    [SerializeField] private float speed = 6.75f;
    private Animator animator;
    private Rigidbody rb;
    private bool canMove = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        // If camera transform not assigned, try to find it
        if (cameraTransform == null)
        {
            // Try to find the main camera or VR camera
            Camera mainCam = Camera.main;
            if (mainCam != null)
                cameraTransform = mainCam.transform;
        }
    }

    // ENables the move action in the player input
    private void OnEnable()
    {
        moveAction?.action.Enable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovePlayer();
    }

    // Moves the player relative to camera facing direction
    private void MovePlayer()
    {
        if (cameraTransform == null || !canMove) return;

        // Lock X and Z rotation to prevent unwanted tilting
        rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);

        // Get forward and right directions from camera (flattened to ignore pitch)
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        // Calculate movement direction relative to camera facing
        Vector3 direction = (forward * input.y + right * input.x).normalized;

        // Use MovePosition instead of AddForce for precise control
        Vector3 movement = direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Set the IsWalking parameter of the animator
        if (animator.avatar != null)
        {
            bool isMoving = movement.magnitude > 0.01f;
            animator.SetBool("isMoving", isMoving);
        }
    }

    // Called when the player is defeated
    public void Defeated()
    {
        // Player can no longer move
        canMove = false;
    }

    //This changes the playerNearBook field to true so the player can change the page in the book
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Book of Lore"))
        {
            GameManager.Instance.playerNearBook = true;
        }
    }

    //This changes the playerNearBook field to true so the player can't change the page in the book
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Book of Lore"))
        {
            GameManager.Instance.playerNearBook = false;
        }
    }
}


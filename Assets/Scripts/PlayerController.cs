using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;
    private Transform playerCamera;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>().transform;
        rb = GetComponent<Rigidbody>();

        // Lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;

        // Prevent player from tipping over by freezing rotation
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        HandleMouseLook();
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMouseLook()
    {
        // --- Mouse Look ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        // --- Movement ---
        float moveX = Input.GetAxis("Horizontal");  // A/D keys or left/right arrows
        float moveZ = Input.GetAxis("Vertical");    // W/S keys or up/down arrows

        // Move player in the direction they are facing
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    void HandleJump()
    {
        // --- Jump ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player is grounded
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}

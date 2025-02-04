using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float mouseSensitivity = 100f;
    public GameObject environment;  // Reference to the Environment object

    private float xRotation = 0f;
    private Transform playerCamera;
    private Rigidbody rb;
    private bool isGrounded;
    private int jumpCount = 0;  // Tracks the number of jumps

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
                jumpCount = 1;  // First jump
            }
            else if (jumpCount == 1)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpCount = 2;  // Second jump

                // Handle Environment rotation based on movement direction
                float moveX = Input.GetAxis("Horizontal");
                if (moveX > 0.1f)  // Moving right
                {
                    RotateEnvironment(-90f);
                }
                else if (moveX < -0.1f)  // Moving left
                {
                    RotateEnvironment(90f);
                }
            }
        }
    }

    void RotateEnvironment(float angle)
    {
        if (environment != null)
        {
            environment.transform.Rotate(0f, 0f, angle);
            UpdateEnvironmentPosition();
        }
    }

    void UpdateEnvironmentPosition()
    {
        if (environment == null) return;

        // Get the Z-axis rotation angle and round it to the nearest multiple of 90
        float zRotation = environment.transform.eulerAngles.z;
        zRotation = Mathf.Round(zRotation / 90f) * 90f;

        // Normalize the angle to the range [-180, 180]
        if (zRotation > 180f) zRotation -= 360f;

        // Update position based on the rotation
        Vector3 newPosition;
        if (Mathf.Approximately(zRotation, 0f) || Mathf.Approximately(zRotation, 360f) || Mathf.Approximately(zRotation, -360f))
        {
            newPosition = new Vector3(10f, 10f, 0f);
        }
        else if (Mathf.Approximately(zRotation, 180f) || Mathf.Approximately(zRotation, -180f))
        {
            newPosition = new Vector3(-10f, 10f, 0f);
        }
        else if (Mathf.Approximately(zRotation, 90f))
        {
            newPosition = new Vector3(0f, 20f, 0f);
        }
        else if (Mathf.Approximately(zRotation, -90f))
        {
            newPosition = new Vector3(0f, 0f, 0f);
        }
        else
        {
            newPosition = environment.transform.position;  // Fallback if an unhandled angle is encountered
        }

        // Apply the new position
        environment.transform.position = newPosition;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player is grounded
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;  // Reset jump count when grounded
        }
    }
}

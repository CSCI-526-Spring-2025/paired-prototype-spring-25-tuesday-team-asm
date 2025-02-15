using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float mouseSensitivity = 100f;
    public GameObject environment;  // Reference to the Environment object
    public float animationSpeed = 2f;  // Speed for position and rotation animations
    public Transform respawnPoint;

    private float xRotation = 0f;
    private Transform playerCamera;
    private Rigidbody rb;
    private bool isGrounded;
    private int jumpCount = 0;  // Tracks the number of jumps
    private Vector3 targetPosition;  // The target position for the environment object
    private Quaternion targetRotation;  // The target rotation for the environment object
    private float prevAnimSpeed = 2;

    public delegate void RespawnEvent();
    public static event RespawnEvent OnRespawn; // Event for respawn

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>().transform;
        rb = GetComponent<Rigidbody>();

        // Lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;

        // Prevent player from tipping over by freezing rotation
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Initialize target position and rotation
        if (environment != null)
        {
            targetPosition = environment.transform.position;
            targetRotation = environment.transform.rotation;
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleJump();

        // Smoothly move the environment object to the target position and rotation
        if (environment != null)
        {
            environment.transform.position = Vector3.Lerp(environment.transform.position, targetPosition, Time.deltaTime * animationSpeed);
            environment.transform.rotation = Quaternion.Lerp(environment.transform.rotation, targetRotation, Time.deltaTime * animationSpeed);
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
                jumpCount = 1;
            }
            else if (jumpCount == 1)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpCount = 2;

                float moveX = Input.GetAxis("Horizontal");
                if (moveX > 0.1f)
                {
                    RotateEnvironment(-90f);
                }
                else if (moveX < -0.1f)
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
            // Determine whether the player is facing forward or backward on the z-axis
            float forwardDirection = Vector3.Dot(transform.forward, Vector3.forward);

            // If facing backward (negative z-axis), invert the rotation direction
            if (forwardDirection < 0)
            {
                angle = -angle;
            }

            // Update target rotation by adding the angle
            targetRotation = Quaternion.Euler(environment.transform.eulerAngles.x, environment.transform.eulerAngles.y, environment.transform.eulerAngles.z + angle);
            UpdateEnvironmentPosition();
        }
    }

    void UpdateEnvironmentPosition()
    {
        if (environment == null) return;

        float zRotation = targetRotation.eulerAngles.z;
        zRotation = Mathf.Round(zRotation / 90f) * 90f;

        if (zRotation > 180f) zRotation -= 360f;

        if (Mathf.Approximately(zRotation, 0f) || Mathf.Approximately(zRotation, 360f) || Mathf.Approximately(zRotation, -360f))
        {
            targetPosition = new Vector3(10f, 10f, 0f);
        }
        else if (Mathf.Approximately(zRotation, 180f) || Mathf.Approximately(zRotation, -180f))
        {
            targetPosition = new Vector3(-10f, 10f, 0f);
        }
        else if (Mathf.Approximately(zRotation, 90f))
        {
            targetPosition = new Vector3(0f, 20f, 0f);
        }
        else if (Mathf.Approximately(zRotation, -90f))
        {
            targetPosition = new Vector3(0f, 0f, 0f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
            animationSpeed = prevAnimSpeed;
        }
    }

    public void Respawn()
    {
        targetRotation = Quaternion.Euler(0, 0, 0);
        prevAnimSpeed = animationSpeed;
        animationSpeed = 20;
        UpdateEnvironmentPosition();
        transform.position = respawnPoint.position;

        GameObject myObject = GameObject.Find("BottomFloor (2)");
        Platform platform = myObject.GetComponent<Platform>();
        platform.Reset();


        OnRespawn?.Invoke();
    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float mouseSensitivity = 100f;
    public float gravityShiftDuration = 0.5f;

    private float xRotation = 0f;
    private Transform playerCamera;
    private Rigidbody rb;
    private bool isGrounded;
    private bool canDoubleJump;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>().transform;
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
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
                Jump(Vector3.up);
                isGrounded = false;
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                Vector3 jumpDirection = GetRelativeJumpDirection();
                Jump(jumpDirection);
                ChangeGravity(jumpDirection);
                canDoubleJump = false;
            }
        }
    }

    void Jump(Vector3 direction)
    {
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction * jumpForce, ForceMode.Impulse);
    }

    Vector3 GetRelativeJumpDirection()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        return moveDirection != Vector3.zero ? moveDirection.normalized : transform.up;
    }

    void ChangeGravity(Vector3 jumpDirection)
    {
        // Determine the new gravity direction based on the jump direction and the player's current up vector
        Vector3 newGravityDirection = -jumpDirection.normalized;

        // Correct the gravity to ensure it pulls "down" to the new floor
        Physics.gravity = newGravityDirection * 9.81f;

        // Orient the player to face the new gravity direction
        OrientPlayerToNewGravity(newGravityDirection);
    }

    void OrientPlayerToNewGravity(Vector3 newGravityDirection)
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -newGravityDirection) * transform.rotation;
        StartCoroutine(SmoothRotation(targetRotation));
    }

    System.Collections.IEnumerator SmoothRotation(Quaternion targetRotation)
    {
        float timeElapsed = 0f;
        Quaternion initialRotation = transform.rotation;

        while (timeElapsed < gravityShiftDuration)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, timeElapsed / gravityShiftDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}

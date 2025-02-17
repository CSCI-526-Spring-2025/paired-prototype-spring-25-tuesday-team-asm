using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    
    public float moveSpeed = 10f;
    private Vector3 startPos;
    private Vector3 endPos;
    public Vector3 offset;

    private int moveFlag = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        endPos = startPos + offset;
        PlayerController.OnRespawn += ResetPlatform;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveFlag == 1)
        {

            transform.position = Vector3.MoveTowards(transform.position, endPos, moveSpeed * Time.deltaTime);
            
            //if close enough, end the movement
            if (Vector3.Distance(transform.position, endPos) < 0.01f)
            {
                moveFlag++;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {        
        if (collision.gameObject.CompareTag("Player"))
        {
            if (moveFlag == 0)
            {
                moveFlag++;
            }
            collision.transform.SetParent(transform); // Make player a child of the platform
        }
    }

    void OnCollisionExit(Collision collision)
    {
        //moveFlag = 0;
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null); // Remove parent when player leaves
        }
    }

    private void ResetPlatform()
    {
        float resetDelay = 0.25f; // Delay the reset to allow scene to rotate to original orientation first
        Invoke(nameof(DelayedReset), resetDelay);
    }

    private void DelayedReset()
    {
        transform.position = startPos;
        moveFlag = 0;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        PlayerController.OnRespawn -= ResetPlatform;
    }
}

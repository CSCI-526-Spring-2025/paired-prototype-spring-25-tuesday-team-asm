using UnityEngine;

public class Platform : MonoBehaviour
{
    private int moveFlag = 0;
    public float moveSpeed = 10f;
    public GameObject player;
    private Vector3 ogPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ogPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(moveFlag == 1){
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            //player.transform.position = player.transform.position + new Vector3(-0.06f, 0, 0);
        }
        // transform.Rotate(0,10,0);
        //transform.position = transform.position + new Vector3(-0.01f, 0, 0);
    }

    void OnCollisionEnter(Collision collision) 
    {   
        
        moveFlag = 1;
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform); // Make player a child of the platform
        }
    }
    void OnCollisionExit(Collision collision)
    {
        moveFlag = 0;
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null); // Remove parent when player leaves
        }
    }
    public void Reset(){
        transform.position = ogPos;
    }
}


using UnityEngine;
using System.Collections;

public class DisappearPlatform : MonoBehaviour
{
    public float fadeSpeed = 0.6f;
    public float fadeInterval = 0.1f;

    private Material thisMat;
    private Color originalColor;

    void Start()
    {
        thisMat = GetComponent<Renderer>().material;
        originalColor = thisMat.color;

        PlayerController.OnRespawn += ResetPlatform;
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FadeOut());
        }
    }
    private IEnumerator FadeOut()
    {
        
        Color matColor = thisMat.color;

        while (matColor.a > 0)
        {
            matColor.a -= fadeSpeed; // Reduce alpha
            thisMat.color = matColor; // Apply updated color
            yield return new WaitForSeconds(fadeInterval); // Wait a bit before next step
        }

        gameObject.SetActive(false); // Disable platform after fading out
    }

    private void ResetPlatform()
    {
        // Reset transparency
        thisMat.color = originalColor;
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        PlayerController.OnRespawn -= ResetPlatform;
    }
}

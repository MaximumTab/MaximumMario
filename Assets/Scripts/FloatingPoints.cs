using UnityEngine;
using System.Collections;

public class FloatingPoints : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float fadeDuration = 1f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FloatAndDisappear());
    }

    IEnumerator FloatAndDisappear()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + new Vector3(0, 3, 0);

        while (elapsedTime < fadeDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

}

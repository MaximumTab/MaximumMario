using UnityEngine;

public class DestroyOnContact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BigMario") || other.CompareTag("SmallMario"))
        {
            AudioManager.Instance.PlaySFX("Coin");
            FindAnyObjectByType<ScoreManager>().AddScore(200, Vector3.zero, false);
            Destroy(gameObject);
        }
    }
}
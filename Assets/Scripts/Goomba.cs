using UnityEngine;

public class Goomba : MonoBehaviour
{
    public float speed = 2f; // Movement speed
    private int direction = -1; // Starts moving left
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool hasActivated = false; // Tracks if Goomba has been activated
    private Camera mainCamera;

    public float stompForce = 5f; // Bounce force applied to player after stomping

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main; // Get the main camera reference
    }

    void Update()
    {
        if (hasActivated) return; // Don't check again if already activated

        // Get camera boundaries
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);

        // Check if Goomba is just outside the camera's view (slightly off-screen)
        if (screenPoint.x > -0.02f && screenPoint.x < 1.02f && screenPoint.y > 0 && screenPoint.y < 1)
        {
            hasActivated = true; // Start moving permanently
        }
    }

    void FixedUpdate()
    {
        if (!hasActivated) return;

        // Ensure Goomba keeps moving left or right while falling naturally
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // If the collision is from above (stomp)
                if (contact.normal.y < -0.5f)
                {
                    Stomped(other);
                    return;
                }
                // If the player collides from the side, they take damage (implement as needed)
                else if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
                {
                    PlayerHit(other);
                    return;
                }
            }
        }
        else
        {
            // Flip direction on wall collision
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
                {
                    Flip();
                    return;
                }
            }
        }
    }

    void Stomped(GameObject player)
    {
        // Add bounce effect to player
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompForce);
        }

        AudioManager.Instance.PlaySFX("Stomp");

        FindAnyObjectByType<ScoreManager>().AddScore(100);
        
        // Destroy Goomba
        Destroy(gameObject);
    }

    void PlayerHit(GameObject player)
    {
        // Implement player damage here (e.g., reduce health, trigger animation, etc.)
        Debug.Log("Player hit by Goomba! Take damage.");
    }

    void Flip()
    {
        direction *= -1;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}

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
    public Sprite stompedSprite; // Stomped sprite
    private bool isStomped = false; // Prevents multiple triggers

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main; 
    }

    void Update()
    {
        if (hasActivated) return;

        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);

        // Check if Goomba is just outside the camera's view (slightly off-screen)
        if (screenPoint.x > -0.02f && screenPoint.x < 1.02f && screenPoint.y > 0 && screenPoint.y < 1)
        {
            hasActivated = true; 
        }
    }

    void FixedUpdate()
    {
        if (!hasActivated || isStomped) return;

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("BigMario") || other.CompareTag("SmallMario"))
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
        if (isStomped) return; // Prevent multiple triggers
        isStomped = true;

        // Add bounce effect to player
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompForce);
        }

        AudioManager.Instance.PlaySFX("Stomp");

        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        int stompScore = scoreManager.GetStompScore();
        scoreManager.AddScore(stompScore, transform.position);
        scoreManager.IncrementStompCount(); // Increase streak

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Change sprite to stomped version
        if (stompedSprite != null)
        {
            spriteRenderer.sprite = stompedSprite;
        }

        // Disable physics interaction
        rb.simulated = false;

        Invoke("DestroyGoomba", 0.2f); 
    }


    void DestroyGoomba()
    {
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
    }

    public void KillGoomba()
    {
        if (isStomped) return;
        isStomped = true;

        spriteRenderer.flipY = true;

        rb.linearVelocity = new Vector2(0f, 5f);
        rb.angularVelocity = 360f; 

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer, true);
            col.enabled = false; 
        }

        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        scoreManager.AddScore(100, transform.position);

        Destroy(gameObject, 3f);
    }



}
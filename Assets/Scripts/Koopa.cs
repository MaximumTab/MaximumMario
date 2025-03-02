using UnityEngine;

public class Koopa : MonoBehaviour
{
    public float speed = 2f; // Walking speed
    private int direction = -1; // Starts moving left
    private Rigidbody2D rb;
    private bool hasActivated = false;
    private Camera mainCamera;

    public float stompForce = 5f; // Bounce force applied to player after stomping
    public GameObject shellPrefab; // Prefab for the shell version
    public Sprite deathSprite; // New sprite for death animation
    private bool isDead = false; // Prevents multiple triggers

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (hasActivated) return;

        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);

        if (screenPoint.x > -0.02f && screenPoint.x < 1.02f && screenPoint.y > 0 && screenPoint.y < 1)
        {
            hasActivated = true;
        }
    }

    void FixedUpdate()
    {
        if (!hasActivated || isDead) return;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("BigMario") || other.CompareTag("SmallMario"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f) // Stomped
                {
                    Stomped(other);
                    return;
                }
                else if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y)) // Player hit from side
                {
                    PlayerHit(other);
                    return;
                }
            }
        }
        else // Flip direction on wall collision
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
                {
                    Flip();
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    return;
                }
            }
        }
    }

    void Stomped(GameObject player)
    {
        if (isDead) return; // Prevent multiple activations

        // Spawn Shell Koopa
        GameObject shell = Instantiate(shellPrefab, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX("Kick");
        
        // Bounce the player upwards
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompForce);
        }

        AudioManager.Instance.PlaySFX("Stomp");

        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        int stompScore = scoreManager.GetStompScore();
        scoreManager.AddScore(stompScore, transform.position);
        scoreManager.IncrementStompCount();

        Destroy(gameObject); // Destroy Alive Koopa
    }

    public void KillKoopa()
    {
        if (isDead) return;
        isDead = true;
        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().enabled = false;
        }
        if (deathSprite != null)
        {
            spriteRenderer.sprite = deathSprite; // Change to death sprite
        }

        GetComponent<SpriteRenderer>().flipY = true; // Flip to indicate death
        rb.linearVelocity = new Vector2(0f, 5f);
        rb.angularVelocity = 360f;

        int ignoreLayer = LayerMask.NameToLayer("IGNOREALL");
        if (ignoreLayer != -1)
        {
            gameObject.layer = ignoreLayer;
        }
        else
        {
            Debug.LogWarning("Layer 'IgnoreEverything' not found! Make sure you created it in Unity.");
        }

        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        scoreManager.AddScore(100, transform.position);

        Destroy(gameObject, 3f);
    }

    void PlayerHit(GameObject player)
    {
        Debug.Log("Player hit by Koopa! Take damage.");
    }

    void Flip()
    {
        direction *= -1;
    }
}

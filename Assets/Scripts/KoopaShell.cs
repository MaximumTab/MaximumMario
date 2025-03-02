using UnityEngine;
using System.Collections;

public class KoopaShell : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isKicked = false;
    private int direction = 0;
    public float shellSpeed = 6f;
    public float wakeUpTime = 4f;
    public float respawnTime = 5.5f;
    public float playerBounceForce = 5f;

    public Sprite wakeUpSprite;
    public Sprite originalSprite;
    public GameObject aliveKoopaPrefab;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool isRespawning = false;
    private Coroutine respawnCoroutine = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sharedMaterial = new PhysicsMaterial2D { friction = 0f, bounciness = 0f };

        rb.linearVelocity = Vector2.zero;

        originalSprite = spriteRenderer.sprite;

        respawnCoroutine = StartCoroutine(WakeUpAndRespawnSequence());
    }

    void FixedUpdate()
    {
        if (isKicked)
        {
            rb.linearVelocity = new Vector2(direction * shellSpeed, rb.linearVelocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("BigMario") || other.CompareTag("SmallMario"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    Stomped(other);
                    return;
                }
                else if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
                {
                    if (!isKicked)
                    {
                        KickShell(other, contact.normal.x > 0 ? -1 : 1);
                    }
                    else
                    {
                        PlayerHit(other);
                    }
                    return;
                }
            }
        }
        else if (isKicked)
        {
            if (other.CompareTag("Enemy"))
            {
                Goomba goomba = other.GetComponent<Goomba>();
                if (goomba != null)
                {
                    goomba.KillGoomba();
                }
                else
                {
                    Destroy(other);
                }
            }
            else
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
                    {
                        ReverseDirection();
                        return;
                    }
                }
            }
        }
        else if (other.CompareTag("Enemy")) // Prevent Goombas from pushing shells
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Stomped(GameObject player)
    {
        if (isKicked)
        {
            StopShell();
        }
        else
        {
            KickShell(player);
        }

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, playerBounceForce);
        }

        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        int stompScore = scoreManager.GetStompScore();
        scoreManager.AddScore(stompScore, transform.position);
        scoreManager.IncrementStompCount();
    }

    void KickShell(GameObject player, int kickDirection = 0)
    {
        if (isKicked) return;

        AudioManager.Instance.PlaySFX("Kick");
        isKicked = true;
        direction = kickDirection != 0 ? kickDirection : (player.transform.position.x > transform.position.x ? 1 : -1);
        rb.linearVelocity = new Vector2(direction * shellSpeed, 0);

        spriteRenderer.sprite = originalSprite;

        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine); // Stop respawn countdown if kicked
            respawnCoroutine = null;
        }

        Debug.Log("Shell kicked!");
    }

    void StopShell()
    {
        isKicked = false;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Shell stopped by player.");

        if (respawnCoroutine == null)
        {
            respawnCoroutine = StartCoroutine(WakeUpAndRespawnSequence()); // Restart respawn timer
        }
    }

    void ReverseDirection()
    {
        AudioManager.Instance.PlaySFX("Bump");

        direction *= -1;
        rb.linearVelocity = new Vector2(direction * shellSpeed, rb.linearVelocity.y);

        rb.AddForce(new Vector2(direction * 2f, 0), ForceMode2D.Impulse);

        Debug.Log("Shell reversed direction. New velocity: " + rb.linearVelocity);
    }

    IEnumerator WakeUpAndRespawnSequence()
    {
        yield return new WaitForSeconds(wakeUpTime);

        if (!isKicked)
        {
            ShowWakeUp();
            yield return new WaitForSeconds(respawnTime - wakeUpTime);
            Respawn();
        }
    }

    void ShowWakeUp()
    {
        if (!isKicked)
        {
            spriteRenderer.sprite = wakeUpSprite;
        }
    }

    void Respawn()
    {
        if (isKicked) return;

        Instantiate(aliveKoopaPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void PlayerHit(GameObject player)
    {
        Debug.Log("Player hit by moving Koopa Shell! Take damage.");
    }
}

using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 5f;
    public float bounceForce = 3f;
    public int maxBounces = 3;
    public Animator animator;

    private int bounceCount = 0;
    private Rigidbody2D rb;
    private bool hasExploded = false; // Prevents multiple triggers

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed; 
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return; // Prevents multiple explosions

        if (collision.CompareTag("Enemy"))
        {
            HandleEnemyHit(collision.gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            HandleGroundBounce();
        }
    }

    void HandleEnemyHit(GameObject enemy)
    {
        if (hasExploded) return;
        hasExploded = true;

        if (enemy.TryGetComponent(out Goomba goomba))
        {
            goomba.KillGoomba();
        }
        else if (enemy.TryGetComponent(out Koopa koopa))
        {
            koopa.KillKoopa();
        }
        else
        {
            Destroy(enemy); // Destroy other enemies if necessary
        }

        Explode();
    }

    void HandleGroundBounce()
    {
        if (bounceCount < maxBounces)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
            bounceCount++;
        }
        else
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;
        animator.SetBool("hasColided", true);
        rb.simulated = false; // Disable physics
        Destroy(gameObject, 0.2f); // Delay destruction for animation
    }
}

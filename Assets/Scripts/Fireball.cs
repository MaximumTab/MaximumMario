using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 5f;
    public float bounceForce = 3f;
    public int maxBounces = 3;
    public float minVelocityThreshold = 0.5f; // Threshold to destroy the fireball
    public Animator animator;

    private int bounceCount = 0;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed; // Use velocity instead of linearVelocity
    }

    void Update()
    {
        // Destroy fireball if its velocity magnitude is too low
        if (rb.linearVelocity.magnitude < minVelocityThreshold)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject); 
            Destroy(gameObject); 
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            if (bounceCount < maxBounces)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce); 
                bounceCount++;
            }
            else
            {
                Destroy(gameObject); 
            }
        }
    }
}

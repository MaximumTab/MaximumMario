using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 5f;
    public float bounceForce = 3f;
    public int maxBounces = 3; 

    private int bounceCount = 0;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed; 
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject); 
            Destroy(gameObject); 
        }
        else if (collision.CompareTag("Ground"))
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

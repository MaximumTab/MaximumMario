using UnityEngine;

public class Goomba : MonoBehaviour
{
    public float speed = 2f; // Movement speed
    private int direction = -1; // Starts moving left
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool hasActivated = false; // Tracks if Goomba has been activated
    private Camera mainCamera;

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

        // Check if Goomba is within camera's view
        if (screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
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
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
            {
                Flip();
                return;
            }
        }
    }

    void Flip()
    {
        direction *= -1;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}

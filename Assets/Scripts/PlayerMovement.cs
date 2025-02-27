using UnityEditor.Searcher;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool dropped=true;
    private bool isGrounded;

    [SerializeField] private float speedClamp=50;
    [SerializeField] private float accel=1;
    [SerializeField] private float jumpForce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        DropCheck();
        Jump();
        Movement();
    }

    void Movement()
    {
        rb.AddForceX(Input.GetAxis("Horizontal") * accel);
    }

    void Sprint()
    {
        
    }
    void Jump()
    {
        if (Input.GetAxis("Vertical") > 0 && isGrounded)
        {
            rb.AddForceY(jumpForce,ForceMode2D.Impulse);
            dropped = false;
        }
    }

    void DropCheck()
    {
        if (rb.linearVelocityY <0f)
        {
            dropped = true;
        }

        if (dropped&&rb.linearVelocityY==0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}

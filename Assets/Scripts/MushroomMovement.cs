using UnityEngine;

public class MushroomMovement : MonoBehaviour
{

    /*
    [SerializeField] private float movementSpeed;
    private Rigidbody2D rb;
    private int directionX = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(directionX * movementSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("I have hit " + collision.gameObject.name);

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("it gets into here somehow");
                return;     //items are unaffected by enemies

            }

            directionX *= -1;
            Debug.Log("direction = " + directionX);
            return;
        }


        //destroy on player
        if (collision.gameObject.CompareTag("Big Mario") || collision.gameObject.CompareTag("Small Mario"))
        {
            Debug.Log("colided with player");
            Destroy(gameObject);
        }
      

    }
  */
    

    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float moveDuration = 5f; // Time to move in one direction

    private Rigidbody2D rb;
    private float directionX = 1f;  // Start by moving to the right
    private bool isGrounded = false;
    private float moveTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(directionX * movementSpeed, rb.linearVelocity.y);
        }

        // Count down the timer
        moveTimer += Time.deltaTime;

        // Change direction every moveDuration seconds
        if (moveTimer >= moveDuration)
        {
            directionX *= -1f;  // Flip direction
            moveTimer = 0f;     // Reset timer
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if grounded and handle the state
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
            }
        }

        if (collision.gameObject.CompareTag("BigMario") || collision.gameObject.CompareTag("SmallMario"))
        {
            // 1) Grab the PlayerController component on this collided object
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc != null)
            {
                // 2) Call the method on that instance
                pc.UpdatePlayerLevel(PlayerController.PlayerLevel.Level2_Big);
                Debug.Log("Collided with player -> changed to Level2_Big");

                pc.MarioAnim.SetBool("Big",true);
                
                // 3) Destroy this object (e.g., the item)
                Destroy(gameObject);
            }
        }

    }

    

}

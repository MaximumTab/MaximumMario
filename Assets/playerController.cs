using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;         // Walking speed
    public float sprintSpeed = 8f;      // Sprinting speed
    public float acceleration = 10f;    // Acceleration rate
    public float deceleration = 15f;    // Deceleration rate
    public float jumpForce = 10f;       // Jump force
    public float gravityScale = 2f;     // Gravity scale

    private Rigidbody2D rb;
    private BoxCollider2D playerCollider;

    private float targetSpeed;          // Desired horizontal velocity
    private float currentSpeed;         // Current horizontal velocity
    private bool isSprinting;           // Whether sprint key is held
    private bool isGrounded;            // Whether player is on the ground

    // Movement input stack (for A/D priority)
    private Stack<KeyCode> movementStack = new Stack<KeyCode>();

    [Header("Layers & Distances")]
    public LayerMask groundLayer;       // Layer for ground checks
    public float groundCheckDistance = 0.05f;  // Distance for ground checking

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        // Basic Rigidbody setup
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        ProcessInput();

        // Jump if grounded
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        bool previouslyGrounded = isGrounded;
        isGrounded = IsGrounded();  // Check ground each frame

        MovePlayer();
    }

    private void ProcessInput()
    {
        // Handle left/right (A/D) via movement stack
        HandleMovementKey(KeyCode.A, -1);
        HandleMovementKey(KeyCode.D, 1);

        // Check for sprint input (shift keys)
        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentMaxSpeed = isSprinting ? sprintSpeed : maxSpeed;

        int movementDirection = GetMovementDirection();

        if (movementDirection != 0)
        {
            // If reversing direction, decelerate to zero before going other way
            if (Mathf.Sign(movementDirection) != Mathf.Sign(rb.linearVelocity.x) && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            {
                targetSpeed = 0f;
            }
            else
            {
                targetSpeed = movementDirection * currentMaxSpeed;
            }
        }
        else
        {
            targetSpeed = 0f;
        }
    }

    private void HandleMovementKey(KeyCode key, int direction)
    {
        if (Input.GetKeyDown(key))
        {
            // Move this key to top of stack
            movementStack = new Stack<KeyCode>(new Stack<KeyCode>(movementStack).ToArray());
            if (!movementStack.Contains(key))
            {
                movementStack.Push(key);
            }
        }
        if (Input.GetKeyUp(key))
        {
            // Remove the key from the stack
            Stack<KeyCode> tempStack = new Stack<KeyCode>();
            while (movementStack.Count > 0)
            {
                KeyCode currentKey = movementStack.Pop();
                if (currentKey != key)
                {
                    tempStack.Push(currentKey);
                }
            }
            movementStack = new Stack<KeyCode>(tempStack);
        }
    }

    private int GetMovementDirection()
    {
        if (movementStack.Count > 0)
        {
            KeyCode lastKey = movementStack.Peek(); // The most recent pressed key
            return (lastKey == KeyCode.A) ? -1 : (lastKey == KeyCode.D ? 1 : 0);
        }
        return 0;
    }

    private void MovePlayer()
    {
        // If no target speed (no movement input), decelerate to 0 if grounded
        if (targetSpeed == 0 && isGrounded)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Accelerate toward target speed
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    private bool IsGrounded()
    {
        // Use a BoxCast slightly below the player
        Vector2 origin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
        Vector2 size = new Vector2(playerCollider.bounds.size.x * 0.9f, 0.1f);
        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, groundCheckDistance, groundLayer);

        return (hit.collider != null);
    }
}

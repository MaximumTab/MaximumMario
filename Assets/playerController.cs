using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 5f; // Walking speed
    public float sprintSpeed = 8f; // Sprinting speed
    public float acceleration = 10f; // Acceleration rate
    public float deceleration = 15f; // Deceleration rate
    public float jumpForce = 10f;
    public float gravityScale = 2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool wasTouchingWallMidAir; // Detects if player hit a wall mid-air
    private BoxCollider2D playerCollider;
    private float targetSpeed;
    private float currentSpeed;
    private bool isSprinting;

    private Stack<KeyCode> movementStack = new Stack<KeyCode>(); // Tracks pressed keys in order

    public LayerMask groundLayer;
    public LayerMask wallLayer; // Separate wall layer to detect collisions
    public float groundCheckDistance = 0.05f;
    public float wallCheckDistance = 0.1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        ProcessInput();

        // Jump only if grounded
       if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        bool previouslyGrounded = isGrounded;
        isGrounded = IsGrounded();

        if (!previouslyGrounded && isGrounded && wasTouchingWallMidAir)
        {
            // Fix: When landing after a wall bump, reset deceleration delay
            wasTouchingWallMidAir = false;
            targetSpeed = GetMovementDirection() * (isSprinting ? sprintSpeed : maxSpeed);
        }

        if (!isGrounded && IsTouchingWall())
        {
            // If player bumps into a wall while in mid-air, flag it
            wasTouchingWallMidAir = true;
        }

        MovePlayer();
    }

    private void ProcessInput()
    {
        // Handle movement key presses
        HandleMovementKey(KeyCode.A, -1);
        HandleMovementKey(KeyCode.D, 1);

        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); // Sprint check

        float currentMaxSpeed = isSprinting ? sprintSpeed : maxSpeed; // Sprint speed adjustment

        int movementDirection = GetMovementDirection();

        if (movementDirection != 0)
        {
            if (wasTouchingWallMidAir)
            {
                // Fix: Ignore deceleration delay after landing from a wall collision
                targetSpeed = movementDirection * currentMaxSpeed;
            }
            else if (Mathf.Sign(movementDirection) != Mathf.Sign(rb.linearVelocity.x) && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            {
                targetSpeed = 0f; // Decelerate to zero before switching
            }
            else
            {
                targetSpeed = movementDirection * currentMaxSpeed; // Normal movement logic
            }
        }
        else
        {
            targetSpeed = 0f; // Stop movement only if no keys are held
        }
    }

    private void HandleMovementKey(KeyCode key, int direction)
    {
        if (Input.GetKeyDown(key))
        {
            // Remove key from stack if it exists and push it to the top
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
            KeyCode lastKey = movementStack.Peek(); // Get the most recent key
            return lastKey == KeyCode.A ? -1 : (lastKey == KeyCode.D ? 1 : 0);
        }
        return 0;
    }

    private void MovePlayer()
    {
        if (targetSpeed == 0 && isGrounded)
        {
            // Decelerate to zero when no input or stopping
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Accelerate towards max speed or sprint speed when moving
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    private bool IsGrounded()
    {
        Vector2 origin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
        Vector2 size = new Vector2(playerCollider.bounds.size.x * 0.9f, 0.1f);
        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, groundCheckDistance, groundLayer);

        return hit.collider != null;
    }

    private bool IsTouchingWall()
    {
        Vector2 origin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.center.y);
        Vector2 size = new Vector2(playerCollider.bounds.size.x * 0.9f, playerCollider.bounds.size.y);
        RaycastHit2D leftCheck = Physics2D.BoxCast(origin, size, 0f, Vector2.left, wallCheckDistance, wallLayer);
        RaycastHit2D rightCheck = Physics2D.BoxCast(origin, size, 0f, Vector2.right, wallCheckDistance, wallLayer);

        return leftCheck.collider != null || rightCheck.collider != null;
    }
}

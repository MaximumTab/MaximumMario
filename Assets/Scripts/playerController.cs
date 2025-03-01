using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float sprintSpeed = 8f;
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float jumpForce = 10f;
    public float gravityScale = 2f;

    private Rigidbody2D rb;
    private BoxCollider2D playerCollider;

    private float targetSpeed;
    private float currentSpeed;
    private bool isSprinting;
    private bool isGrounded;

    // Movement input stack (for A/D priority)
    private Stack<KeyCode> movementStack = new Stack<KeyCode>();

    [Header("Layers & Distances")]
    [Tooltip("Assign ground and pipe layers together if you want to jump on pipes.")]
    public LayerMask groundAndPipeLayers;   // ← Combine normal ground + special pipe layers here
    public float groundCheckDistance = 0.05f;

    [Header("Pipe Settings (Downward Cutscene)")]
    public LayerMask pipeLayer;
    public float pipeCheckDistance = 0.05f;
    public float pipeMoveSpeed = 2f;
    public float pipeMoveDuration = 2f;
    public Transform pipeDestination;

    private bool isInPipeCutscene = false;

    [Header("Cutscene Collision & Rendering")]
    public SpriteRenderer playerSprite;      
    public int behindSortingOrder = -10;     
    private int originalSortingOrder;        

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.gravityScale = gravityScale;

        if (playerSprite != null)
        {
            originalSortingOrder = playerSprite.sortingOrder;
        }
    }

    private void Update()
    {
        if (!isInPipeCutscene)
        {
            ProcessInput();

            // Jump if grounded
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

            // Check for pipe below when pressing S
            if (Input.GetKeyDown(KeyCode.S))
            {
                CheckPipeBelow();
            }
        }
    }

    private void FixedUpdate()
    {
        bool previouslyGrounded = isGrounded;
        isGrounded = IsGrounded();  // Now includes ground + pipe

        if (!isInPipeCutscene)
        {
            MovePlayer();
        }
    }

    private void ProcessInput()
    {
        // Handle left/right (A/D)
        HandleMovementKey(KeyCode.A, -1);
        HandleMovementKey(KeyCode.D, 1);

        // Check for sprint
        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentMaxSpeed = isSprinting ? sprintSpeed : maxSpeed;

        int movementDirection = GetMovementDirection();
        if (movementDirection != 0)
        {
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
            // Move this key to the top of the stack
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
            KeyCode lastKey = movementStack.Peek();
            return (lastKey == KeyCode.A) ? -1 : (lastKey == KeyCode.D ? 1 : 0);
        }
        return 0;
    }

    private void MovePlayer()
    {
        if (targetSpeed == 0 && isGrounded)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    
    private bool IsGrounded()
    {
        Vector2 origin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
        Vector2 size = new Vector2(playerCollider.bounds.size.x * 0.9f, 0.1f);

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            size,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundAndPipeLayers  // ← Combines ground + pipe
        );
        return (hit.collider != null);
    }

    // -------------------- PIPE CUTSCENE LOGIC --------------------
    private void CheckPipeBelow()
    {
        // BoxCast slightly below to detect a downward pipe
        Vector2 bottomOrigin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
        Vector2 size = new Vector2(playerCollider.bounds.size.x * 0.9f, 0.1f);

        RaycastHit2D hit = Physics2D.BoxCast(
            bottomOrigin,
            size,
            0f,
            Vector2.down,
            pipeCheckDistance,
            pipeLayer
        );

        if (hit.collider != null)
        {
            Debug.Log("Pipe below detected! Starting downward cutscene...");
            StartCoroutine(PipeCutscene());
        }
    }

    private IEnumerator PipeCutscene()
    {
        isInPipeCutscene = true;

        // Turn off collisions & move behind everything
        playerCollider.enabled = false;
        if (playerSprite != null) playerSprite.sortingOrder = -10;

        float elapsed = 0f;
        while (elapsed < pipeMoveDuration)
        {
            transform.position += Vector3.down * (pipeMoveSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (pipeDestination != null)
        {
            transform.position = pipeDestination.position;
        }

         // Switch the camera to underground after the pipe cutscene finishes
        FindObjectOfType<SideScrolling>().SetUnderground(true);


        // Restore collisions & sprite order
        playerCollider.enabled = true;
        if (playerSprite != null) playerSprite.sortingOrder = originalSortingOrder;

        isInPipeCutscene = false;
        Debug.Log("Pipe cutscene finished!");
    }
}

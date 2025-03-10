using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float sprintSpeed = 8f;
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float jumpForce = 10f;
    

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
    public LayerMask groundAndPipeLayers;   
    public float groundCheckDistance = 0.05f;

    [Header("Pipe Settings (Downward Cutscene)")]
    public LayerMask pipeLayer;             // For downward pipe
    public float pipeCheckDistance = 0.05f;
    public float pipeMoveSpeed = 2f;
    public float pipeMoveDuration = 2f;
    public Transform pipeDestination;

    [Header("Pipe Settings (Rightward Cutscene)")]
    public LayerMask pipeLayerRight;        // For the rightward pipe (e.g. "InteractPipe2" layer)
    public float pipeCheckDistanceRight = 0.05f;  // Distance for boxcast to the right
    public float pipeMoveDurationRight = 2f;      // Duration of moving horizontally
    public Transform pipeDestinationRight;        // Separate teleporter for right pipe

    private bool isInPipeCutscene = false;

    [Header("Cutscene Collision & Rendering")]
    public SpriteRenderer playerSprite;      
    public int behindSortingOrder = -10;     
    private int originalSortingOrder;

    [Header("Player Levels")]
    public PlayerLevel currentLevel = PlayerLevel.Level1_Small;

    [Header("Fire Shooter")]
    public GameObject fireShooter;

    public enum PlayerLevel
    {
        Level1_Small,
        Level2_Big,
        Level3_Fire,
        Level4_Star
    }

    // Not used for scale logic currently, but you can keep them for reference
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private PlayerLevel lastLevel;
    private bool isInvincible = false;
    private Coroutine invincibleRoutine;
    public Animator MarioAnim;

    private ScoreManager scoreManager; // Add reference to ScoreManager

    private void Start()
    {
        MarioAnim=gameObject.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        originalColliderSize = playerCollider.size;
        originalColliderOffset = playerCollider.offset;
       

        if (playerSprite != null)
        {
            originalSortingOrder = playerSprite.sortingOrder;
        }

        // Store original collider data
        originalColliderSize = playerCollider.size;
        originalColliderOffset = playerCollider.offset;

        // Initialize
        UpdatePlayerForm(currentLevel);
        lastLevel = currentLevel;

        scoreManager = FindObjectOfType<ScoreManager>(); // Initialize ScoreManager reference
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
                AudioManager.Instance.PlaySFX("JumpSmall");
                MarioAnim.SetBool("Jumped",true);
            }

            if (rb.linearVelocityY <=0.1f && isGrounded)
            {
                MarioAnim.SetBool("Jumped",false);
            }

            // Press S to check for downward pipe
            if (Input.GetKeyDown(KeyCode.S))
            {
                CheckPipeBelow();
            }

            // Press E to check for rightward pipe
            if (Input.GetKeyDown(KeyCode.D))
            {
                CheckPipeRight();
            }
        }

        // If changed in Inspector or code, apply updates
        if (currentLevel != lastLevel)
        {
            UpdatePlayerLevel(currentLevel);
            lastLevel = currentLevel;
        }
    }

    private void FixedUpdate()
    {
        bool previouslyGrounded = isGrounded;
        isGrounded = IsGrounded();

        if (isGrounded && !previouslyGrounded)
        {
            scoreManager.ResetStompCount(); // Reset stomp count when player touches the ground
        }

        if (!isInPipeCutscene)
            MovePlayer();
    }



    private void ProcessInput()
    {
        HandleMovementKey(KeyCode.A, -1);
        HandleMovementKey(KeyCode.D, 1);

        // Check for sprint
        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentMaxSpeed = isSprinting ? sprintSpeed : maxSpeed;

        int movementDirection = GetMovementDirection();
        MarioAnim.SetFloat("InputX",movementDirection);
        MarioAnim.SetFloat("VelocityX",rb.linearVelocityX);
        if (movementDirection != 0)
        {
            // Decelerate to zero if reversing direction
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
        MarioAnim.SetFloat("Speed",Math.Abs(currentSpeed/5));
    }

    private void HandleMovementKey(KeyCode key, int direction)
    {
        if (Input.GetKeyDown(key))
        {
            movementStack = new Stack<KeyCode>(new Stack<KeyCode>(movementStack).ToArray());
            if (!movementStack.Contains(key))
            {
                movementStack.Push(key);
            }
        }
        if (Input.GetKeyUp(key))
        {
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
            groundAndPipeLayers
        );
        return (hit.collider != null);
    }

    // ------------- PIPE CUTSCENE LOGIC (DOWN) -------------
    private void CheckPipeBelow()
    {
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
            StartCoroutine(PipeCutsceneDown());
            AudioManager.Instance.PlaySFX("Pipe");
        }
    }

    [System.Obsolete]
    private IEnumerator PipeCutsceneDown()
    {
        isInPipeCutscene = true;

        // 1) Disable collisions & reorder sprite
        playerCollider.enabled = false;
        if (playerSprite != null) playerSprite.sortingOrder = behindSortingOrder;

        // 2) Freeze gravity & velocity
        
        Vector2 originalVelocity = rb.velocity;
        
        rb.velocity = Vector2.zero;

        float elapsed = 0f;
        while (elapsed < pipeMoveDuration)
        {
            // Only the pipe movement is applied – no gravity or normal movement
            transform.position += Vector3.down * (pipeMoveSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (pipeDestination != null)
        {
            transform.position = pipeDestination.position;
        }

        FindObjectOfType<SideScrolling>().SetUnderground(true);

        // 3) Restore collisions & sprite
        playerCollider.enabled = true;
        if (playerSprite != null) playerSprite.sortingOrder = originalSortingOrder;

        // 4) Restore gravity & velocity
        rb.velocity = originalVelocity; // Optionally restore old velocity or just keep zero

        isInPipeCutscene = false;
        Debug.Log("Pipe (down) cutscene finished!");
    }

    // ---------------- PIPE CUTSCENE LOGIC (RIGHT) ----------------
    private void CheckPipeRight()
    {
        Vector2 rightOrigin = new Vector2(playerCollider.bounds.max.x, playerCollider.bounds.center.y);
        Vector2 size = new Vector2(playerCollider.bounds.size.x * 0.9f, playerCollider.bounds.size.y);

        RaycastHit2D hit = Physics2D.BoxCast(
            rightOrigin,
            size,
            0f,
            Vector2.right,
            pipeCheckDistanceRight,
            pipeLayerRight
        );

        if (hit.collider != null)
        {
            Debug.Log("Pipe to the right detected! Starting rightward cutscene...");
            StartCoroutine(PipeCutsceneRight());
            AudioManager.Instance.PlaySFX("Pipe");
        }
    }

    [System.Obsolete]
    private IEnumerator PipeCutsceneRight()
    {
        isInPipeCutscene = true;

        // 1) Disable collisions & reorder sprite
        playerCollider.enabled = false;
        if (playerSprite != null) playerSprite.sortingOrder = behindSortingOrder;

        // 2) Freeze gravity & velocity
       
        Vector2 originalVelocity = rb.velocity;
      
        rb.velocity = Vector2.zero;

        float elapsed = 0f;
        while (elapsed < pipeMoveDurationRight)
        {
            // Only the pipe movement is applied
            transform.position += Vector3.right * (pipeMoveSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (pipeDestinationRight != null)
        {
            transform.position = pipeDestinationRight.position;
        }

        FindObjectOfType<SideScrolling>().SetRightwardPipeTransition(pipeDestinationRight);

        // 3) Restore collisions & sprite
        playerCollider.enabled = true;
        if (playerSprite != null) playerSprite.sortingOrder = originalSortingOrder;

        // 4) Restore gravity & velocity
       
        rb.velocity = originalVelocity;

        isInPipeCutscene = false;
        Debug.Log("Pipe (right) cutscene finished!");
    }


    // ------------- PLAYER LEVEL LOGIC -------------
    public void UpdatePlayerLevel(PlayerLevel newLevel)
    {
        if (newLevel == PlayerLevel.Level3_Fire)
        {
            currentLevel = PlayerLevel.Level3_Fire;
            gameObject.tag = "BigMario";
            if (fireShooter) fireShooter.SetActive(true);
        }
        else if (newLevel == PlayerLevel.Level2_Big)
        {
            currentLevel = PlayerLevel.Level2_Big;
            gameObject.tag = "BigMario";
            if (fireShooter) fireShooter.SetActive(false);
        }
        else
        {
            currentLevel = PlayerLevel.Level1_Small;
            gameObject.tag = "SmallMario";
            if (fireShooter) fireShooter.SetActive(false);
        }
        Debug.Log("Player level updated to: " + currentLevel + ", tag = " + gameObject.tag);
    }

    private void ShrinkHitboxToOriginal(BoxCollider2D col)
    {
        // Restore the original values stored in Start()
        col.size = originalColliderSize;
        col.offset = originalColliderOffset;
    }

    public void UpdatePlayerForm(PlayerLevel newLevel)
    {
        UpdatePlayerLevel(newLevel);
    }

    private void DowngradeLevel()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        // If we are already in invincibility mode, do nothing:
        if (isInvincible) return;

        // If Star, ignore damage
        if (currentLevel == PlayerLevel.Level4_Star)
        {
            Debug.Log("Star Mario is invincible! No downgrade.");
            return; // Do nothing
        }
        else if (currentLevel == PlayerLevel.Level3_Fire)
        {
            MarioAnim.SetBool("Fire",false);
            UpdatePlayerLevel(PlayerLevel.Level2_Big);
        }
        else if (currentLevel == PlayerLevel.Level2_Big)
        {
            MarioAnim.SetBool("Big",false);
            UpdatePlayerLevel(PlayerLevel.Level1_Small);
            if (col != null) ShrinkHitboxToOriginal(col);
        }
        else // Already Level1_Small => die or reset scene
        {
            Debug.Log("Mario died! Reloading scene...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            MarioAnim.Play("Death");
            return;
        }

        // If we successfully downgraded from Fire->Big or Big->Small,
        // start the invincibility flicker
        if (invincibleRoutine != null) StopCoroutine(invincibleRoutine);
        invincibleRoutine = StartCoroutine(InvincibilityFlicker());
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            bool shouldDowngrade = false;

            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y <= 0.5f)
                {
                    shouldDowngrade = true;
                    break;
                }
            }

            if (shouldDowngrade)
            {
                Debug.Log("Hit enemy from side/bottom → Downgrade!");
                DowngradeLevel(); 
            }
            else
            {
                Debug.Log("Hit enemy from above → no downgrade (maybe stomp enemy).");
            }
        }
    }

   private List<Collider2D> currentlyIgnored = new List<Collider2D>();

    private void SetEnemyCollisionsIgnored(bool ignore)
    {
        if (ignore)
        {
            // Find all enemies
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                Collider2D enemyCol = enemy.GetComponent<Collider2D>();
                if (enemyCol != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, enemyCol, true);
                    currentlyIgnored.Add(enemyCol);
                }
            }
        }
        else
        {
            // Re-enable collisions only for those we actually ignored
            foreach (Collider2D col in currentlyIgnored)
            {
                if (col != null) // might have been destroyed
                {
                    Physics2D.IgnoreCollision(playerCollider, col, false);
                }
            }
            currentlyIgnored.Clear();
        }
    }


    private IEnumerator InvincibilityFlicker()
    {
        isInvincible = true;

        // Random time between 3 and 4 seconds
        float randomDuration = Random.Range(3f, 4f);
        float elapsed = 0f;

        // 1) Ignore collisions with enemies
        SetEnemyCollisionsIgnored(true);

        // 2) Flicker sprite
        SpriteRenderer sr = playerSprite;
        if (sr == null) yield break;

        while (elapsed < randomDuration)
        {
            sr.enabled = !sr.enabled;
            float flickerStep = 0.2f;
            yield return new WaitForSeconds(flickerStep);
            elapsed += flickerStep;
        }

        // End of invincibility: make sprite visible
        sr.enabled = true;

        // 3) Re-enable collisions
        SetEnemyCollisionsIgnored(false);

        isInvincible = false;
        Debug.Log("Invincibility ended.");
    }

}

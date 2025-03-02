using UnityEngine;

public class MushroomMovement : MonoBehaviour
{

    [SerializeField] private float movementSpeed;
    private Rigidbody2D rb;
    private float directionX = 1f;

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
        Debug.Log("I have hit something");
        //foreach (ContactPoint2D contact in collision.contacts)
        //{
        //    if (collision.gameObject.CompareTag("Enemy"))
        //    {
        //        return;     //items are unaffected by enemies
        //    }  
            
        //}

        directionX *= -1f;
        Debug.Log("direction = " + directionX);

    }
  


}

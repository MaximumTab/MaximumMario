using System.Runtime.CompilerServices;

using UnityEngine;

public class PlayerFireball : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    private PlayerController pc;

    void Start()
    {
        pc = gameObject.GetComponent<PlayerController>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && Time.time > nextFireTime)
        {
            ShootFireball();
            nextFireTime = Time.time + fireRate;
        }

        if(Time.time > nextFireTime)
        {
            pc.MarioAnim.SetBool("Shoot",false);
        }
    }

    void ShootFireball()
    {
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        float direction = transform.localScale.x; // Checks facing direction
        fireball.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(5f * direction, 0f);
        pc.MarioAnim.SetBool("Shoot",true);
    }
}

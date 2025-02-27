using UnityEngine;

public class SideScrolling : MonoBehaviour
{
    private Transform player;
    private float leftScreenBound;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        leftScreenBound = Camera.main.ViewportToWorldPoint(Vector3.zero).x;

        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.Max(cameraPosition.x, player.position.x);
        transform.position = cameraPosition;

        Vector3 playerPosition = player.position;
        playerPosition.x = Mathf.Max(playerPosition.x, leftScreenBound);
        player.position = playerPosition;
    }
}

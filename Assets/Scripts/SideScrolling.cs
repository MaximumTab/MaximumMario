using UnityEngine;

public class SideScrolling : MonoBehaviour
{
    private Transform player;
    private float leftScreenBound;
    public float height = 6.5f;

    public float undergroundHeight = -9.5f;
    public float undergroundThreshold = 0f;
    
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

    public void SetUnderground(bool underground)
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayMusic("UGmusic");
    
    }

}
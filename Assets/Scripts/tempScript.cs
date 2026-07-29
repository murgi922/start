using UnityEngine;

public class tempScript : MonoBehaviour
{
    public Transform player;
    public float rotSpeed = 10f;
    void Start()
    {
        
    }

    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.position - transform.position), Time.deltaTime* rotSpeed);
    }
}

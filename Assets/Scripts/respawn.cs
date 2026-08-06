using Unity.VisualScripting;
using UnityEngine;

public class respawn : MonoBehaviour
{
    private GameObject player;
    private Vector3 playerLocation;
    private BoxCollider collider;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.LogError("Player could not be found");
        playerLocation = player.transform.position;
        collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.CompareTag("Player"))
        {
            other.gameObject.transform.parent.position = playerLocation;
            other.gameObject.transform.parent.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            Debug.Log("Overlap!");
        }
    }
}

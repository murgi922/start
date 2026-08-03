using UnityEngine;
using UnityEngine.AI;

public class enemyScript : MonoBehaviour
{
    private Transform target;
    private NavMeshAgent agent;
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.LogError("Could not find player!");
        else target = player.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.destination = target.position;
    }
    private void Update()
    {
        agent.destination = target.position;
    }
}

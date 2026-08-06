using UnityEngine;
using UnityEngine.AI;

public class enemyAIScript : MonoBehaviour
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
    public void SetTarget(string targetTag)
    {
        GameObject temp = GameObject.FindGameObjectWithTag(targetTag);
        if (temp != null)
        {
            target = temp.transform;
        }
        else Debug.LogError("Could not find anything with '" +  targetTag + "' tag!");
    }
}

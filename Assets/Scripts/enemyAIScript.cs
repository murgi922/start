using UnityEngine;
using UnityEngine.AI;

public class enemyAIScript : MonoBehaviour
{
    private Transform target;
    private NavMeshAgent agent;
    [SerializeField] private Transform patrol1;
    [SerializeField] private Transform patrol2;
    [SerializeField] private bool willPatrol;
    [SerializeField] private enemyOrientation orientation;
    private GameObject player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.LogError("Could not find player!");
        agent = GetComponent<NavMeshAgent>();
    }
    
    private void Update()
    {
        if (willPatrol)
        {
            orientation.SetGetWillLook(false);
            if (target != patrol1 && target != patrol2) target = patrol1;
            if (agent.remainingDistance <= agent.stoppingDistance && target == patrol1) target = patrol2;
            else if (agent.remainingDistance <= agent.stoppingDistance && target == patrol2) target = patrol1;
        }
        else
        {
            orientation.SetGetWillLook(true);
            target = player.transform;
        }
        agent.destination = target.position;
    }
    public void SetWillPatrol(bool patrol)
    { willPatrol = patrol; }
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

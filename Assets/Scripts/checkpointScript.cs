using System.Diagnostics;
using UnityEngine;

public class checkpointScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null)
        {
            if (other.transform.parent.CompareTag("Player"))
            {
                other.transform.root.GetComponent<playerManager>().StepOnCheckpoint(transform);
            }
        }
    }
}

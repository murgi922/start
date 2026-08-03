using UnityEngine;

public class enemyOrientation : MonoBehaviour
{
    private Transform player;
    public float rotSpeed = 10f;
    [SerializeField] private bool willLook;
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) Debug.LogError("Player can't be found!");
        else player = playerObject.transform;
    }

    void Update()
    {
        if (willLook)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.position - transform.position), Time.deltaTime * rotSpeed);
        }
    }
    public bool setGetWillLook(bool value)
    {
        bool temp;
        temp = willLook;
        willLook = value;
        return temp;
    }
}

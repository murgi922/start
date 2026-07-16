using UnityEngine;

public class GunFollow : MonoBehaviour
{
    public Transform gunTransform;
    public float speed = 1.0f;
    void Start()
    {
        if (gunTransform == null) Debug.LogError("Gun Transform could not be found");
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(this.transform.position, gunTransform.position, Time.deltaTime * speed);
        transform.rotation = Quaternion.Slerp(transform.rotation, gunTransform.rotation, Time.deltaTime * speed);
    }
}

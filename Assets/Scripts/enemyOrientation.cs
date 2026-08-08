using UnityEngine;

public class enemyOrientation : MonoBehaviour
{
    private Transform camera;
    public float rotSpeed = 10f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private bool willLook;
    private Vector3 temp;
    private Vector3 vel;

    void Start()
    {
        GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        if (cameraObject == null) Debug.LogError("Camera can't be found!");
        else camera = cameraObject.transform;
    }

    void Update()
    {
        if (willLook)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(camera.position - transform.position), Time.deltaTime * rotSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(vel.normalized), Time.deltaTime * rotSpeed);
        }
    }
    private void FixedUpdate()
    {
        vel = Velocity();
    }
    public bool SetGetWillLook(bool value)
    {
        bool temp;
        temp = willLook;
        willLook = value;
        return temp;
    }
    private Vector3 Velocity()
    {
        Vector3 temp1;
        temp1 = transform.position;
        Vector3 delta;
        delta = temp1 - temp;
        temp = transform.position;
        delta /= Time.fixedDeltaTime;
        return delta;
    }
}

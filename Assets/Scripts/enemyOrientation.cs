using UnityEngine;

public class enemyOrientation : MonoBehaviour
{
    private Transform camera;
    public float rotSpeed = 10f;
    [SerializeField] private bool willLook;
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
    }
    public bool setGetWillLook(bool value)
    {
        bool temp;
        temp = willLook;
        willLook = value;
        return temp;
    }
}

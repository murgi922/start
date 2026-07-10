using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerCam : MonoBehaviour
{
    private readonly float sensitivity = 50.0f;
    public float sensitivity_multiplier = 1.0f;
    public Transform orientation;
    public Transform gunOrientation;
    public Transform cameraPos;

    InputAction lookAction;
    private Vector2 look_crd;
    private float xRot;
    private float yRot;
    private float xRotGun;
    private float yRotGun;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        lookAction = InputSystem.actions.FindAction("Look");
        xRot = transform.rotation.eulerAngles.x;
        yRot = transform.rotation.eulerAngles.y;
        xRotGun = gunOrientation.rotation.eulerAngles.x;
        yRotGun = gunOrientation.rotation.eulerAngles.y;
        gunOrientation.position = cameraPos.position;
        
    }

    private void Update()
    {
        look_crd.y = lookAction.ReadValue<Vector2>().x * Time.deltaTime;
        look_crd.x = lookAction.ReadValue<Vector2>().y * Time.deltaTime;

        xRot -= look_crd.x * sensitivity * sensitivity_multiplier;
        yRot += look_crd.y * sensitivity * sensitivity_multiplier;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        xRotGun -= look_crd.x * sensitivity * sensitivity_multiplier;
        yRotGun += look_crd.y * sensitivity * sensitivity_multiplier;
        xRotGun = Mathf.Clamp(xRot, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        gunOrientation.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRot, 0);


    }
}

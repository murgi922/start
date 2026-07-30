using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class weaponSway : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private float springBackSpeed;
    [SerializeField] private float maxSwayDistance;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private float springBackSpeedRot;
    [SerializeField] private float rotSpeed;
    [SerializeField] private Transform camera;
    [SerializeField] private float speed;
    private Quaternion targetRot;
    private Vector3 startPosLoc;
    private Quaternion startRotLoc;
    private Quaternion currentRot;
    private Quaternion finalRot;
    private Quaternion deltaRot;
    InputAction aimAction;
    void Start()
    {
        startPosLoc = transform.localPosition;
        startRotLoc = gunTransform.localRotation;
        aimAction = InputSystem.actions.FindAction("Aim");
    }

    void Update()
    {
        currentRot = camera.localRotation;
        deltaRot = Quaternion.Inverse(currentRot) * finalRot;
        finalRot = camera.localRotation;
        deltaRot = Quaternion.Euler(deltaRot.eulerAngles.x, - deltaRot.eulerAngles.y, deltaRot.eulerAngles.z);
        deltaRot = Quaternion.SlerpUnclamped(Quaternion.identity, deltaRot, speed * 2);
        targetRot = startRotLoc * deltaRot;
        Sway();
    }
    void Sway()
    {
        if (!aimAction.IsInProgress())
        {
            transform.position -= Vector3.ClampMagnitude(playerRb.linearVelocity, maxSwayDistance) * Time.deltaTime;
            gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, targetRot, Time.deltaTime * speed);
        }
        transform.localPosition = Vector3.LerpUnclamped(transform.localPosition, startPosLoc, Time.deltaTime * springBackSpeed);
    }

}

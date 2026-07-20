using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class gunControl : MonoBehaviour
{
    [Header("Gun Control")]
    public Vector3 endPosition = new Vector3(-0.41f, 0.11f, -0.19f);
    private Vector3 startPosition;
    private Quaternion startRot;
    public Quaternion endRot = Quaternion.Euler(0f, 190f, 0f);
    private bool willAim = false;
    public float adsDuration = 3.0f;
    private float elapsedTime;
    InputAction aimAction;

    [Header("Camera Control")]
    public GameObject camera;
    private Camera mainCam;
    private float camStartPos = 72f;
    public float camEndPos = 60f;

    [Header("UI control")]
    public GameObject ui;

    [Header("Gun Animation")]
    public GameObject gunObject;
    private Animator gunAnimator;
    private InputAction fireAction;

    [Header("Raycast")]
    public Transform cameraTransform;
    private RaycastHit gunHit;

    [Header("Muzzle Flash")]
    public ParticleSystem gunParticleSystem;
    

    void Start()
    {
        startPosition = transform.localPosition;
        startRot = transform.localRotation;
        aimAction = InputSystem.actions.FindAction("Aim");
        mainCam = camera.GetComponent<Camera>();
        gunAnimator = gunObject.GetComponent<Animator>();
        fireAction = InputSystem.actions.FindAction("Fire");
    }

    void Update()
    {
        HideCrossHair();
        ADS();
        Fire();
    }
    void ADS()
    {
        if (aimAction.IsPressed()) willAim = true;
        if (willAim && aimAction.IsInProgress())
        {
            if (aimAction.WasPerformedThisFrame()) elapsedTime = 0.0f;
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / adsDuration;
            
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, percentageComplete);
            transform.localRotation = Quaternion.Lerp(startRot, endRot, percentageComplete);
            mainCam.fieldOfView = Mathf.Lerp(camStartPos, camEndPos, percentageComplete);
            if (transform.localPosition == endPosition) willAim = false;
        }
        else if (transform.localPosition != startPosition)
        {
            if (aimAction.WasReleasedThisFrame()) elapsedTime = 0f;
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / adsDuration;
            transform.localPosition = Vector3.Lerp(endPosition, startPosition, percentageComplete);
            transform.localRotation = Quaternion.Lerp(endRot, startRot, percentageComplete);
            mainCam.fieldOfView = Mathf.Lerp(camEndPos, camStartPos, percentageComplete);
        }
    }
    void HideCrossHair()
    {
        if (aimAction.IsPressed()) ui.SetActive(false);
        if (aimAction.WasReleasedThisFrame()) ui.SetActive(true);
    }
    void Fire()
    {
        if (fireAction.triggered)
        {
            gunAnimator.SetTrigger("Fire");
            Physics.Raycast(cameraTransform.position, cameraTransform.forward, out gunHit, Mathf.Infinity);
            if (gunHit.rigidbody != null)
            {
                gunHit.rigidbody.AddForceAtPosition(cameraTransform.forward * 10f, gunHit.point, ForceMode.Impulse);
            }
            gunParticleSystem.Play();
        }
    }
}

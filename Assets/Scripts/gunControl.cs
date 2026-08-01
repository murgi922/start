using System.Runtime.CompilerServices;
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
    public float fireCoolDwn = 0.1f;
    private float fireTime = 0.0f;


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

    [Header("Visual Effects")]
    public ParticleSystem gunParticleSystem;
    public ParticleSystem spark;
    public Transform barrelTipTransform;
    public ParticleSystem bulletEffect;
    [SerializeField] private float bulletSpread = 1f;
    [SerializeField] private float aimBulletSpread = 0.1f;
    private float tempBullSpread;
    

    [Header("Firing Sound")]
    public AudioSource gunSound;

    [Header("Player Stuff")]
    private playerMovement playerScript;

    void Start()
    {
        startPosition = transform.localPosition;
        startRot = transform.localRotation;
        aimAction = InputSystem.actions.FindAction("Aim");
        mainCam = camera.GetComponent<Camera>();
        gunAnimator = gunObject.GetComponent<Animator>();
        fireAction = InputSystem.actions.FindAction("Fire");
        playerScript = GetComponentInParent<playerMovement>();
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
            if (percentageComplete <= 2) transform.localRotation = Quaternion.Lerp(startRot, endRot, percentageComplete);
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
        if (aimAction.IsInProgress())
        {
            tempBullSpread = aimBulletSpread;
            playerScript.isAiming = true;
        }
        else
        {
            tempBullSpread = bulletSpread;
            playerScript.isAiming = false;
        }
    }
    void HideCrossHair()
    {
        if (aimAction.IsPressed()) ui.SetActive(false);
        if (aimAction.WasReleasedThisFrame()) ui.SetActive(true);
    }
    void Fire()
    {
        fireTime += Time.deltaTime;
        if (fireAction.triggered && fireTime > fireCoolDwn)
        {
            gunAnimator.SetTrigger("Fire");
            bool didHit = false;
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            didHit = Physics.Raycast(ray, out gunHit, Mathf.Infinity);
            gunParticleSystem.Play();
            fireTime = 0.0f;
            gunSound.pitch = Random.Range(0.9f, 1.5f);
            gunSound.Play();
            Vector3 direction;
            if (didHit)
            {
                direction = gunHit.point - barrelTipTransform.position;

            }
            else direction = ray.GetPoint(100) - barrelTipTransform.position;
            ParticleSystem bullet;
            Vector3 bulletSpreadVect = Vector3.zero;
            bulletSpreadVect.x = Random.Range(-1f, 1f);
            bulletSpreadVect.y = Random.Range(-1f, 1f);
            bulletSpreadVect.z = Random.Range(-1f, 1f);
            float moveSpeed;
            if (tempBullSpread == 0f) moveSpeed = 0f;
            else moveSpeed = playerScript.moveVelocity.magnitude;
            bullet = Instantiate(bulletEffect, barrelTipTransform.position, Quaternion.LookRotation(direction + bulletSpreadVect.normalized * (tempBullSpread + moveSpeed) * 0.01f * direction.magnitude));
            bullet.GetComponent<bulletScript>().AddGunRef(this);
            camera.GetComponentInParent<CameraShake>().FireStart();
        }
    }
    public void BulletHit(Vector3 hitLocation, Vector3 hitNormal, GameObject hitObject)
    {
        Instantiate(spark, hitLocation, Quaternion.LookRotation(hitNormal));
        if (hitObject.GetComponent<Rigidbody>() != null) hitObject.GetComponent<Rigidbody>().AddForceAtPosition(cameraTransform.forward * 10f, hitLocation, ForceMode.Impulse);
    }
    
}

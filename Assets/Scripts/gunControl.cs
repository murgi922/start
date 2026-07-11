using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class gunControl : MonoBehaviour
{
    private Vector3 startPosition;
    public Vector3 endPosition = new Vector3(-0.41f, 0.11f, -0.19f);
    private Quaternion startRot;
    private Quaternion endRot = Quaternion.Euler(0f, 0f, -1.4f);
    private bool willAim = false;
    public float adsDuration = 3.0f;
    private float elapsedTime;
    InputAction aimAction;

    public GameObject camera;
    private Camera mainCam;
    private float camStartPos = 72f;
    public float camEndPos = 60f;
    void Start()
    {
        startPosition = transform.localPosition;
        startRot = transform.localRotation;
        aimAction = InputSystem.actions.FindAction("Aim");
        mainCam = camera.GetComponent<Camera>();
    }

    void Update()
    {

        ADS();
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
    
}

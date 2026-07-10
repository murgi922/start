using UnityEngine;
using UnityEngine.InputSystem;

public class gunControl : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 endPosition = new Vector3(-0.41f, 0.11f, -0.19f);
    private bool willAim = false;
    public float adsDuration = 3.0f;
    private float cameraZoomDuration = 0.1f;
    private float elapsedTime;
    InputAction aimAction;
    public GameObject camera;
    void Start()
    {
        startPosition = transform.localPosition;
        aimAction = InputSystem.actions.FindAction("Aim");
    }

    void Update()
    {
        if (aimAction.IsPressed()) willAim = true;
        if (willAim && aimAction.IsInProgress())
        {
            if (aimAction.WasPerformedThisFrame()) elapsedTime = 0.0f;
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / adsDuration;
            
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, percentageComplete);
            if (transform.localPosition == endPosition) willAim = false;
        }
        else if (transform.localPosition != startPosition)
        {
            if (aimAction.WasReleasedThisFrame()) elapsedTime = 0f;
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / adsDuration;
            transform.localPosition = Vector3.Lerp(endPosition, startPosition, percentageComplete);
        }
        
    }
}

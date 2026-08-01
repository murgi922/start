using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private AnimationCurve shakeCurve;
    [SerializeField] private float duration;
    [SerializeField] private Vector2 magnitude;
    [SerializeField] public Transform player;
    private float curveOutput = 0f;
    private float elapsedTime = Mathf.Infinity;

    private void Update()
    {
        float mag = Random.Range(magnitude.x, magnitude.y);
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;
            curveOutput = shakeCurve.Evaluate(normalizedTime) * mag;
        }
        else
        {
            curveOutput = 0f;
        }
        
        transform.RotateAround(transform.position, - player.right, curveOutput * Time.deltaTime);
    }
    public void FireStart()
    {
        elapsedTime = 0f;
    }
}

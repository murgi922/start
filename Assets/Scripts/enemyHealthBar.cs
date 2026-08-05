using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class enemyHealthBar : MonoBehaviour
{
    private float healthNorm = 1;
    private Image foreground;
    private void Start()
    {
        foreground = transform.Find("Background").Find("Foreground").GetComponent<Image>();
    }
    private void Update()
    {
        foreground.fillAmount = healthNorm;
        if (healthNorm == 0 && gameObject.activeSelf) gameObject.SetActive(false);
    }
    public void SetHealth(float health)
    { healthNorm = Mathf.InverseLerp(0, 100, health); }
}

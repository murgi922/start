using System;
using System.Collections;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class enemyManager : MonoBehaviour
{
    [Header("Health Related")]
    [SerializeField]
    [Range(0, 100)]
    private int health;
    [SerializeField]
    [Range(0, 100)]
    private int damageToTake;
    [SerializeField, ColorUsage(true, true)] private Color enemyHitColor;
    [SerializeField] private float colorChangeTime;
    private Color enemyColor;
    private HealthSystem healthSystem;
    private bool dead = false;
    [SerializeField] private Transform capsule;
    private Coroutine enemyColorChangeCoroutine;

    [Header("Shooting")]
    [SerializeField] private float fireDelay;
    [SerializeField] private float fireSpread;
    [SerializeField] private float detectionRange;
    private bool willAggro;
    [SerializeField] private gunControlEnemy enemyGun;
    private Transform player;

    [Header("UI")]
    private enemyHealthBar healthBar;

    [Header("AI")]
    [SerializeField] private enemyAIScript enemyAIScript;
    [SerializeField] private string targetTag;
    private void Start()
    {
        enemyColor = capsule.GetComponent<Renderer>().material.color;
        healthBar = GetComponentInChildren<enemyHealthBar>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyGun.IsAliveSet(true);
        StartCoroutine(Temp());
        enemyAIScript.SetTarget(targetTag);
    }
    IEnumerator Temp()
    {
        yield return new WaitForSeconds(3);
        enemyGun.Fire(player, fireDelay, fireSpread);
        
    }
    private void Awake()
    {
        healthSystem = new HealthSystem(100);

                                        }
    private void OnValidate()
    {
        damageToTake = Mathf.Clamp(damageToTake, 0, health);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    public void HitByProjectile()
    {
        if (!dead)
        {
            if (enemyColorChangeCoroutine != null) StopCoroutine(enemyColorChangeCoroutine);
            enemyColorChangeCoroutine = StartCoroutine(HitColorChange(enemyColor, enemyHitColor, colorChangeTime));
            healthSystem.TakeDamage(damageToTake);
            healthBar.SetHealth(healthSystem.GetHealth());
            if (!healthSystem.IsAlive())
            {
                gameObject.GetComponentInChildren<enemyAIScript>().enabled = false;
                gameObject.GetComponentInChildren<enemyOrientation>().enabled = false;
                gameObject.GetComponentInChildren<NavMeshAgent>().enabled = false;
                gameObject.GetComponentInChildren<Rigidbody>().freezeRotation = false;
                gameObject.GetComponentInChildren<Rigidbody>().isKinematic = false;
                dead = true;
                enemyGun.IsAliveSet(false);
            }
        }
    } 
    IEnumerator HitColorChange(Color initialColor, Color color, float timeSpan)
    {
        float elapsedTime = 0f;
        float percentageComplete = 0f;
        Renderer capsuleRenderer = capsule.GetComponent<Renderer>();
        capsuleRenderer.material.color = color;
        percentageComplete = 0f;
        elapsedTime = 0f;
        while (percentageComplete <= 1f || capsuleRenderer.material.color != initialColor)
        {
            elapsedTime += Time.deltaTime;
            percentageComplete = elapsedTime / (timeSpan / 2);
            capsuleRenderer.material.color = Color.Lerp(color, initialColor, percentageComplete);
            yield return null;
        }
    }

    public float GetSpread()
    { return fireSpread; }
    
}

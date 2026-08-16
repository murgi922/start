using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private gunControl gunControlScript;
    private gunControlEnemy gunControlEnemy;
    private ParticleSystemRenderer particleSysRend;
    [Header("Speed Scale settings")]
    [SerializeField] private float maxSpeedScale = 0.1f;
    [SerializeField] private float targetDistance = 30f;
    private float percent = 0f;
    private ParticleSystem.Particle[] particles= new ParticleSystem.Particle[1];
    private Vector3 intialGunPos;
    
    private List<ParticleCollisionEvent> particleColEvents = new List<ParticleCollisionEvent>();
    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSysRend = particleSystem.GetComponent<ParticleSystemRenderer>();
    }
    private void OnEnable()
    {
        particleSysRend.velocityScale = 0f;
        particleSystem.GetParticles(particles);
        intialGunPos = transform.position;
    }
    private void FixedUpdate()
    {
        particleSystem.GetParticles(particles);
        Vector3 particleDirection = particles[0].position - intialGunPos;
        percent = particleDirection.magnitude / targetDistance;
        particleSysRend.velocityScale = Mathf.Lerp(0f, maxSpeedScale, percent);
    }
    public void AddGunRef (GameObject gun)
    {
        gunControlScript = gun.GetComponent<gunControl>();
        if (gunControlScript == null) gunControlEnemy = gun.GetComponent<gunControlEnemy>();
    }
    private void OnParticleCollision(GameObject other)
    {
        if (other.transform.root.gameObject.CompareTag("Enemy") && gunControlEnemy == null)
        {
            other.transform.root.gameObject.GetComponent<enemyManager>().HitByProjectile();
        }
        particleSystem.GetCollisionEvents(other, particleColEvents);
        if (particleColEvents.Count > 0)
        {
            ParticleCollisionEvent particleCol = particleColEvents[0];
            if (gunControlScript != null)
            {
                gunControlScript.BulletHit(particleCol.intersection, particleCol.normal, other);
            }
            else if (gunControlEnemy != null)
            {
                gunControlEnemy.BulletHit(particleCol.intersection, particleCol.normal, other);
            }
            else Debug.LogError("gunControl script could not be accessed!");
        }
        if (other.CompareTag("Player"))
        {
            other.transform.root.GetComponent<playerManager>().PlayerHit();
        }
        Destroy(this.gameObject);
        
    }
}

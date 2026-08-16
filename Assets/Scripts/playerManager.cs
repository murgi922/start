using System.Collections;
using UnityEngine;

public class playerManager : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField, Range(1, 100)] private int maxHealth;
    [SerializeField, Range(0, 100)] private int damage;
    [SerializeField] private MonoBehaviour[] toKill;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private GameObject player;
    private HealthSystem playerHealthSystem;
    private bool playerKilled = false;
    private Vector3 respawnLocation;
    void Start()
    {
        playerHealthSystem = new HealthSystem(maxHealth);
    }
    public void PlayerHit()
    {
        if (playerHealthSystem.IsAlive())
        {
            playerHealthSystem.TakeDamage(damage);
            Debug.Log("Taken " + damage + " damage, remaining health " + playerHealthSystem.GetHealth());
        }
        else if (!playerKilled)
        {
            KillPlayer();
        }
    }
    public void KillPlayer()
    {
        for (int i = 0; i < toKill.Length; i++)
        {
            toKill[i].enabled = false;
        }
        playerRb.freezeRotation = false;
        playerKilled = true;
        StartCoroutine(RespawnTimer());
    }
    public void Respawn()
    {
        playerHealthSystem.Heal(playerHealthSystem.GetMaxHealth() - playerHealthSystem.GetHealth());
        player.transform.position = respawnLocation;
        player.transform.rotation = Quaternion.identity;
        playerRb.freezeRotation = true;
        for (int i = 0;i < toKill.Length;i++)
        {
            toKill[(i)].enabled = true;
        }
        playerKilled = false;
    }
    public void StepOnCheckpoint(Transform checkpoint)
    {
        respawnLocation = checkpoint.position;
    }
    public bool IsAlive() { return playerHealthSystem.IsAlive(); }
    private void OnValidate()
    {
        damage = Mathf.Clamp(damage, 0, maxHealth);
    }
    IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(1f);
        Respawn();
    }
}

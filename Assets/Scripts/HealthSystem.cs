public class HealthSystem
{
    private float health;
    private float maxHealth;
    public HealthSystem(int health)
    {
        this.health = health;
        maxHealth = health;
    }
    public HealthSystem(int health, int maxHealth)
    {
        this.health = health;
        this.maxHealth = maxHealth;
    }
    public float GetHealth()
    {
        return health;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0) health = 0;
    }
    public void Heal(int healAmount)
    {
        health += healAmount;
        if (health > maxHealth) health = maxHealth;
    }
    public bool IsAlive()
    {
        return health > 0;
    }
}

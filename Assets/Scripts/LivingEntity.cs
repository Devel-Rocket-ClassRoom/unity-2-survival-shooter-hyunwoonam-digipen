using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f;
    public float Health { get; private set; }
    public bool IsDead { get; private set; }

    public UnityEvent OnDead;

    protected virtual void OnEnable()
    {
        IsDead = false;
        Health = startingHealth;
    }
    public void AddHealth(float health)
    {
        Health += health;

        if (Health > 100)
        {
            Health = 100;
        }
    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }

    public virtual void OnHeal(float add)
    {
        Health += add;

        if (Health > startingHealth)
        {
            Health = startingHealth;
        }
    }

    public virtual void Die()
    {
        IsDead = true;
        OnDead?.Invoke();
    }
}

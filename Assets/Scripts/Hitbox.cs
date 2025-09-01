using UnityEngine;

public class Hitbox : MonoBehaviour, IDamageable
{
    [SerializeField] PlayerController owner;

    [SerializeField] float damageMultiplier = 1f;

    private void Awake()
    {
        if (owner == null)
            owner = GetComponentInParent<PlayerController>();
    }

    public void TakeDamage(float damage)
    {
        if (owner != null)
        {
            owner.TakeDamage(damage * damageMultiplier);
        }
    }
}

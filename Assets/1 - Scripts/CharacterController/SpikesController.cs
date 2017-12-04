using UnityEngine;

public class SpikesController : MonoBehaviour
{
    public int damage;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.collider.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(damage);
    }
}

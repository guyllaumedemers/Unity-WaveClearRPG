
using UnityEngine;

namespace Characters.Enemy
{
    public interface IEnemy
    {
        Stats EnemyStats { get; set; }
        void Attack();
        void TakeDamage(float amount);
        void Die();
    }
}

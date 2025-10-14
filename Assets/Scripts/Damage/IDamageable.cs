using UnityEngine;

namespace Damage
{
    public interface IDamageable
    {
        void ApplyDamage(float amount);
        bool IsAlive { get; }
    }
}

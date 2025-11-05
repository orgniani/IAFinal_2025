using UnityEngine;

namespace Damage
{
    public interface IDamageable
    {
        void ApplyDamage(float amount);
        bool IsAlive { get; }


        // Optional combat behaviors
        bool AggroLocked { get; }
        void ClearAggroLock();
    }
}

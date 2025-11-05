using UnityEngine;

namespace VFX
{
    public class TargetManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string enemyLayerName = "Enemy";
        [SerializeField] private string outlineLayerName = "Outline";

        private GameObject _currentTarget;
        private int _currentTargetOriginalLayer;

        public GameObject CurrentTarget => _currentTarget;

        public void UpdateHoverTarget(GameObject newTarget)
        {
            if (_currentTarget == newTarget)
                return;

            ClearCurrentOutline();

            if (newTarget != null && newTarget.layer == LayerMask.NameToLayer(enemyLayerName))
                ApplyOutline(newTarget);
        }

        private void ApplyOutline(GameObject target)
        {
            _currentTarget = target;
            _currentTargetOriginalLayer = target.layer;

            int outlineLayer = LayerMask.NameToLayer(outlineLayerName);
            _currentTarget.layer = outlineLayer;
        }

        private void ClearCurrentOutline()
        {
            if (_currentTarget == null)
                return;

            _currentTarget.layer = _currentTargetOriginalLayer;
            _currentTarget = null;
        }
    }
}

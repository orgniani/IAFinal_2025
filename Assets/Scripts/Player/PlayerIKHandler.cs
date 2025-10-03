using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerIKHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float rayLength = 1.5f;
        [SerializeField] private float footOffset = 0.1f;
        [SerializeField] private float ikWeight = 1f;
        [SerializeField] private float smooth = 10f;

        private Animator _animator;
        private Vector3 _leftFootPos, _rightFootPos;
        private Quaternion _leftFootRot, _rightFootRot;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            AdjustFootIK(AvatarIKGoal.LeftFoot, ref _leftFootPos, ref _leftFootRot);
            AdjustFootIK(AvatarIKGoal.RightFoot, ref _rightFootPos, ref _rightFootRot);
        }

        private void AdjustFootIK(AvatarIKGoal foot, ref Vector3 footPos, ref Quaternion footRot)
        {
            Vector3 origin = _animator.GetIKPosition(foot) + Vector3.up;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, groundLayer))
            {
                Vector3 targetPos = hit.point + Vector3.up * footOffset;
                Quaternion targetRot = Quaternion.LookRotation(transform.forward, hit.normal);

                footPos = Vector3.Lerp(footPos, targetPos, Time.deltaTime * smooth);
                footRot = Quaternion.Slerp(footRot, targetRot, Time.deltaTime * smooth);

                _animator.SetIKPositionWeight(foot, ikWeight);
                _animator.SetIKRotationWeight(foot, ikWeight);
                _animator.SetIKPosition(foot, footPos);
                _animator.SetIKRotation(foot, footRot);
            }
            else
            {
                _animator.SetIKPositionWeight(foot, 0f);
                _animator.SetIKRotationWeight(foot, 0f);
            }
        }
    }
}
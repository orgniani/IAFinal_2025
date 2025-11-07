using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyJumpState : StateMachineBehaviour
    {
        [Header("Jump Settings")]
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float jumpDuration = 0.6f;
        [SerializeField] private float rotationSpeed = 720f;

        [Header("Parameters")]
        [SerializeField] private EnemyAnimationParameters animationParameters;

        private NavMeshAgent _agent;
        private Vector3 _startPos;
        private Vector3 _endPos;
        private Vector3 _jumpDir;
        private float _elapsed;
        private bool _isJumping;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _agent = animator.GetComponent<NavMeshAgent>();
            if (_agent == null || !_agent.isOnOffMeshLink)
            {
                animator.SetBool(animationParameters.isJumpingBool, false);
                return;
            }

            _agent.autoTraverseOffMeshLink = false;
            _isJumping = true;
            _elapsed = 0f;

            OffMeshLinkData data = _agent.currentOffMeshLinkData;
            _startPos = _agent.transform.position;
            _endPos = data.endPos + Vector3.up * _agent.baseOffset;

            _jumpDir = _endPos - _startPos;
            _jumpDir.y = 0f;
            if (_jumpDir.sqrMagnitude > 0.001f)
                _jumpDir.Normalize();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_isJumping || _agent == null)
                return;

            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / jumpDuration);

            Vector3 pos = Vector3.Lerp(_startPos, _endPos, t);
            pos.y += Mathf.Sin(Mathf.PI * t) * jumpHeight;
            _agent.transform.position = pos;

            if (_jumpDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(_jumpDir);
                _agent.transform.rotation = Quaternion.RotateTowards(
                    _agent.transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );
            }

            if (t >= 1f)
            {
                _agent.CompleteOffMeshLink();
                _isJumping = false;
                animator.SetBool(animationParameters.isJumpingBool, false);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_agent && _agent.isOnOffMeshLink)
                _agent.CompleteOffMeshLink();
            _isJumping = false;
        }
    }
}

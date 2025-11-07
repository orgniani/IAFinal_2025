using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerJump : MonoBehaviour
    {
        [Header("Jump Settings")]
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float jumpDuration = 0.6f;

        private NavMeshAgent _agent;
        private bool _isJumping;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.autoTraverseOffMeshLink = false;
        }

        private void Update()
        {
            if (_agent.isOnOffMeshLink && !_isJumping)
                StartCoroutine(HandleJump());
        }

        private IEnumerator HandleJump()
        {
            _isJumping = true;

            OffMeshLinkData linkData = _agent.currentOffMeshLinkData;
            Vector3 startPos = _agent.transform.position;
            Vector3 endPos = linkData.endPos + Vector3.up * _agent.baseOffset;

            float elapsed = 0f;

            while (elapsed < jumpDuration)
            {
                float t = elapsed / jumpDuration;
                Vector3 pos = Vector3.Lerp(startPos, endPos, t);
                pos.y += Mathf.Sin(Mathf.PI * t) * jumpHeight;

                _agent.transform.position = pos;

                elapsed += Time.deltaTime;
                yield return null;
            }

            _agent.CompleteOffMeshLink();
            _isJumping = false;
        }
    }
}

using UnityEngine;
using Helpers;
using UnityEngine.AI;

namespace Player
{
    public class PlayerView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController player;

        [Header("Animator Parameters")]
        [SerializeField] private string moveXParameter = "moveX";
        [SerializeField] private string moveZParameter = "moveZ";
        [SerializeField] private string shootBoolParameter = "shoot";

        [Header("Animation Settings")]
        [SerializeField] private float dampTime = 0.15f;

        private Animator _animator;

        private Transform _playerTransform;
        private NavMeshAgent _playerAgent;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            ValidateReferences();

            _playerTransform = player.transform;
            _playerAgent = player.GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            player.OnShoot += HandleShooting;
        }

        private void OnDisable()
        {
            player.OnShoot -= HandleShooting;
        }

        private void Update()
        {
            Vector3 velocity = _playerAgent.velocity;
            Vector3 localVelocity = _playerTransform.InverseTransformDirection(velocity);

            float moveX = localVelocity.x;
            float moveZ = localVelocity.z;

            _animator.SetFloat(moveXParameter, moveX, dampTime, Time.deltaTime);
            _animator.SetFloat(moveZParameter, moveZ, dampTime, Time.deltaTime);
        }

        private void HandleShooting(bool isShooting)
        {
            _animator.SetBool(shootBoolParameter, isShooting);
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(player, nameof(player), this);
            ReferenceValidator.Validate(_animator, nameof(_animator), this);
        }
    }
}

using Damage;
using UnityEngine;
using System;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;

        private CharacterController _controller;
        private Camera _mainCamera;
        private Vector2 _moveInput;

        private Health _health;

        public event Action<bool> OnShoot;
        public CharacterController Controller => _controller;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _mainCamera = Camera.main;
            _health = GetComponent<Health>();
        }

        private void Update()
        {
            if (!_health.IsAlive)
                return;

            HandleMovement();
            HandleRotation();
        }

        public void SetMoveInput(Vector2 input) => _moveInput = input;

        public void SetShootInput(bool shooting)
        {
            if (!_health.IsAlive)
                return;

            OnShoot?.Invoke(shooting);
        }

        private void HandleMovement()
        {
            Vector3 input = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
            _controller.SimpleMove(input * moveSpeed);
        }

        private void HandleRotation()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 lookDir = hit.point - transform.position;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.01f)
                    transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }
}

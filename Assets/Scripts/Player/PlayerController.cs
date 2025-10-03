using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;

        private CharacterController _controller;
        private Camera _mainCamera;

        private Vector2 _moveInput;
        public event Action<bool> OnShoot;

        public CharacterController Controller => _controller;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleMovement();
            HandleRotation();
        }

        public void SetMoveInput(Vector2 input) => _moveInput = input;

        public void SetShootInput(bool shooting)
        {
            OnShoot?.Invoke(shooting);
        }

        private void HandleMovement()
        {
            Vector3 input = new Vector3(_moveInput.x, 0, _moveInput.y);
            input = input.normalized;

            Vector3 velocity = input * moveSpeed;
            _controller.SimpleMove(velocity);
        }

        private void HandleRotation()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 lookDir = hit.point - transform.position;
                lookDir.y = 0;

                if (lookDir.sqrMagnitude > 0.01f)
                    transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }
}

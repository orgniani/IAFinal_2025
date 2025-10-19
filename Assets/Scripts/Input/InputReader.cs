using UnityEngine;
using UnityEngine.InputSystem;
using Player;
using Helpers;
using VFX;

namespace Input
{
    public class InputReader : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("References")]
        [SerializeField] private PlayerController player;
        [SerializeField] private ClickIndicatorController clickIndicatorController;

        [Header("Settings")]
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float rayDistance = 100f;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _attackAction;
        private Camera _mainCamera;

        private void Awake()
        {
            ValidateReferences();
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            _moveAction = inputActions.FindAction("Move");
            if (_moveAction != null)
            {
                _moveAction.started += HandleMoveInput;
                _moveAction.canceled += HandleMoveInput;
            }

            _lookAction = inputActions.FindAction("Look");
            if (_lookAction != null)
            {
                _lookAction.performed += HandleLookInput;
                _lookAction.canceled += HandleLookInput;
            }

            _attackAction = inputActions.FindAction("Attack");
            if (_attackAction != null)
            {
                _attackAction.performed += HandleShootInput;
                _attackAction.canceled += HandleShootInput;
            }
        }

        private void OnDisable()
        {
            if (_moveAction != null)
            {
                _moveAction.started -= HandleMoveInput;
                _moveAction.canceled -= HandleMoveInput;
            }

            if (_lookAction != null)
            {
                _lookAction.performed -= HandleLookInput;
                _lookAction.canceled -= HandleLookInput;
            }

            if (_attackAction != null)
            {
                _attackAction.performed -= HandleShootInput;
                _attackAction.canceled -= HandleShootInput;
            }
        }

        private void HandleMoveInput(InputAction.CallbackContext ctx)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();

            if (TryGetMouseWorldPosition(screenPos, out Vector3 worldPos, stopOnObstacles: true))
            {
                player.MoveToClickPoint(worldPos);
                clickIndicatorController?.SpawnIndicator(worldPos);
            }
        }

        private void HandleLookInput(InputAction.CallbackContext ctx)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            if (TryGetMouseWorldPosition(screenPos, out Vector3 worldPos, stopOnObstacles: false))
                player.RotateTowards(worldPos);
        }

        private bool TryGetMouseWorldPosition(Vector2 screenPos, out Vector3 worldPos, bool stopOnObstacles)
        {
            worldPos = Vector3.zero;

            if (Camera.main == null)
            {
                Debug.LogError("[InputReader] Camera.main is NULL. Cannot raycast.");
                return false;
            }

            screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width - 1);
            screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height - 1);

            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);
            if (hits.Length == 0)
                return false;

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                int layer = hit.collider.gameObject.layer;
                bool isGround = (groundMask & (1 << layer)) != 0;

                if (isGround)
                {
                    worldPos = hit.point;
                    return true;
                }

                if (stopOnObstacles)
                {
                    Debug.Log($"[InputReader] Blocked by '{hit.collider.name}' on layer {layer} ({LayerMask.LayerToName(layer)})");
                    return false;
                }
            }

            return false;
        }

        private void HandleShootInput(InputAction.CallbackContext ctx)
        {
            bool shooting = ctx.phase != InputActionPhase.Canceled;
            player.Shoot(shooting);
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(inputActions, nameof(inputActions), this);
            ReferenceValidator.Validate(player, nameof(player), this);
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using Helpers;
using Player;

namespace Input
{
    public class InputReader : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("References")]
        [SerializeField] private PlayerController player;

        private InputAction _moveAction;
        private InputAction _attackAction;

        private void Awake()
        {
            ValidateReferences();
        }

        private void OnEnable()
        {
            _moveAction = inputActions.FindAction("Move");
            if (_moveAction != null)
            {
                _moveAction.performed += HandleMovementInput;
                _moveAction.canceled += HandleMovementInput;
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
                _moveAction.performed -= HandleMovementInput;
                _moveAction.canceled -= HandleMovementInput;
            }

            if (_attackAction != null)
            {
                _attackAction.performed -= HandleShootInput;
                _attackAction.canceled -= HandleShootInput;
            }
        }

        private void HandleMovementInput(InputAction.CallbackContext ctx)
        {
            Vector2 movementInput = ctx.ReadValue<Vector2>();
            player.SetMoveInput(movementInput);
        }

        private void HandleShootInput(InputAction.CallbackContext ctx)
        {
            bool shooting = ctx.phase != InputActionPhase.Canceled;
            player.SetShootInput(shooting);
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(inputActions, nameof(inputActions), this);
            ReferenceValidator.Validate(player, nameof(player), this);
        }
    }
}
using Features.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Slayground.Common
{
    public class TestMovementInputReader
    {
        private readonly PlayerControls _playerControls = new();
        
        private Vector2 MovementInput => ReadMovementInput();

        public void Enable()
        {
            _playerControls.Locomotion.Move.performed += OnMove;
            _playerControls.Locomotion.Jump.performed += OnJump;
            
            _playerControls.Enable();
            
            _playerControls.UI.Disable();
            _playerControls.Locomotion.Enable();
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            Debug.Log("Jump!");
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            Debug.Log($"MOVING: {ctx.ReadValue<Vector2>()}, {MovementInput}");
        }

        public void Disable()
        {
            _playerControls.Disable();
            
            _playerControls.Locomotion.Move.performed -= OnMove;
            _playerControls.Locomotion.Jump.performed -= OnJump;
        }

        private Vector2 ReadMovementInput()
        {
            return _playerControls.Locomotion.Move.ReadValue<Vector2>();
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lesson7
{
    public class InputController : MonoBehaviour
    {
        public static event Action<Vector2> OnMoveInput;
        public static event Action<Vector2> OnRotateInput;
        public static event Action<Vector2> OnLookInput;
        public static event Action<bool> OnCameraLock;
        public static event Action<bool> OnPrimaryInput; // Added event for primary input

        [SerializeField] private InputActionAsset _inputActionAsset;
        [SerializeField] private string _mapName;
        [SerializeField] private string _moveName;
        [SerializeField] private string _rotateName;
        [SerializeField] private string _lookAroundName;
        [SerializeField] private string _cameraLockName;
        [SerializeField] private string _primaryInputName; // Added primary input name

        private InputAction _moveAction;
        private InputAction _rotateAction;
        private InputAction _lookAroundAction;
        private InputAction _cameraLockAction;
        private InputAction _primaryInputAction; // Added primary input action

        private bool _inputUpdated;

        private void OnEnable()
        {
            _inputActionAsset.Enable();
            InputActionMap actionMap = _inputActionAsset.FindActionMap(_mapName);
            _moveAction = actionMap[_moveName];
            _rotateAction = actionMap[_rotateName];
            _lookAroundAction = actionMap[_lookAroundName];
            _cameraLockAction = actionMap[_cameraLockName];
            _primaryInputAction = actionMap[_primaryInputName]; // Assign primary input action

            _moveAction.performed += MovePerformedHandler;
            _moveAction.canceled += MoveCanceledHandler;
            
            _rotateAction.performed += RotatePerformedHandler;
            _rotateAction.canceled += RotateCanceledHandler;

            _lookAroundAction.performed += LookPerformedHandler;
            _lookAroundAction.canceled += LookCanceledHandler;
            
            _cameraLockAction.performed += CameraLockPerformedHandler;
            _cameraLockAction.canceled += CameraLockCanceledHandler;
            
            _primaryInputAction.performed += PrimaryInputPerformedHandler; // Subscribe to primary input
            _primaryInputAction.canceled += PrimaryInputCanceledHandler;
        }

        private void OnDisable()
        {
            _inputActionAsset.Disable();
        }

        private void OnDestroy()
        {
            OnMoveInput = null;
            OnRotateInput = null;
            OnLookInput = null;
            OnCameraLock = null;
            OnPrimaryInput = null; // Cleanup primary input event
        }

        private void MovePerformedHandler(InputAction.CallbackContext context)
        {
            OnMoveInput?.Invoke(context.ReadValue<Vector2>());
        }

        private void MoveCanceledHandler(InputAction.CallbackContext context)
        {
            OnMoveInput?.Invoke(context.ReadValue<Vector2>());
        }
        
        private void RotatePerformedHandler(InputAction.CallbackContext context)
        {
            OnRotateInput?.Invoke(context.ReadValue<Vector2>());
        }

        private void RotateCanceledHandler(InputAction.CallbackContext context)
        {
            OnRotateInput?.Invoke(context.ReadValue<Vector2>());
        }
        
        private void LookPerformedHandler(InputAction.CallbackContext context)
        {
            OnLookInput?.Invoke(context.ReadValue<Vector2>());
        }
        
        private void LookCanceledHandler(InputAction.CallbackContext context)
        {
            OnLookInput?.Invoke(context.ReadValue<Vector2>());
        }

        private void CameraLockPerformedHandler(InputAction.CallbackContext context)
        {
            OnCameraLock?.Invoke(true);
        }

        private void CameraLockCanceledHandler(InputAction.CallbackContext context)
        {
            OnCameraLock?.Invoke(false);
        }

        private void PrimaryInputPerformedHandler(InputAction.CallbackContext context)
        {
            OnPrimaryInput?.Invoke(true); // Trigger event for primary input
        }
        
        private void PrimaryInputCanceledHandler(InputAction.CallbackContext context)
        {
            OnPrimaryInput?.Invoke(false); // Trigger event for primary input
        }
    }
}

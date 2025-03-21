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
        public static event Action<bool> OnPrimaryInput;
        public static event Action<bool> OnGrenadeInput;
        public static event Action<bool> OnGoLobbyInput;

        [SerializeField] private InputActionAsset _inputActionAsset;
        [SerializeField] private string _mapName;
        [SerializeField] private string _moveName;
        [SerializeField] private string _rotateName;
        [SerializeField] private string _lookAroundName;
        [SerializeField] private string _cameraLockName;
        [SerializeField] private string _primaryInputName;
        [SerializeField] private string _grenadeInputName;
        [SerializeField] private string _goLobbyInputName;

        private InputAction _moveAction;
        private InputAction _rotateAction;
        private InputAction _lookAroundAction;
        private InputAction _cameraLockAction;
        private InputAction _primaryInputAction;
        private InputAction _grenadeInputAction;
        private InputAction _goLobbyInputAction;

        private bool _inputUpdated;

        private void OnEnable()
        {
            _inputActionAsset.Enable();
            InputActionMap actionMap = _inputActionAsset.FindActionMap(_mapName);
            _moveAction = actionMap[_moveName];
            _rotateAction = actionMap[_rotateName];
            _lookAroundAction = actionMap[_lookAroundName];
            _cameraLockAction = actionMap[_cameraLockName];
            _primaryInputAction = actionMap[_primaryInputName];
            _grenadeInputAction = actionMap[_grenadeInputName];
            _goLobbyInputAction = actionMap[_goLobbyInputName];

            _moveAction.performed += MovePerformedHandler;
            _moveAction.canceled += MoveCanceledHandler;
            
            _rotateAction.performed += RotatePerformedHandler;
            _rotateAction.canceled += RotateCanceledHandler;

            _lookAroundAction.performed += LookPerformedHandler;
            _lookAroundAction.canceled += LookCanceledHandler;
            
            _cameraLockAction.performed += CameraLockPerformedHandler;
            _cameraLockAction.canceled += CameraLockCanceledHandler;
            
            _primaryInputAction.performed += PrimaryInputPerformedHandler;
            _primaryInputAction.canceled += PrimaryInputCanceledHandler;
            
            _grenadeInputAction.performed += GrenadeInputPerformedHandler;
            _grenadeInputAction.canceled += GrenadeInputCanceledHandler;
            
            _goLobbyInputAction.performed += GoLobbyInputPerformedHandler;
            _goLobbyInputAction.canceled += GoLobbyInputCanceledHandler;
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
        
        private void GrenadeInputPerformedHandler(InputAction.CallbackContext context)
        {
            OnGrenadeInput?.Invoke(true); // Trigger event for primary input
        }
        
        private void GrenadeInputCanceledHandler(InputAction.CallbackContext context)
        {
            OnGrenadeInput?.Invoke(false); // Trigger event for primary input
        }
        private void GoLobbyInputPerformedHandler(InputAction.CallbackContext context)
        {
            OnGoLobbyInput?.Invoke(true); 
        }
        
        private void GoLobbyInputCanceledHandler(InputAction.CallbackContext context)
        {
            OnGoLobbyInput?.Invoke(false); // Trigger event for primary input
        }
        
    }
}

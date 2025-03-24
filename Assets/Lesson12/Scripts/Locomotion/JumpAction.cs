using StateMachineSystem.ServiceLocatorSystem;
using UnityEngine;
using Lesson7;

namespace StateMachineSystem.Locomotion
{
    public class JumpAction : MonoBehaviour
    {
        [SerializeField] private LocomotionController _locomotionController;
        [SerializeField] private CharacterController _characterController;

        private void Start()
        {
            ServiceLocator.Instance.GetService<InputController>().OnJump += JumpHandler;
        }

        private void JumpHandler()
        {
            if (_characterController.isGrounded)
                _locomotionController.StateMachine.SetState((byte)LocomotionState.Jump);
        }
    }
}
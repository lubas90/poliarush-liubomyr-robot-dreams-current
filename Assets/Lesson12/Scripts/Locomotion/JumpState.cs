using System.Collections.Generic;
using UnityEngine;

namespace StateMachineSystem.Locomotion
{
    public class JumpState : StateBase
    {
        private CharacterController _characterController;

        private Vector3 _jumpDirection;
        private Vector3 _velocity;
        private float _verticalJumpSpeed;
        private float _horizontalJumpSpeed;
        private float _drag;
        
        public JumpState(
            StateMachine stateMachine,
            byte stateId,
            CharacterController characterController,
            float drag, float verticalJumpSpeed, float horizontalJumpSpeed) : base(stateMachine, stateId)
        {
            _characterController = characterController;
            _drag = drag;
            _verticalJumpSpeed = verticalJumpSpeed;
            _horizontalJumpSpeed = horizontalJumpSpeed;
            
            conditions = new List<IStateCondition> {new BaseCondition((byte)LocomotionState.Fall, JumpComplete)};
        }

        public override void Enter()
        {
            Vector3 direction = _characterController.velocity;
            direction.y = 0f;
            float speed = direction.magnitude;
            direction.Normalize();

            _velocity = direction * (speed * _horizontalJumpSpeed) + Vector3.up * _verticalJumpSpeed;
        }

        protected override void OnUpdate(float deltaTime)
        {
            float verticalVelocity = _velocity.y;
            _velocity.y = 0f;
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, _drag * deltaTime);
            _velocity.y = verticalVelocity;
            
            _velocity += Physics.gravity * deltaTime;
            
            _ = _characterController.Move(_velocity * deltaTime);
        }
        
        public override void Dispose()
        {
        }

        private bool JumpComplete()
        {
            return _velocity.y <= 0f;
        }
    }
}
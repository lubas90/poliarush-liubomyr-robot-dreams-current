using System;
using System.Collections.Generic;
using StateMachineSystem.ServiceLocatorSystem;
using UnityEngine;
using Lesson7;

namespace StateMachineSystem.Locomotion
{
    public class MovementState : StateBase
    {
        private readonly CharacterController _characterController;
        private readonly Transform _transform;
        private readonly float _speed;
        
        private bool _grounded;
        private float rotationY;

        private Vector3 _localDirection;
        private Vector3 _rotateDirection;
        
        private InputController _inputController;
        
        public MovementState(
            StateMachine stateMachine,
            byte stateId,
            CharacterController characterController,
            Transform transform,
            float speed) : base(stateMachine, stateId)
        {
            _characterController = characterController;
            _transform = transform;
            _speed = speed;
            
            conditions = new List<IStateCondition>
            {
                new BaseCondition((byte)LocomotionState.Idle, IsIdle),
                new BaseCondition((byte)LocomotionState.Fall, IsFalling)
            };
            
            _inputController = ServiceLocator.Instance.GetService<InputController>();
            
            _inputController.OnMoveInput += MoveHandler;

            _inputController.OnRotateInput += RotateHandler;
        }

        private void MoveHandler(Vector2 input)
        {
            _localDirection = new Vector3(input.x, 0, input.y);
        }

        private void RotateHandler(Vector2 input)
        {
            _rotateDirection = new Vector3(input.x, 0, input.y);
            Debug.Log(_rotateDirection);
            rotationY = _rotateDirection.x * _speed/7;
            //_characterController.transform.Rotate(Vector3.up, rotationY);
        }
        
        protected override void OnUpdate(float deltaTime)
        {
            Vector3 forward = _transform.forward;
            Vector3 right = _transform.right;
            
            Vector3 direction = forward * _localDirection.z + right * _localDirection.x;

            //_grounded = _characterController.SimpleMove(direction * _speed);
            _ = _characterController.Move((direction * _speed + Physics.gravity) * deltaTime);
            
            _characterController.transform.Rotate(Vector3.up, rotationY);
        }

        public override void Dispose()
        {
            _inputController.OnMoveInput -= MoveHandler;
            _inputController.OnRotateInput -= RotateHandler;
        }

        private bool IsIdle()
        {
            return Mathf.Approximately(_localDirection.sqrMagnitude, 0f);
        }
        
        private bool IsFalling()
        {
            return !_characterController.isGrounded;
        }
    }
}
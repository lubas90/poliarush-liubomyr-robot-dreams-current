using UnityEngine;

namespace Lesson7
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _speed;

        private Transform _transform;

        private Vector2 _moveInput;
        private Vector2 _rotateInput;

        private void Start()
        {
            InputController.OnMoveInput += MoveHandler;
            InputController.OnRotateInput += RotateHandler;
            _transform = transform;
        }

        private void FixedUpdate()
        {
            Vector3 forward = _transform.forward;
            Vector3 right = _transform.right;
            Vector3 movement = forward * _moveInput.y + right * _moveInput.x;
            _characterController.SimpleMove(movement * _speed);
            float rotationY = _rotateInput.x * _speed;
            _characterController.transform.Rotate(Vector3.up, rotationY);
        }

        private void MoveHandler(Vector2 moveInput)
        {
            _moveInput = moveInput.normalized;
        }
        
        private void RotateHandler(Vector2 rotateInput)
        {
            _rotateInput = rotateInput;
        }
    }
}
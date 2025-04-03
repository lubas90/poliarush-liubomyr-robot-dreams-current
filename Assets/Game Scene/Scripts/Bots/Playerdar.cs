using System;
using PhysX;
using StateMachineSystem.ServiceLocatorSystem;
using UnityEngine;

namespace BehaviourTreeSystem
{
    public class Playerdar : MonoBehaviour
    {
        public enum State
        {
            Scanning,
            Chasing,
            Searching,
            ChoosingAttack
        }

        [SerializeField] private EnemyController _enemyController;
        [SerializeField] private float _range;
        [SerializeField] private float _angle;
        [SerializeField] private float _meleeRange;

        private float _cosine;
        private Transform _transform;
        private IPlayerService _playerService;

        private TargetableBase _currentTarget;
        private bool _hasTarget;
        private bool _seesTarget;
        private bool _meleeTarget;
        private Vector3 _lastTargetPosition;

        private State _currentState;

        public State CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState == value)
                    return;
                _currentState = value;
                _enemyController.ComputeBehaviour();
            }
        }

        public TargetableBase CurrentTarget => _currentTarget;
        public bool HasTarget => _hasTarget;
        public bool SeesTarget => _seesTarget;
        public bool MeleeTarget => _meleeTarget;
        public Vector3 LastTargetPosition => _lastTargetPosition;
        public float MeleeRange => _meleeRange;
        public EnemyController EnemyController => _enemyController;

        private void Start()
        {
            _transform = transform;
            _playerService = ServiceLocator.Instance.GetService<IPlayerService>();
            _cosine = Mathf.Cos(_angle * Mathf.Deg2Rad);

            _enemyController.Health.OnHealthChanged += HealthChangedHandler;
        }

        private void FixedUpdate()
        {
            switch (CurrentState)
            {
                case State.Scanning:
                    ScanningUpdate();
                    break;
                case State.Chasing:
                    ChasingUpdate();
                    break;
                case State.Searching:
                    SearchingUpdate();
                    break;
                case State.ChoosingAttack:
                    CheckingDistance();
                    break;
            }

            UpdateMeleeTarget();
        }

        private void UpdateMeleeTarget()
        {
            float distance = Vector3.Distance(_playerService.Player.transform.position, transform.position);
            _meleeTarget = distance < _meleeRange;
        }

        private void CheckingDistance()
        {
            if (!CheckTarget(_playerService.Player.Targetable))
                return;

            UpdateMeleeTarget();
            float distance = Vector3.Distance(_playerService.Player.transform.position, transform.position);

            Debug.Log(_meleeTarget
                ? $"melee target, distance:{distance}, melee range:{_meleeRange}"
                : $"gun target, distance:{distance}, melee range:{_meleeRange}");

            CurrentState = State.Chasing;
        }

        private void ScanningUpdate()
        {
            if (!CheckTarget(_playerService.Player.Targetable))
                return;

            _currentTarget = _playerService.Player.Targetable;
            _hasTarget = true;
            _seesTarget = true;
            CurrentState = State.ChoosingAttack;
        }

        private void ChasingUpdate()
        {
            _lastTargetPosition = _currentTarget.TargetPivot.position;

            if (CheckTarget(_currentTarget))
                return;

            _seesTarget = false;
            CurrentState = State.Searching;
        }

        private void SearchingUpdate()
        {
            if (!CheckTarget(_currentTarget))
                return;

            _seesTarget = true;
            CurrentState = State.Chasing;
        }

        public void LookAround()
        {
            if (_hasTarget)
            {
                if (CheckTarget(_currentTarget, false))
                {
                    _seesTarget = true;
                    CurrentState = State.Chasing;
                }
                else
                {
                    _seesTarget = false;
                    CurrentState = State.Scanning;
                }
            }
            else
            {
                if (CheckTarget(_playerService.Player.Targetable, false))
                {
                    _hasTarget = true;
                    _seesTarget = true;
                    _currentTarget = _playerService.Player.Targetable;
                    CurrentState = State.Chasing;
                }
                else
                {
                    _seesTarget = false;
                    CurrentState = State.Scanning;
                }
            }
        }

        private bool CheckTarget(TargetableBase targetable, bool useFov = true)
        {
            Vector3 position = _transform.position;
            Vector3 playerPosition = targetable.TargetPivot.position;

            Vector3 playerDirection = Vector3.ProjectOnPlane(playerPosition - position, Vector3.up);

            if (playerDirection.sqrMagnitude > _range * _range)
                return false;

            playerDirection.Normalize();
            Vector3 forward = Vector3.ProjectOnPlane(_transform.forward, Vector3.up).normalized;

            if (useFov && Vector3.Dot(playerDirection, forward) < _cosine)
                return false;

            if (!Physics.Raycast(position, (playerPosition - position).normalized, out RaycastHit hit, _range))
                return false;

            return hit.collider == _playerService.Player.CharacterController;
        }

        private void HealthChangedHandler(int health)
        {
            LookAround();
        }
    }
}
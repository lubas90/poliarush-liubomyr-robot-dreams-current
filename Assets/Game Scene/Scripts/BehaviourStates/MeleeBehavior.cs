using System.Collections.Generic;
using StateMachineSystem;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTreeSystem.BehaviourStates
{
    public class MeleeBehaviour : BehaviourStateBase
    {
        public enum State
        {
            Approaching,
            Punching,
            Cooldown,
        }

        private readonly Transform _characterTransform;
        private readonly Transform _weaponTransform;
        private readonly NavMeshAgent _agent;
        private readonly CharacterController _characterController;
        private bool _didSwing;

        private State _state;
        private float _time;
        private float _inaccuracy;

        public MeleeBehaviour(StateMachine stateMachine, byte stateId, EnemyController enemyController)
            : base(stateMachine, stateId, enemyController)
        {
            _characterTransform = enemyController.CharacterTransform;
            _weaponTransform = enemyController.WeaponTransform;
            _agent = enemyController.NavMeshAgent;
            _characterController = enemyController.CharacterController;

            _agent.speed = enemyController.Data.ChaseSpeed;
        }

        public override void Enter()
        {
            base.Enter();
            ChangeState(State.Approaching);
            _inaccuracy = enemyController.Data.Inaccuracy;
            WeaponUp();
            _didSwing = false;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            switch (_state)
            {
                case State.Approaching:
                    UpdateApproaching(deltaTime);
                    break;
                case State.Punching:
                    UpdatePunching(deltaTime);
                    break;
                case State.Cooldown:
                    UpdateCooldown(deltaTime);
                    break;
            }
        }

        private void UpdateApproaching(float deltaTime)
        {
            _agent.SetDestination(enemyController.Playerdar.CurrentTarget.transform.position);
            Vector3 velocity = _agent.desiredVelocity;
            velocity.y = 0;

            Vector3 previousPosition = _characterTransform.position;
            _characterController.Move(velocity * (deltaTime * enemyController.Data.ChaseSpeed) + Physics.gravity);

            Vector3 newPosition = _characterTransform.position;
            Vector3 direction = newPosition - previousPosition;
            float distance = direction.magnitude;
            _agent.nextPosition = newPosition;
            enemyController.PatrolStamina -= distance;

            RotateTowards(newPosition);

            float actualRange = Vector3.Distance(_characterTransform.position, enemyController.Playerdar.CurrentTarget.transform.position);
            Debug.Log($"[ComputeBehaviour] HasTarget: {enemyController.Playerdar.HasTarget}, SeesTarget: {enemyController.Playerdar.SeesTarget}, MeleeTarget: {enemyController.Playerdar.MeleeTarget}, PatrolStamina: {enemyController.PatrolStamina:F2}, Range: {actualRange:F2}, MeleeRange: {enemyController.Playerdar.MeleeRange:F2}");
            enemyController.ComputeBehaviour();

            if (_agent.remainingDistance <= enemyController.Data.PunchDistance)
            {
                enemyController.Playerdar.LookAround();
                ChangeState(State.Punching);
            }
        }

        private void UpdatePunching(float deltaTime)
        {
            if (_time < enemyController.WeaponData.ReloadTime / 2)
            {
                WeaponFront();

                if (!_didSwing)
                {
                    enemyController.HitScanMelee.Swing();
                    _didSwing = true;
                }

                _time += deltaTime;
            }
            else if (_time < enemyController.WeaponData.ReloadTime)
            {
                WeaponUp();
                _time += deltaTime;
            }
            else
            {
                ChangeState(State.Cooldown);
            }
        }


        private void UpdateCooldown(float deltaTime)
        {
            _time += deltaTime;
            _didSwing = false;

            float actualRange = Vector3.Distance(_characterTransform.position, enemyController.Playerdar.CurrentTarget.transform.position);
            Debug.Log($"[ComputeBehaviour] HasTarget: {enemyController.Playerdar.HasTarget}, SeesTarget: {enemyController.Playerdar.SeesTarget}, MeleeTarget: {enemyController.Playerdar.MeleeTarget}, PatrolStamina: {enemyController.PatrolStamina:F2}, Range: {actualRange:F2}, MeleeRange: {enemyController.Playerdar.MeleeRange:F2}");
            enemyController.ComputeBehaviour();

            if (_time >= enemyController.WeaponData.ReloadTime)
            {
                ChangeState(State.Approaching);
            }
        }

        private void WeaponUp()
        {
            RotateTowards(enemyController.Playerdar.CurrentTarget.TargetPivot.position);
            _weaponTransform.rotation = Quaternion.LookRotation(Vector3.up);
        }

        private void WeaponFront()
        {
            RotateTowards(enemyController.Playerdar.CurrentTarget.TargetPivot.position);
            Vector3 weaponDirection = (GetAimingTarget(enemyController.Playerdar.CurrentTarget.transform.position, _inaccuracy) - _weaponTransform.position).normalized;
            _weaponTransform.rotation = Quaternion.LookRotation(weaponDirection);
        }

        private Vector3 GetAimingTarget(Vector3 playerPosition, float inaccuracyMultiplier)
        {
            Vector3 localPlayerPos = _characterTransform.InverseTransformPoint(playerPosition);
            float distance = localPlayerPos.z;
            float distanceFactor = 1f - distance / enemyController.Data.PreferredRange;
            float inaccuracyFactor = distanceFactor * enemyController.Data.InaccuracyFactor;
            inaccuracyMultiplier -= inaccuracyFactor;
            Vector2 randomOffset = Random.insideUnitCircle * inaccuracyMultiplier;
            localPlayerPos.x += randomOffset.x;
            localPlayerPos.y += randomOffset.y + (inaccuracyMultiplier - 0.5f);
            return _characterTransform.TransformPoint(localPlayerPos);
        }

        private void RotateTowards(Vector3 targetPosition)
        {
            Vector3 direction = Vector3.ProjectOnPlane(targetPosition - _characterTransform.position, Vector3.up).normalized;
            if (direction.sqrMagnitude > 0.01f)
            {
                _characterTransform.rotation = Quaternion.LookRotation(direction);
            }
        }

        private void ChangeState(State newState)
        {
            _state = newState;
            _time = 0f;
            Debug.Log($"[Melee] State changed to: {_state}");
        }

        public override void Dispose() { }
    }
}

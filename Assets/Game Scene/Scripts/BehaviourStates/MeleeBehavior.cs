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
        private readonly Transform _currentTarget;
        private readonly NavMeshAgent _agent;
        private readonly CharacterController _characterController;
        private State _state;
        private float _time;
        private int _charge;
        private float _inaccuracy;
        
        public MeleeBehaviour(StateMachine stateMachine, byte stateId, EnemyController enemyController) : base(stateMachine, stateId, enemyController)
        {
            _characterTransform = enemyController.CharacterTransform;
            _weaponTransform = enemyController.WeaponTransform;
            enemyController.NavMeshAgent.speed = enemyController.Data.ChaseSpeed;
            //_currentTarget = enemyController.Playerdar.CurrentTarget.transform;
            _agent = enemyController.NavMeshAgent;
            _characterController = enemyController.CharacterController;
        }

        public override void Enter()
        {
            base.Enter();
            _time = 0f;
            _state = State.Approaching;
            _inaccuracy = enemyController.Data.Inaccuracy;
            Debug.Log("Enter MeleeBehaviour: Aproaching");
            WeaponUp();
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            //deltaTime = 1;
            
            switch (_state)
            {
                case State.Approaching:
                    ApproachingUpdate(deltaTime);
                    break;
                case State.Punching:
                    PunchingUpdate(deltaTime);
                    break;
                case State.Cooldown:
                    CooldownUpdate(deltaTime);
                    break;
            }
        }



        private void ApproachingUpdate(float deltaTime)
        {
            enemyController.NavMeshAgent.SetDestination(enemyController.Playerdar.CurrentTarget.transform.position);
            Vector3 velocity = _agent.desiredVelocity;
            velocity.y = 0;
            //velocity.x = 0.1f;
            Vector3 position = _characterTransform.position;
           // _characterController.Move(velocity * deltaTime);
            _characterController.Move(velocity * (deltaTime * enemyController.Data.ChaseSpeed) + Physics.gravity);
            Vector3 newPosition = _characterTransform.position;
            Vector3 direction = newPosition - position;
            float distance = (newPosition - position).magnitude;
            _agent.nextPosition = newPosition;
            enemyController.PatrolStamina -= distance;
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            direction.Normalize();
            if (!Mathf.Approximately(direction.magnitude, 0f))
            {
                _characterTransform.rotation = Quaternion.LookRotation(direction);
            }

            if (_agent.remainingDistance <= enemyController.Data.PunchDistance)
            {
                enemyController.Playerdar.LookAround();
                _state = State.Punching;
                _time = 0f;
                Debug.Log("Change MeleeBehaviour: Punching");
            }
        }

        private void PunchingUpdate(float deltaTime)
        {
            if (_time < enemyController.WeaponData.ReloadTime / 2)
            {
                WeaponFront();
                _time += deltaTime;
            }
            else if (_time < enemyController.WeaponData.ReloadTime)
            {
                WeaponUp();
                _time += deltaTime;
            }
            else
            {
                _time = 0f;
                _state = State.Cooldown;
            }
        }
        
        private void CooldownUpdate (float deltaTime)
        {
            if (_time < enemyController.WeaponData.ReloadTime)
            {
                WeaponUp();
                _time += deltaTime;
            }
            else
            {
                _time = 0f;
                _state = State.Approaching;
            }
        }
        
        private void WeaponUp()
        {
            Vector3 direction = Vector3
                .ProjectOnPlane(
                    enemyController.Playerdar.CurrentTarget.TargetPivot.position - _characterTransform.position,
                    Vector3.up).normalized;

            _characterTransform.rotation = Quaternion.LookRotation(direction);

            Vector3 weaponDirection = Vector3.up;
            _weaponTransform.rotation = Quaternion.LookRotation(weaponDirection);
        }
        
        private void WeaponFront()
        {
            Vector3 direction = Vector3
                .ProjectOnPlane(
                    enemyController.Playerdar.CurrentTarget.TargetPivot.position - _characterTransform.position,
                    Vector3.up).normalized;

            _characterTransform.rotation = Quaternion.LookRotation(direction);

            Vector3 weaponDirection =
                (GetAimingTarget(enemyController.Playerdar.CurrentTarget.transform.position, _inaccuracy) - _weaponTransform.position).normalized;
            _weaponTransform.rotation = Quaternion.LookRotation(weaponDirection);
        }
        
        public Vector3 GetAimingTarget(Vector3 playerPosition, float inaccuracyMultiplier)
        {
            Vector3 localPlayerPos = _characterTransform.InverseTransformPoint(playerPosition);

            float distance = localPlayerPos.z;

            float distanceFactor = 1f - distance / enemyController.Data.PreferredRange;

            float inaccuracyFactor = distanceFactor * enemyController.Data.InaccuracyFactor;

            inaccuracyMultiplier = inaccuracyMultiplier - inaccuracyFactor;
            
            Vector2 randomOffset = Random.insideUnitCircle * inaccuracyMultiplier;

            localPlayerPos.x += randomOffset.x;
            localPlayerPos.y += randomOffset.y + (inaccuracyMultiplier - 0.5f);

            Vector3 targetPosition = _characterTransform.TransformPoint(localPlayerPos);

            return targetPosition;
        }
        
/*            _time += deltaTime;
            if (_time < enemyController.Data.AimTime)
                return;

            _inaccuracy = enemyController.Data.Inaccuracy;

            _charge = enemyController.WeaponData.MaxCharge;
            Shoot();
        }

        private void CooldownUpdate(float deltaTime)
        {
            _time += deltaTime;
            if (_time < enemyController.WeaponData.CooldownTime)
                return;
            Shoot();
        }

        private void ReloadUpdate(float deltaTime)
        {
            _time += deltaTime;
            if (_time < enemyController.WeaponData.ReloadTime)
                return;
            _charge = enemyController.WeaponData.MaxCharge;
            Shoot();
        }

        private void ShootUpdate(float deltaTime)
        {
            _time += deltaTime;
            if (_time < enemyController.Data.ShotDelay)
                return;

            enemyController.HitScanGun.Shoot();
            _time = 0f;
            _state = _charge > 0 ? State.Cooldown : State.Reload;
        }

        private void Shoot()
        {
            Vector3 playerPosition = enemyController.Playerdar.CurrentTarget.TargetPivot.position;
            Vector3 weaponDirection =
                (GetAimingTarget(playerPosition, _inaccuracy) - _weaponTransform.position).normalized;
            _weaponTransform.rotation = Quaternion.LookRotation(weaponDirection);

            _charge -= enemyController.WeaponData.ChargePerShot;
            _time = 0f;

            _inaccuracy = Mathf.Max(_inaccuracy - enemyController.Data.AccuracyPerShot, enemyController.Data.MinInaccuracy);

            _state = State.Shoot;
        }

        public Vector3 GetAimingTarget(Vector3 playerPosition, float inaccuracyMultiplier)
        {
            Vector3 localPlayerPos = _characterTransform.InverseTransformPoint(playerPosition);

            float distance = localPlayerPos.z;

            float distanceFactor = 1f - distance / enemyController.Data.PreferredRange;

            float inaccuracyFactor = distanceFactor * enemyController.Data.InaccuracyFactor;

            inaccuracyMultiplier = inaccuracyMultiplier - inaccuracyFactor;

            Vector2 randomOffset = Random.insideUnitCircle * inaccuracyMultiplier;

            localPlayerPos.x += randomOffset.x;
            localPlayerPos.y += randomOffset.y + (inaccuracyMultiplier - 0.5f);

            Vector3 targetPosition = _characterTransform.TransformPoint(localPlayerPos);

            return targetPosition;
        }

        
        */
        public override void Dispose()
        {
        }
    }
}
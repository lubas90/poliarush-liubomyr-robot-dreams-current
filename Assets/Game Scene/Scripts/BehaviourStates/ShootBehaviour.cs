using StateMachineSystem;
using UnityEngine;

namespace BehaviourTreeSystem.BehaviourStates
{
    public class ShootBehaviour : BehaviourStateBase
    {
        public enum State
        {
            Aiming,
            Cooldown,
            Reload,
            Shoot,
        }

        private readonly Transform _characterTransform;
        private readonly Transform _weaponTransform;

        private State _state;
        private float _time;
        private int _charge;
        private float _inaccuracy;

        public ShootBehaviour(StateMachine stateMachine, byte stateId, EnemyController enemyController) : base(stateMachine, stateId, enemyController)
        {
            _characterTransform = enemyController.CharacterTransform;
            _weaponTransform = enemyController.WeaponTransform;
        }

        public override void Enter()
        {
            base.Enter();
            _time = 0f;
            _state = State.Aiming;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (_state != State.Shoot)
                UpdateRotation();

            switch (_state)
            {
                case State.Aiming:
                    AimingUpdate(deltaTime);
                    break;
                case State.Cooldown:
                    CooldownUpdate(deltaTime);
                    break;
                case State.Reload:
                    ReloadUpdate(deltaTime);
                    break;
                case State.Shoot:
                    ShootUpdate(deltaTime);
                    break;
            }
        }

        private void AimingUpdate(float deltaTime)
        {
            _time += deltaTime;
            if (_time < enemyController.Data.AimTime)
                return;

            _inaccuracy = enemyController.Data.Inaccuracy;

            _charge = enemyController.WeaponData.MaxCharge;
            Shoot();
        }

        private void CooldownUpdate(float deltaTime)
        {
            _time += deltaTime;
            enemyController.ComputeBehaviour();
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

        private void UpdateRotation()
        {
            Vector3 direction = Vector3
                .ProjectOnPlane(
                    enemyController.Playerdar.CurrentTarget.TargetPivot.position - _characterTransform.position,
                    Vector3.up).normalized;

            _characterTransform.rotation = Quaternion.LookRotation(direction);

            Vector3 weaponDirection =
                (enemyController.Playerdar.CurrentTarget.TargetPivot.position - _weaponTransform.position).normalized;
            _weaponTransform.rotation = Quaternion.LookRotation(weaponDirection);
        }

        public override void Dispose()
        {
        }
    }
}

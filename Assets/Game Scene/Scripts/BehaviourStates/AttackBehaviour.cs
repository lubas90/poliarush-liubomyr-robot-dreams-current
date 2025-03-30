using StateMachineSystem;
using UnityEngine;

namespace BehaviourTreeSystem.BehaviourStates
{
    public class AttackBehaviour : BehaviourStateBase
    {
        private readonly Transform _characterTransform;
        private readonly Transform _weaponTransform;
        
        public AttackBehaviour(StateMachine stateMachine, byte stateId, EnemyController enemyController) : base(stateMachine, stateId, enemyController)
        {
            _characterTransform = enemyController.CharacterTransform;
            _weaponTransform = enemyController.WeaponTransform;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

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
using System.Collections.Generic;
using StateMachineSystem;
using UnityEngine;

namespace BehaviourTreeSystem.BehaviourStates
{
    public class IdleBehaviour : BehaviourStateBase
    {
        private float _time;
        private float _duration;
        
        public IdleBehaviour(StateMachine stateMachine, byte stateId, EnemyController enemyController) : base(stateMachine, stateId, enemyController)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _time = 0f;
            Vector2 durationBounds = enemyController.Data.IdleDuration;
            _duration = Random.Range(durationBounds.x, durationBounds.y);

            conditions = new List<IStateCondition>
                { new BaseCondition((byte)EnemyBehaviour.Deciding, CompletedCondition) };
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            _time += deltaTime;
        }

        public override void Exit()
        {
            base.Exit();
            enemyController.RestorePatrolStamina();
        }

        public override void Dispose()
        {
        }

        private bool CompletedCondition()
        {
            return _time >= _duration;
        }
    }
}
using System.Collections.Generic;
using StateMachineSystem;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTreeSystem.BehaviourStates
{
    public class SearchBehaviour : BehaviourStateBase
    {
        private readonly NavMeshAgent _agent;
        private readonly CharacterController _characterController;
        private readonly Transform _characterTransform;
        
        public SearchBehaviour(StateMachine stateMachine, byte stateId, EnemyController enemyController) : base(stateMachine, stateId, enemyController)
        {
            _agent = enemyController.NavMeshAgent;
            _characterController = enemyController.CharacterController;
            _characterTransform = enemyController.CharacterTransform;
        }

        public override void Enter()
        {
            base.Enter();

            enemyController.NavMeshAgent.speed = enemyController.Data.ChaseSpeed;
            enemyController.NavMeshAgent.SetDestination(enemyController.Playerdar.LastTargetPosition);

            conditions = new List<IStateCondition>
                { new BaseCondition((byte)EnemyBehaviour.Deciding, ArrivedCondition) };
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            Vector3 velocity = _agent.desiredVelocity;
            velocity.y = 0;
            Vector3 position = _characterTransform.position;
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

            if (_agent.remainingDistance <= enemyController.Data.LookAroundDistance)
            {
                enemyController.Playerdar.LookAround();
            }
        }

        public override void Dispose()
        {
        }

        private bool ArrivedCondition()
        {
            return _agent.pathPending || _agent.remainingDistance <= _agent.stoppingDistance;
        }
    }
}
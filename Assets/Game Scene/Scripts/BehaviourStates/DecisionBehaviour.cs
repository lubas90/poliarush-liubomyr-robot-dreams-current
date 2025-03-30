using StateMachineSystem;

namespace BehaviourTreeSystem.BehaviourStates
{
    public class DecisionBehaviour : BehaviourStateBase
    {
        public DecisionBehaviour(StateMachine stateMachine, byte stateId, EnemyController enemyController) : base(stateMachine, stateId, enemyController)
        {
        }

        public override void Enter()
        {
            base.Enter();
            enemyController.ComputeBehaviour();
        }

        public override void Dispose()
        {
        }
    }
}
using StateMachineSystem;

namespace BehaviourTreeSystem.BehaviourStates
{
    public abstract class BehaviourStateBase : StateBase
    {
        protected readonly EnemyController enemyController;
        
        protected BehaviourStateBase(StateMachine stateMachine, byte stateId, EnemyController enemyController) : base(stateMachine, stateId)
        {
            this.enemyController = enemyController;
        }
    }
}
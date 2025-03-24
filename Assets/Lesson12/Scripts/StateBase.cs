using System;
using System.Collections.Generic;

namespace StateMachineSystem
{
    public abstract class StateBase : IState
    {
        public byte StateId { get; }
        
        protected List<IStateCondition> conditions;
        
        private StateMachine _stateMachine;

        protected StateBase(StateMachine stateMachine, byte stateId)
        {
            _stateMachine = stateMachine;
            StateId = stateId;
        }
        
        public virtual void Enter()
        {
        }

        public void Update(float deltaTime)
        {
            if (conditions != null)
            {
                for (int i = 0; i < conditions.Count; ++i)
                {
                    IStateCondition condition = conditions[i];
                    if (!condition.Invoke())
                        continue;
                    _stateMachine.SetState(condition.State);
                    return;
                }
            }
            
            OnUpdate(deltaTime);
        }

        protected virtual void OnUpdate(float deltaTime)
        {
            
        }

        public virtual void Exit()
        {
        }
        
        public abstract void Dispose();
    }
}
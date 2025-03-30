using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachineSystem
{
    public class StateMachine : IDisposable
    {
        public Action<byte> OnStateChange;
        
        private readonly Dictionary<byte, IState> _states = new();

        private IState _currentState;

        public IState CurrentState
        {
            get => _currentState;
            private set
            {
                if (_currentState == value)
                    return;
                _currentState?.Exit();
                _currentState = value;
                _currentState?.Enter();
                OnStateChange?.Invoke(_currentState.StateId);
            }
        }

        public void InitState(byte stateID)
        {
            if (!_states.TryGetValue(stateID, out IState state))
            {
                Debug.LogWarning("State " + stateID + " not found");
                return;
            }
            _currentState = state;
            _currentState?.Enter();
        }
        
        public void ForceState(byte stateID)
        {
            if (!_states.TryGetValue(stateID, out IState state))
            {
                Debug.LogWarning("State " + stateID + " not found");
                return;
            }
            _currentState?.Exit();
            _currentState = state;
            _currentState?.Enter();
            OnStateChange?.Invoke(_currentState.StateId);
        }
        
        public void AddState(byte stateID, IState state)
        {
            _states.Add(stateID, state);
        }

        public void SetState(byte stateID)
        {
            if (!_states.TryGetValue(stateID, out IState state))
            {
                Debug.LogWarning("State " + stateID + " not found");
                return;
            }
            CurrentState = state;
        }

        public void Update(float deltaTime)
        {
            CurrentState?.Update(deltaTime);
        }

        public void Dispose()
        {
            foreach (IState state in _states.Values)
                state?.Dispose();
        }
    }
}
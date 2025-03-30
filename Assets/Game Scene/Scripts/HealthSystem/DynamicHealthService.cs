using System;
using MainMenu;
using StateMachineSystem.ServiceLocatorSystem;
using UnityEngine;

namespace BehaviourTreeSystem
{
    [DefaultExecutionOrder(-10)]
    public class DynamicHealthService : DynamicHealthSystem, IHealthService
    {
        public Type Type { get; } = typeof(IHealthService);
        
        protected override void Awake()
        {
            base.Awake();
            ServiceLocator.Instance.AddService(this);
        }

        protected virtual void OnDestroy()
        {
            ServiceLocator.Instance.RemoveService(this);
        }
    }
}
using System;
using UnityEngine;

namespace StateMachineSystem.ServiceLocatorSystem
{
    [DefaultExecutionOrder(-20)]
    public abstract class GlobalMonoServiceBase : MonoServiceBase
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}
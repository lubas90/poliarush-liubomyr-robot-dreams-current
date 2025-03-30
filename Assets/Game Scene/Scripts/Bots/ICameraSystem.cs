using StateMachineSystem.ServiceLocatorSystem;
using UnityEngine;

namespace BehaviourTreeSystem
{
    public interface ICameraService : IService
    {
        Camera Camera { get; }
    }
}
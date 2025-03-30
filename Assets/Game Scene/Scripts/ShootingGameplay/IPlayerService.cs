using StateMachineSystem.ServiceLocatorSystem;
using UnityEngine;

namespace BehaviourTreeSystem
{
    public interface IPlayerService : IService
    {
        PlayerController Player { get; }
        bool IsPlayer(Collider collider);
    }
}
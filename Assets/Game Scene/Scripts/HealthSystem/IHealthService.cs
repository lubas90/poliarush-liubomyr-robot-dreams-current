using MainMenu;
using StateMachineSystem.ServiceLocatorSystem;
using UnityEngine;

namespace BehaviourTreeSystem
{
    public interface IHealthService : IService
    {
        IHealth this[Collider collider] { get; }

        void AddCharacter(IHealth character);

        void RemoveCharacter(IHealth character);

        bool GetHealth(Collider characterController, out Dummies.Health health);
    }
}
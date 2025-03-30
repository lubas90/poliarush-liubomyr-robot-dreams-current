using System;
using UnityEngine;

namespace MainMenu
{
    public interface IHealth
    {
        event Action OnDeath;
        event Action<int> OnHealthChanged;
        event Action<float> OnHealthChanged01;
        CharacterController CharacterController { get; }
        int HealthValue { get; }

        bool IsAlive { get; }

        float HealthValue01 { get; }
        int MaxHealthValue { get; }

        void TakeDamage(int damage);

        void Heal(int heal);

        void SetHealth(int health);
    }
}
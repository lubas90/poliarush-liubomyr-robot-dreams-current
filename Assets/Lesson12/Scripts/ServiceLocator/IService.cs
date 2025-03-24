using System;

namespace StateMachineSystem.ServiceLocatorSystem
{
    public interface IService
    {
        Type Type { get; }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachineSystem.ServiceLocatorSystem
{
    [DefaultExecutionOrder(-30)]
    public class ServiceLocator : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeServiceLocator()
        {
            GameObject serviceLocator = new GameObject("ServiceLocator");
            Instance = serviceLocator.AddComponent<ServiceLocator>();
            DontDestroyOnLoad(serviceLocator);
        }
        
        public static ServiceLocator Instance { get; private set; }

        private Dictionary<Type, IService> _services = new();
        
        public void AddService(IService service)
        {
            _services.Add(service.Type, service);
        }

        public void AddServiceExplicit(Type type, IService service)
        {
            _services.Add(type, service);
        }
        
        public void RemoveService(IService service)
        {
            _services.Remove(service.Type);
        }
        
        public bool RemoveServiceExplicit(Type type, IService service)
        {
            if (_services.TryGetValue(type, out IService serviceValue) && serviceValue == service)
            {
                return _services.Remove(type);
            }
            return false;
        }

        public T GetService<T>() where T : class, IService
        {
            _services.TryGetValue(typeof(T), out IService service);
            return service as T;
        }
    }
}
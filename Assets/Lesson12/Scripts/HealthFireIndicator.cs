using System.Collections.Generic;
using UnityEngine;

namespace Dummies
{
    public class HealthFireIndicator : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private GameObject _firePrefab; // Fire effect prefab
        [SerializeField] private Transform _parentObject; // Parent object where fires will be attached
        [SerializeField] private Transform _headObject; // Head object for fire when health is low
        [SerializeField] private int _maxFireInstances = 10;
        [SerializeField] private Vector3 _spawnAreaSize; // Defines the area where fire can spawn
        
        private float _targetHealth;
        private List<GameObject> _activeFires = new List<GameObject>();
        private GameObject _headFireInstance;
        private Vector3 _headfirePosition;

        private void Start()
        {
            _targetHealth = _health.HealthValue01;
            _health.OnHealthChanged01 += HealthChangedHandler;
        }

        private void Update()
        {
            AdjustFireEffects();
        }

        private void HealthChangedHandler(float health)
        {
            _targetHealth = health;
        }

        private void AdjustFireEffects()
        {
            int targetFireCount = Mathf.RoundToInt((1 - _targetHealth) * _maxFireInstances);

            // Remove fire prefabs if health is regenerating
            while (_activeFires.Count > targetFireCount)
            {
                Destroy(_activeFires[0]);
                _activeFires.RemoveAt(0);
            }

            // Add fire prefabs if health is decreasing
            while (_activeFires.Count < targetFireCount)
            {
                SpawnFire();
            }

            // Add fire on head if health is below 25%
            if (_targetHealth <= 0.25f && _headFireInstance == null && _headObject != null)
            {
                _headfirePosition = new Vector3(_headObject.position.x, _headObject.position.y + 1, _headObject.position.z);
                _headFireInstance = Instantiate(_firePrefab, _headfirePosition, Quaternion.identity, _headObject);
            }
            else if (_targetHealth > 0.25f && _headFireInstance != null)
            {
                Destroy(_headFireInstance);
                _headFireInstance = null;
            }
        }

        private void SpawnFire()
        {
            if (_firePrefab == null || _parentObject == null)
                return;

            Vector3 randomOffset = new Vector3(
                Random.Range(-_spawnAreaSize.x / 2, _spawnAreaSize.x / 2),
                Random.Range(-_spawnAreaSize.y / 2, _spawnAreaSize.y / 2),
                Random.Range(-_spawnAreaSize.z / 2, _spawnAreaSize.z / 2)
            );

            GameObject newFire = Instantiate(_firePrefab, _parentObject.position + randomOffset, Quaternion.identity, _parentObject);
            _activeFires.Add(newFire);
        }
    }
}
using System;
using System.Collections.Generic;
using Dummies;
using MainMenu;
using StateMachineSystem.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace BehaviourTreeSystem
{
    public class RoomController : MonoBehaviour, INavPointProvider
    {
        [SerializeField] private Vector3 _roomExtends;
        [SerializeField] private Vector3 _roomOffset;
        [SerializeField] private EnemyController _enemyController;
        [SerializeField] private int _maxEnemies;
        [SerializeField] private float _spawnDelay;

        private List<EnemyController> _enemies;
        
        private readonly Vector3[] _gizmosPoints = new Vector3[4];

        private Vector3 _point;
        private NavMeshHit _hit;

        private IHealthService _healthService;
        private ICameraService _cameraSystem;
        
        private float _time;
        
        private void Start()
        {
            _healthService = ServiceLocator.Instance.GetService<IHealthService>();
            _cameraSystem = ServiceLocator.Instance.GetService<ICameraService>();
            _enemies = new List<EnemyController>(_maxEnemies);
            
            _time = 0f;
            SpawnEnemies(_maxEnemies);
        }

        private void Update()
        {
            if (_time < _spawnDelay)
            {
                _time += Time.deltaTime;
                return;
            }

            _time = 0f;
            SpawnEnemies(_maxEnemies - _enemies.Count);
        }

        private void SpawnEnemies(int count)
        {
            for (int i = 0; i < count; ++i)
                SpawnEnemy();
        }
        
        [ContextMenu("GetPoint")]
        private void GetPointInternal()
        {
            Vector3 center = transform.position + _roomOffset;
            Vector3 min = center - _roomExtends;
            Vector3 max = center + _roomExtends;
            _point.x = Random.Range(min.x, max.x);
            _point.y = Random.Range(min.y, max.y);
            _point.z = Random.Range(min.z, max.z);
            NavMesh.SamplePosition(_point, out _hit, 1.0f, NavMesh.AllAreas);
        }

        public Vector3 GetPoint()
        {
            GetPointInternal();
            return _hit.position;
        }
        
        [ContextMenu("Spawn Enemy")]
        private void SpawnEnemy()
        {
            GetPointInternal();

            while (!_hit.hit)
            {
                GetPointInternal();
            }

            
            EnemyController enemy = Instantiate(_enemyController, _hit.position, Quaternion.identity);
            enemy.Initialize(this, Camera.current);
            
            _healthService.AddCharacter(enemy.Health);
            enemy.Health.OnDeath += () => EnemyDeathHandler(enemy);
            
            _enemies.Add(enemy);
            //Debug.Log(_enemies.Count);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Vector3 center = transform.position + _roomOffset;
            
            Vector3 position = center;
            position.x += _roomExtends.x;
            position.z += _roomExtends.z;
            _gizmosPoints[0] = position;
            
            position = center;
            position.x += _roomExtends.x;
            position.z -= _roomExtends.z;
            _gizmosPoints[1] = position;
            
            position = center;
            position.x -= _roomExtends.x;
            position.z -= _roomExtends.z;
            _gizmosPoints[2] = position;
            
            position = center;
            position.x -= _roomExtends.x;
            position.z += _roomExtends.z;
            _gizmosPoints[3] = position;
            
            Gizmos.DrawLineStrip(_gizmosPoints, true);
            
            Gizmos.color = _hit.hit ? Color.blue : Color.red;
            
            Gizmos.DrawSphere(_hit.hit ? _hit.position : _point, 0.33f);
        }

        private void EnemyDeathHandler(EnemyController enemy)
        {
            _enemies.Remove(enemy);
        }
    }
}
using System;
using UnityEngine;

namespace MainMenu
{
    public class DummyTrack : MonoBehaviour
    {
        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _endPosition;

        private Transform _transform;
        private Vector3 _worldStartPosition;
        private Vector3 _worldEndPosition;
        
        private void Awake()
        {
            _transform = transform;
            _worldStartPosition = _transform.TransformPoint(_startPosition);
            _worldEndPosition = _transform.TransformPoint(_endPosition);
        }
        
        public Vector3 Evaluate(float time)
        {
            int cycles = Mathf.FloorToInt(time);
            time = (cycles % 2) switch
            {
                0 => time - cycles,
                1 => 1f - (time - cycles),
                _ => throw new ArgumentOutOfRangeException()
            };
            return Vector3.Lerp(_worldStartPosition, _worldEndPosition, time);
        }
        
        private void OnDrawGizmosSelected()
        {
            Vector3 startPosition = transform.TransformPoint(_startPosition);
            Vector3 endPosition = transform.TransformPoint(_endPosition);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(startPosition, 0.2f);
            Gizmos.DrawSphere(endPosition, 0.2f);
            Gizmos.DrawLine(startPosition, endPosition);
        }
    }
}
using System;
using System.Collections.Generic;
using BehaviourTreeSystem.BehaviourStates;
using UnityEngine;

namespace BehaviourTreeSystem
{
    public class EnemyRimController : MonoBehaviour
    {
        private static readonly int RimColor = Shader.PropertyToID("_RimColor");

        [Serializable]
        private struct RimColorData
        {
            public EnemyBehaviour state;
            public Color color;
        }
        
        [SerializeField] private EnemyController _enemyController;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private RimColorData[] _rimColors;
        
        private Dictionary<EnemyBehaviour, Color> _colors = new();
        
        private void Start()
        {
            _renderer.material.SetColor(RimColor, Color.black);
            
            _colors.Clear();
            for (int i = 0; i < _rimColors.Length; ++i)
            {
                RimColorData rimColor = _rimColors[i];
                _colors.Add(rimColor.state, rimColor.color);
            }

            _enemyController.BehaviourMachine.OnStateChange += SetRimColor;
        }

        private void SetRimColor(byte state)
        {
            _renderer.material.SetColor(RimColor, _colors[(EnemyBehaviour)state]);
        }
    }
}
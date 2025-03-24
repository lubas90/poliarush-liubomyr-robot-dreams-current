using System;
using System.Collections.Generic;
using StateMachineSystem.Locomotion;
using UnityEngine;

namespace StateMachineSystem.VisualEffects
{
    public class RimController : MonoBehaviour
    {
        private static readonly int RimColor = Shader.PropertyToID("_RimColor");

        [Serializable]
        private struct RimColorData
        {
            public Locomotion.LocomotionState state;
            public Color color;
        }
        
        [SerializeField] private LocomotionController _locomotionController;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private RimColorData[] _rimColors;
        
        private Dictionary<Locomotion.LocomotionState, Color> _colors = new();
        
        private void Awake()
        {
            _renderer.material.SetColor(RimColor, Color.black);
            
            _colors.Clear();
            for (int i = 0; i < _rimColors.Length; ++i)
            {
                RimColorData rimColor = _rimColors[i];
                _colors.Add(rimColor.state, rimColor.color);
            }

            _locomotionController.OnStateChanged += SetRimColor;
        }

        private void SetRimColor(LocomotionState state)
        {
            _renderer.material.SetColor(RimColor, _colors[state]);
        }
    }
}
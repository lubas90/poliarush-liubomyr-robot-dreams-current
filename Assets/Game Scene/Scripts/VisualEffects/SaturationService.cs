using System;
using StateMachineSystem.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BehaviourTreeSystem.VisualEffects
{
    public class SaturationService : MonoServiceBase, ISaturationService
    {
        [SerializeField] private float _regularSaturation;
        [SerializeField] private float _deathSaturation;
        [SerializeField] private float _blendSpeed;
        [SerializeField] private Volume _volume;

        private ColorAdjustments _colorAdjustments;
        private float _saturation;
        private float _targetSaturation;

        public override Type Type { get; } = typeof(ISaturationService);

        protected override void Awake()
        {
            base.Awake();
            _volume.sharedProfile.TryGet(out _colorAdjustments);
            _colorAdjustments.saturation.value = _targetSaturation = _saturation = _regularSaturation;
        }

        private void Update()
        {
            float newSaturation = Mathf.MoveTowards(_saturation, _targetSaturation, _blendSpeed * Time.deltaTime);
            if (newSaturation == _saturation)
                return;
            _saturation = newSaturation;
            _colorAdjustments.saturation.value = _saturation;
        }

        public void SetDeathSaturation()
        {
            _targetSaturation = _deathSaturation;
        }
    }
}
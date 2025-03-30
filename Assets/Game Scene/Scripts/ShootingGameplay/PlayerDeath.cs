using System;
using System.Collections;
using BehaviourTreeSystem.VisualEffects;
using MainMenu;
using StateMachineSystem.ServiceLocatorSystem;
using UnityEngine;
using Lesson7;

namespace BehaviourTreeSystem
{
    public class PlayerDeath : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private Transform _fallMark;
        [SerializeField] private AnimationCurve _fallCurve;
        [SerializeField] private GameObject _rootObject;
        [SerializeField] private float _healthBarDelayTime;

        private Vector3 _fallMarkPosition;
        private Quaternion _fallMarkRotation;
        private DummyTrack _dummyTrack;

        private float _time;

        private void Start()
        {
            _health.OnDeath += DeathHandler;
            
            _fallMark.GetLocalPositionAndRotation(out _fallMarkPosition, out _fallMarkRotation);
        }

        private void DeathHandler()
        {
            StartCoroutine(DelayedDestroy());
        }

        private IEnumerator DelayedDestroy()
        {
            float time = 0f;
            float reciprocal = 1f / _healthBarDelayTime;

            while (time < _healthBarDelayTime)
            {

                EvaluateFall(time * reciprocal);
                yield return null;
                time += Time.deltaTime;
            }

            EvaluateFall(1f);

            //Destroy(_rootObject);

            ServiceLocator.Instance.GetService<Lesson7.InputController>().enabled = false;
            ServiceLocator.Instance.GetService<ISaturationService>().SetDeathSaturation();

        }

        private void EvaluateFall(float progress)
        {
            float curveFactor = _fallCurve.Evaluate(progress);
            Vector3 position = Vector3.Lerp(Vector3.zero, _fallMarkPosition, curveFactor);
            Quaternion rotation = Quaternion.Slerp(Quaternion.identity, _fallMarkRotation, curveFactor);
            _bodyTransform.SetLocalPositionAndRotation(position, rotation);
        }
    }
}
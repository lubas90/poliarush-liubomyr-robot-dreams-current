using StateMachineSystem;
using UnityEngine;

namespace BehaviourTreeSystem.BehaviourStates
{
    public class DeathBehaviour : BehaviourStateBase
    {
        private readonly Vector3 _fallMarkPosition;
        private readonly Quaternion _fallMarkRotation;
        private readonly Vector3 _regularPosition;
        private readonly Quaternion _regularRotation;

        private float _time;
        private float _reciprocal;
        
        public DeathBehaviour(StateMachine stateMachine, byte stateId, EnemyController enemyController) : base(stateMachine, stateId, enemyController)
        {
            enemyController.FallMark.GetLocalPositionAndRotation(out _fallMarkPosition, out _fallMarkRotation);
            enemyController.MeshRendererTransform.GetLocalPositionAndRotation(out _regularPosition, out _regularRotation);
        }

        public override void Enter()
        {
            base.Enter();
            _time = 0f;
            _reciprocal = 1f / enemyController.Data.HealthBarDelayTime;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (_time < enemyController.Data.HealthBarDelayTime)
            {
                _time += deltaTime;
                EvaluateFall(_time * _reciprocal);
                return;
            }
            
            EvaluateFall(1f);

            Object.Destroy(enemyController.RootObject);
        }

        public override void Dispose()
        {
        }
        
        private void EvaluateFall(float progress)
        {
            float curveFactor = enemyController.Data.FallCurve.Evaluate(progress);
            Vector3 position = Vector3.Lerp(_regularPosition, _fallMarkPosition, curveFactor);
            Quaternion rotation = Quaternion.Slerp(_regularRotation, _fallMarkRotation, curveFactor);
            enemyController.MeshRendererTransform.SetLocalPositionAndRotation(position, rotation);
        }
    }
}
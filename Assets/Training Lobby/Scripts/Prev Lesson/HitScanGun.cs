using System;
using System.Collections;
using UnityEngine;
using Lesson7;

namespace Shooting
{
    public class HitScanGun : MonoBehaviour
    {
        public event Action<Collider> OnHit;
        public event Action OnShot;
        
        [SerializeField] protected HitscanShotAspect _shotPrefab;
        [SerializeField] protected Transform _muzzleTransform;
        [SerializeField] protected GameObject _flamePrefab;
        [SerializeField] protected float _decaySpeed;
        [SerializeField] protected Vector3 _shotScale;
        [SerializeField] protected float _shotRadius;
        [SerializeField] protected float _shotVisualDiameter;
        [SerializeField] protected string _tilingName;
        [SerializeField] protected float _range;
        [SerializeField] protected LayerMask _layerMask;
        [SerializeField] private Transform _gunTransform;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _rayDistance;
        [SerializeField] private LayerMask _rayMask;

        private Vector3 _hitPoint;
        protected int _tilingId;

        protected virtual void Start()
        {
            _tilingId = Shader.PropertyToID(_tilingName);
        }

        protected void OnEnable()
        {
            InputController.OnPrimaryInput += PrimaryInputHandler;
        }

        protected void OnDisable()
        {
            InputController.OnPrimaryInput -= PrimaryInputHandler;
        }

        protected virtual void PrimaryInputHandler(bool enableH)
        {
            if (enableH)
            {
                Vector3 muzzlePosition = _muzzleTransform.position;
                Vector3 muzzleForward = _muzzleTransform.forward;
                Ray ray = new Ray(muzzlePosition, muzzleForward);
                Vector3 hitPoint = muzzlePosition + muzzleForward * _range;
                if (Physics.SphereCast(ray, _shotRadius, out RaycastHit hitInfo, _range, _layerMask))
                {
                    Vector3 directVector = hitInfo.point - _muzzleTransform.position;
                    Vector3 rayVector = Vector3.Project(directVector, ray.direction);
                    hitPoint = muzzlePosition + rayVector;

                    OnHit?.Invoke(hitInfo.collider);
                }

                // Instantiate flame effect at muzzle
                GameObject flame = Instantiate(_flamePrefab, _muzzleTransform.position, _muzzleTransform.rotation, _muzzleTransform);
                
                // Instantiate the laser shot
                HitscanShotAspect shot = Instantiate(_shotPrefab, hitPoint, _muzzleTransform.rotation);
                shot.distance = (hitPoint - _muzzleTransform.position).magnitude;
                shot.outerPropertyBlock = new MaterialPropertyBlock();
                StartCoroutine(ShotRoutine(shot, flame));

                OnShot?.Invoke();
            }
        }
        
        private void FixedUpdate()
        {
            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            _hitPoint = _cameraTransform.position + _cameraTransform.forward * _rayDistance;
            if (Physics.Raycast(ray, out RaycastHit hitInfo, _rayDistance, _rayMask))
                _hitPoint = hitInfo.point;
            _gunTransform.LookAt(_hitPoint);
        }

        protected IEnumerator ShotRoutine(HitscanShotAspect shot, GameObject flame)
        {
            float interval = _decaySpeed * Time.deltaTime;
            while (shot.distance >= interval)
            {
                EvaluateShot(shot);
                yield return null;
                shot.distance -= interval;
                interval = _decaySpeed * Time.deltaTime;
            }

            Destroy(shot.gameObject);
            Destroy(flame); // Destroy the flame effect after the shot object is destroyed
        }

        protected void EvaluateShot(HitscanShotAspect shot)
        {
            shot.Transform.localScale = new Vector3(_shotScale.x, _shotScale.y, shot.distance * 0.5f);
            Vector4 tiling = Vector4.one;
            tiling.y = shot.distance * 0.5f / _shotVisualDiameter;
            shot.outerPropertyBlock.SetVector(_tilingId, tiling);
            shot.Outer.SetPropertyBlock(shot.outerPropertyBlock);
        }
    }
}
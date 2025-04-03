using System;
using UnityEngine;

namespace BehaviourTreeSystem
{
    public class HitScanMelee : MonoBehaviour
    {
        [SerializeField] private float _range = 2f;
        [SerializeField] private float _radius = 1f;
        [SerializeField] private LayerMask _layerMask;

        public event Action<Collider> OnHit;

        public void Swing()
        {
            Ray ray = new Ray(transform.position, transform.forward);

            if (Physics.SphereCast(ray, _radius, out RaycastHit hitInfo, _range, _layerMask))
            {
                Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.red, 1f);
                OnHit?.Invoke(hitInfo.collider);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * _range, Color.yellow, 1f);
            }
        }
    }

}
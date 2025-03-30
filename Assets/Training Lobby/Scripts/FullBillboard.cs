using UnityEngine;

namespace Dummies
{
    
    public class FullBillboard : BillboardBase
    {
        private Transform _cameraTransform;
        private Transform _transform;
        private Camera _camera;
        
        public override void SetCamera(Camera camera)
        {
            _camera = camera;
            _transform = camera.transform;
            _cameraTransform = _camera.transform;
        }

        private void LateUpdate()
        {
//            Vector3 direction = (_camera.transform.position - _transform.position).normalized;
  //          _transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}
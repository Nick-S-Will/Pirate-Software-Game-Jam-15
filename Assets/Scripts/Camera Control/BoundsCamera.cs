using Cinemachine;
using UnityEngine;

namespace ShadowAlchemy.CameraControl
{
    public class BoundsCamera : CinemachineVirtualCamera
    {
        [SerializeField] private Bounds bounds = new(Vector3.zero, Vector3.one);
        [Header("Debug")]
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private bool showBounds;

        [ContextMenu("Move Bounds To Transform")]
        private void MoveBoundsToTransform()
        {
            bounds.center = transform.position;
        }

        public float DistanceTo(Vector3 point) => Vector3.Distance(transform.position, point);

        public bool CloserThan(BoundsCamera boundsCamera, Vector3 point) => DistanceTo(point) <= boundsCamera.DistanceTo(point);

        public bool Contains(Vector3 point) => bounds.Contains(point);

        private void OnDrawGizmosSelected()
        {
            if (!showBounds) return;

            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
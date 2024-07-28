using Cinemachine;
using UnityEngine;

namespace ShadowAlchemy.CameraControl
{
    public class BoundsCamera : CinemachineVirtualCamera
    {
        [SerializeField] private Bounds localBounds = new(Vector3.zero, Vector3.one);
        [Header("Debug")]
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private bool showBounds;

        public Bounds Bounds => new(transform.position + localBounds.center, localBounds.size);

        public float DistanceTo(Vector3 point) => Vector3.Distance(transform.position, point);

        public bool CloserThan(BoundsCamera boundsCamera, Vector3 point) => DistanceTo(point) <= boundsCamera.DistanceTo(point);

        public bool Contains(Vector3 point) => Bounds.Contains(point);

        private void OnDrawGizmosSelected()
        {
            if (!showBounds) return;

            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        }
    }
}
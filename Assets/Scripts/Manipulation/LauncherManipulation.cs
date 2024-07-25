using UnityEngine;
using UnityEngine.Events;

namespace ShadowAlchemy.Manipulation
{
    public class LauncherManipulation : ManipulationBehaviour
    {
        [SerializeField] private Rigidbody[] projectiles;
        [SerializeField] private Vector3 localLaunchDirection = Vector3.up;
        [SerializeField][Min(0f)] private float launchForce = 1f, maxLaunchDelta = 0.1f;
        [Space]
        public UnityEvent OnLaunch, OnLoad;
        [Header("Debug")]
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private bool showLaunchDirection;

        private float[] projectileStartDeltaMagnitudes;

        private Vector3 LaunchDirection => (transform.rotation * localLaunchDirection).normalized;

        protected override void Awake()
        {
            base.Awake();

            if (projectiles.Length == 0) Debug.LogWarning($"{nameof(projectiles)} array is empty");

            projectileStartDeltaMagnitudes = new float[projectiles.Length];
            for (int i = 0; i < projectiles.Length; i++)
            {
                projectileStartDeltaMagnitudes[i] = GetDeltaMagnitudeFor(projectiles[i].transform);
            }
        }

        protected override void Use()
        {
            TryLaunch();
            OnLaunch.Invoke();
        }

        protected override void Restore()
        {
            OnLoad.Invoke();
        }

        private void TryLaunch()
        {
            for (int i = 0; i < projectiles.Length; i++)
            {
                if (InLaunchRange(i)) projectiles[i].AddForce(launchForce * LaunchDirection, ForceMode.Impulse);
            }
        }

        private bool InLaunchRange(int projectileIndex)
        {
            var currentDeltaMagnitude = GetDeltaMagnitudeFor(projectiles[projectileIndex].transform);
            return currentDeltaMagnitude - projectileStartDeltaMagnitudes[projectileIndex] <= maxLaunchDelta;
        }

        private float GetDeltaMagnitudeFor(Transform projectileTransform)
        {
            return (Quaternion.Inverse(transform.rotation) * (projectileTransform.position - transform.position)).magnitude;
        }

        private void OnDrawGizmosSelected()
        {
            if (!showLaunchDirection) return;

            var startPoint = transform.position;
            var endPoint = startPoint + LaunchDirection;
            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(startPoint, endPoint);
        }
    }
}
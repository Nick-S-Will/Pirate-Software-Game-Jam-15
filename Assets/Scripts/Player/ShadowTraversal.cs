using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ShadowAlchemy.Player
{
    public class ShadowTraversal : MonoBehaviour
    {
        [Header("Gameplay")]
        [SerializeField][Min(0f)] private float moveSpeed = 1f;
        [Header("Physics Checks")]
        [SerializeField][Min(0f)] private float surfaceOffset = .005f;
        [SerializeField][Range(0f, 180f)] private float maxSlopeAngle = 50f;
        [SerializeField] private LayerMask obstacleMask;
        [Header("Events")]
        public UnityEvent OnShadowExited;
        [Header("Debug")]
        [SerializeField] private Color lightGizmoColor = Color.white;
        [SerializeField] private Color shadowGizmoColor = Color.green, outOfRangeGizmoColor = Color.black;
        [SerializeField] private bool showLastShadowCheck;

        private List<Light> lights = new();
        private Vector2 moveInput;
        private Vector3 targetPosition = Vector2.positiveInfinity;
        private List<RaycastHit> shadeCastHits = new();

        /// <summary>
        /// Array of colliders that currently creating shade for this
        /// </summary>
        public Collider[] ShadeColliders
        {
            get
            {
                if (!enabled || !InShadow) return new Collider[0];

                return shadeCastHits.Select(hitInfo => hitInfo.collider).Where(collider => collider != null).ToArray();
            }
        }
        public bool InShadow
        {
            get
            {
                var inShadow = enabled && PointIsInShadow(transform.position);
                if (enabled && !inShadow) OnShadowExited.Invoke();

                return inShadow;
            }
        }

        private void Awake()
        {
            lights.AddRange(FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        }

        private void FixedUpdate()
        {
            TryMove();
        }

        public void SetMoveInput(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        #region Movement Check
        private void TryMove()
        {
            if (moveInput == Vector2.zero) return;

            var moveDelta = CalculateMoveDelta();
            targetPosition = transform.position + moveDelta;
            if (!CanMove(moveDelta, out RaycastHit hitInfo) && !CanClimb(hitInfo) || !PointIsInShadow(targetPosition)) return;

            if (hitInfo.collider)
            {
                transform.position = hitInfo.point + surfaceOffset * hitInfo.normal;
                transform.up = hitInfo.normal;
            }
            else transform.position = targetPosition;
        }

        private Vector3 CalculateMoveDelta()
        {
            var localSpaceInput = new Vector3(moveInput.x, 0f, moveInput.y);
            var worldSpaceInput = transform.rotation * Vector3.ProjectOnPlane(Camera.main.transform.rotation * localSpaceInput, Vector3.up);
            return moveSpeed * Time.fixedDeltaTime * worldSpaceInput.normalized;
        }

        private bool CanMove(Vector3 moveDelta, out RaycastHit obstacleHit)
        {
            var targetPosition = transform.position + moveDelta;
            if (!Physics.Raycast(targetPosition, -transform.up, out obstacleHit, 2 * surfaceOffset, obstacleMask)) return false;

            return !Physics.Raycast(transform.position, moveDelta, out obstacleHit, moveDelta.magnitude, obstacleMask);
        }

        private bool CanClimb(RaycastHit obstacleHit)
        {
            if (obstacleHit.collider == null) return false; 

            var angleToSurface = Vector3.Angle(Vector3.up, obstacleHit.normal);
            return angleToSurface <= maxSlopeAngle;
        }
        #endregion

        #region Shadow Check
        private bool PointIsInShadow(Vector3 point)
        {
            shadeCastHits.Clear();

            var canMove = true;
            foreach (var light in lights) if (PointIsVisibleFrom(point, light)) canMove = false;

            return canMove;
        }

        private bool PointIsVisibleFrom(Vector3 point, Light light)
        {
            RaycastHit hitInfo = default;
            if (!light.enabled)
            {
                shadeCastHits.Add(hitInfo);
                return false;
            }

            var isVisible = false;
            switch (light.type)
            {
                case LightType.Spot:
                    if (PointOutOfRange(point, light) || PointOutOfAngle(point, light)) break;
                    var distanceToSpot = Vector3.Distance(point, light.transform.position);
                    isVisible = !Physics.Raycast(point, light.transform.position - point, out hitInfo, distanceToSpot, obstacleMask);
                    break;
                case LightType.Directional:
                    isVisible = !Physics.Raycast(point, -light.transform.forward, out hitInfo, float.MaxValue, obstacleMask);
                    break;
                case LightType.Point:
                    if (PointOutOfRange(point, light)) break;
                    var distanceToPoint = Vector3.Distance(point, light.transform.position);
                    isVisible = !Physics.Raycast(point, light.transform.position - point, out hitInfo, distanceToPoint, obstacleMask);
                    break;
                default:
                    Debug.LogWarning($"Light type \"{light.type}\" isn't implemented");
                    break;
            }
            shadeCastHits.Add(hitInfo);

            return isVisible;
        }

        private bool PointOutOfRange(Vector3 point, Light light) => Vector3.Distance(point, light.transform.position) > light.range;
        private bool PointOutOfAngle(Vector3 point, Light light) => Vector3.Angle(light.transform.forward, point - light.transform.position) > light.spotAngle / 2;
        #endregion

        #region Debug
        private void OnDrawGizmos()
        {
            if (showLastShadowCheck && Application.isPlaying && targetPosition != Vector3.positiveInfinity)
            {
                for (int i = 0; i < lights.Count && i < shadeCastHits.Count; i++)
                {
                    DrawLightGizmo(lights[i], shadeCastHits[i]);
                }
            }
        }

        private void DrawLightGizmo(Light light, RaycastHit hitInfo)
        {
            if (!light.enabled) return;

            Vector3 endPoint;
            if (hitInfo.collider)
            {
                endPoint = hitInfo.point;
                Gizmos.color = shadowGizmoColor;
            }
            else
            {
                endPoint = light.transform.position;
                Gizmos.color = lightGizmoColor;
                switch (light.type)
                {
                    case LightType.Spot:
                        if (PointOutOfRange(targetPosition, light) || PointOutOfAngle(targetPosition, light)) Gizmos.color = outOfRangeGizmoColor;
                        break;
                    case LightType.Directional:
                        endPoint = targetPosition - Camera.main.farClipPlane * light.transform.forward;
                        break;
                    case LightType.Point:
                        if (PointOutOfRange(targetPosition, light)) Gizmos.color = outOfRangeGizmoColor;
                        break;
                    default:
                        endPoint = targetPosition;
                        break;
                }
            }

            Gizmos.DrawLine(targetPosition, endPoint);
        }
        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShadowAlchemy.Player
{
    public class ShadowTraversal : MonoBehaviour
    {
        [Header("Gamplay")]
        [SerializeField][Min(0f)] private float moveSpeed = 1f;
        [Header("Shadow Check")]
        [SerializeField] private LayerMask obstacleMask;
        [Header("Debug")]
        [SerializeField] private Color lightGizmoColor = Color.white;
        [SerializeField] private Color shadowGizmoColor = Color.green, outOfRangeGizmoColor = Color.black;
        [SerializeField] private bool showLastShadowCheck;

        private List<Light> lights = new();
        private Vector2 moveInput;
        private Vector3 targetPosition = Vector2.positiveInfinity;
        private List<RaycastHit> castHits = new();

        private Vector3 MoveDelta
        {
            get
            {
                var localSpaceInput = new Vector3(moveInput.x, 0f, moveInput.y);
                var worldSpaceInput = Vector3.ProjectOnPlane(Camera.main.transform.rotation * localSpaceInput, Vector3.up);
                return moveSpeed * Time.fixedDeltaTime * worldSpaceInput.normalized;
            }
        }

        /// <summary>
        /// Array of colliders that currently creating shade for this
        /// </summary>
        public Collider[] ShadeColliders
        {
            get
            {
                if (!InShadow) return new Collider[0];

                return castHits.Select(hitInfo => hitInfo.collider).Where(collider => collider != null).ToArray();
            }
        }
        public bool InShadow => PointIsInShadow(transform.position);

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

        private void TryMove()
        {
            if (moveInput == Vector2.zero) return;

            targetPosition = transform.position + MoveDelta;
            if (!PointIsInShadow(targetPosition)) return;

            transform.position = targetPosition;
        }

        private bool PointIsInShadow(Vector3 point)
        {
            castHits.Clear();

            var canMove = true;
            foreach (var light in lights) if (PointIsVisibleFrom(point, light)) canMove = false;

            return canMove;
        }

        private bool PointIsVisibleFrom(Vector3 point, Light light)
        {
            RaycastHit hitInfo = default;
            if (!light.enabled)
            {
                castHits.Add(hitInfo);
                return false;
            }

            var isVisible = false;
            switch (light.type)
            {
                case LightType.Spot:
                    if (PointOutOfRange(point, light) || PointOutOfAngle(point, light)) break;
                    isVisible = !Physics.Raycast(point, light.transform.position - point, out hitInfo, float.MaxValue, obstacleMask);
                    break;
                case LightType.Directional:
                    isVisible = !Physics.Raycast(point, -light.transform.forward, out hitInfo, float.MaxValue, obstacleMask);
                    break;
                case LightType.Point:
                    if (PointOutOfRange(point, light)) break;
                    isVisible = !Physics.Raycast(point, light.transform.position - point, out hitInfo, float.MaxValue, obstacleMask);
                    break;
                default:
                    Debug.LogWarning($"Light type \"{light.type}\" isn't implemented");
                    break;
            }
            castHits.Add(hitInfo);

            return isVisible;
        }

        private bool PointOutOfRange(Vector3 point, Light light) => Vector3.Distance(point, light.transform.position) > light.range;
        private bool PointOutOfAngle(Vector3 point, Light light) => Vector3.Angle(light.transform.forward, point - light.transform.position) > light.spotAngle / 2;

        #region Debug
        private void OnDrawGizmos()
        {
            if (showLastShadowCheck && Application.isPlaying && targetPosition != Vector3.positiveInfinity)
            {
                for (int i = 0; i < lights.Count && i < castHits.Count; i++)
                {
                    DrawLightGizmo(lights[i], castHits[i]);
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
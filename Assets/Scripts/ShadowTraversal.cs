using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShadowAlchemy.Player
{
    public class ShadowTraversal : MonoBehaviour
    {
        [Header("Gamplay")]
        [SerializeField][Min(0f)] private float moveSpeed = 1f;
        [Header("Shadow Check")]
        [SerializeField] private List<Light> lights;
        [SerializeField] private LayerMask obstacleMask;
        [Header("Debug")]
        [SerializeField] private Color lightGizmoColor = Color.white;
        [SerializeField] private Color shadowGizmoColor = Color.green, outOfRangeGizmoColor = Color.black;
        [SerializeField] private bool showLastShadowCheck;

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

        public bool InShadow => CanMoveTo(transform.position);

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
            if (!CanMoveTo(targetPosition)) return;

            transform.position = targetPosition;
        }

        private bool CanMoveTo(Vector3 targetPosition)
        {
            castHits.Clear();

            var canMove = true;
            foreach (var light in lights) if (PointIsVisibleFrom(targetPosition, light)) canMove = false;

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
            }
            castHits.Add(hitInfo);

            return isVisible;
        }

        private bool PointOutOfRange(Vector3 point, Light light) => Vector3.Distance(point, light.transform.position) > light.range;
        private bool PointOutOfAngle(Vector3 point, Light light) => Vector3.Angle(light.transform.forward, point - light.transform.position) > light.spotAngle / 2;

        private void OnDrawGizmos()
        {
            if (showLastShadowCheck && Application.isPlaying && targetPosition != Vector3.positiveInfinity)
            {
                var point = targetPosition;
                for (int i = 0; i < lights.Count && i < castHits.Count; i++)
                {
                    var light = lights[i];
                    var hitInfo = castHits[i];
                    if (!light.enabled) continue;

                    Vector3 endPoint;
                    if (hitInfo.collider)
                    {
                        endPoint = hitInfo.point;
                        Gizmos.color = shadowGizmoColor;
                    }
                    else
                    {
                        Gizmos.color = lightGizmoColor;
                        switch (light.type)
                        {
                            case LightType.Spot:
                                endPoint = light.transform.position;
                                if (PointOutOfRange(point, light) || PointOutOfAngle(point, light)) Gizmos.color = outOfRangeGizmoColor;
                                break;
                            case LightType.Directional:
                                endPoint = point - Camera.main.farClipPlane * light.transform.forward;
                                break;
                            case LightType.Point:
                                endPoint = light.transform.position;
                                if (PointOutOfRange(point, light)) Gizmos.color = outOfRangeGizmoColor;
                                break;
                            default:
                                endPoint = point;
                                break;
                        }
                    }

                    Gizmos.DrawLine(point, endPoint);
                }
            }
        }
    }
}
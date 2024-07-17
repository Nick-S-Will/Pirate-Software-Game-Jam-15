using UnityEngine;
using UnityEngine.InputSystem;

namespace ShadowAlchemy.Player
{
    public class ShadowTraversal : MonoBehaviour
    {
        [Header("Gamplay")]
        [SerializeField][Min(0f)] private float moveSpeed = 1f;
        [Header("Shadow Check")]
        [SerializeField] private Light sun;
        [SerializeField] private LayerMask obstacleMask;
        [Header("Debug")]
        [SerializeField] private Color mainGizmoColor = Color.white;
        [SerializeField] private Color altGizmoColor = Color.green;
        [SerializeField] private bool showLastShadowCheck;

        private Vector2 moveInput;
        private Vector3 targetPosition = Vector2.positiveInfinity;
        private RaycastHit lastShadowCheckInfo;

        private Vector3 MoveDelta
        {
            get
            {
                var localSpaceInput = new Vector3(moveInput.x, 0f, moveInput.y);
                var worldSpaceInput = Vector3.ProjectOnPlane(Camera.main.transform.rotation * localSpaceInput, Vector3.up);
                return moveSpeed * Time.fixedDeltaTime * worldSpaceInput.normalized;
            }
        }

        private void Awake()
        {
            if (sun == null) Debug.LogError($"{nameof(sun)} is not assigned");
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

            var moveDelta = MoveDelta;
            if (!CanMoveBy(moveDelta)) return;

            transform.position = transform.position + moveDelta;
        }

        private bool CanMoveBy(Vector3 moveDelta)
        {
            targetPosition = transform.position + moveDelta;
            var directionToSun = -sun.transform.forward;
            return Physics.Raycast(targetPosition, directionToSun, out lastShadowCheckInfo, float.MaxValue, obstacleMask);
        }

        private void OnDrawGizmos()
        {
            if (showLastShadowCheck && Application.isPlaying && targetPosition != Vector3.positiveInfinity)
            {
                bool inShadow = lastShadowCheckInfo.collider;
                Gizmos.color = inShadow ? altGizmoColor : mainGizmoColor;

                var startPoint = targetPosition;
                var endPoint = inShadow ? lastShadowCheckInfo.point : startPoint + Camera.main.farClipPlane * -sun.transform.forward;
                Gizmos.DrawLine(startPoint, endPoint);
            }
        }
    }
}
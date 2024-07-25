using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ShadowAlchemy.Manipulation
{
    public class DoorManipulation : ManipulationBehaviour
    {
        [Space]
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField][Min(0f)] private float rotationAngle = 90f, rotationSpeed = 90f;
        [Space]
        public UnityEvent OnOpenDoor, OnCloseDoor;
        [Header("Debug")]
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private bool showRotationAxis;

        private Coroutine useDoorRoutine;

        public override bool CanBeManipulated { get => useDoorRoutine == null; protected set { } }

        protected override void Awake()
        { 
            base.Awake();

            if (rigidbody == null) Debug.LogError($"{nameof(rigidbody)} not assigned");
        }

        protected override void Use() => UseDoor(true);

        protected override void Restore() => UseDoor(false);

        private void UseDoor(bool open) => useDoorRoutine ??= StartCoroutine(UseDoorRoutine(open));
        private IEnumerator UseDoorRoutine(bool open)
        {
            if (open) OnOpenDoor.Invoke();
            else OnCloseDoor.Invoke();

            var startRotation = rigidbody.rotation;
            var axis = open ? rotationAxis : -rotationAxis;
            var endRotation = startRotation * Quaternion.AngleAxis(rotationAngle, axis);

            while (rigidbody.rotation != endRotation)
            {
                var rotation = Quaternion.RotateTowards(rigidbody.rotation, endRotation, rotationSpeed * Time.fixedDeltaTime);
                rigidbody.MoveRotation(rotation);

                yield return new WaitForFixedUpdate();
            }

            useDoorRoutine = null;
            OnCanBeManipulated.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            if (!showRotationAxis) return;
            if (rigidbody == null)
            {
                Debug.LogWarning($"{nameof(rigidbody)} must be assigned to show {nameof(rotationAxis)}");
                return;
            }

            var startPoint = rigidbody.position;
            var endPoint = startPoint + (rigidbody.rotation * rotationAxis).normalized;
            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(startPoint, endPoint);
        }
    }
}
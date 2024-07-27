using UnityEngine;
using UnityEngine.Events;

namespace ShadowAlchemy.Manipulation
{
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleManipulation : ManipulationBehaviour
    {
        public enum VehicleState { Idle, Forward, Reverse }

        [Header("Drive Variables")]
        [SerializeField][Min(0f)] private float speed = 2f;
        [Header("Physics Checks")]
        [SerializeField] private Transform frontCastPoint;
        [SerializeField] private Transform backCastPoint;
        [SerializeField][Min(0f)] private float stopDistance = .1f;
        [Space]
        public UnityEvent OnDrive, OnStop;
        [Header("Debug")]
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private bool showForwardDirection;
        
        private new Rigidbody rigidbody;
        private Vector3 velocity;
        private VehicleState state = VehicleState.Idle;

        public VehicleState State
        {
            get => state;
            private set
            {
                state = value;

                if (state == VehicleState.Idle)
                {
                    velocity = Vector3.zero;
                    OnStop.Invoke();
                    return;
                }
                
                var direction = state == VehicleState.Forward ? 1 : -1;
                velocity = direction * speed * transform.forward;
                OnDrive.Invoke();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (frontCastPoint == null) Debug.LogError($"{nameof(frontCastPoint)} not assigned");
            if (backCastPoint == null) Debug.LogError($"{nameof(backCastPoint)} not assigned");

            rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (State == VehicleState.Idle) return;

            var castPoint = State == VehicleState.Forward ? frontCastPoint : backCastPoint;
            if (Physics.Raycast(castPoint.position, castPoint.forward, stopDistance)) State = VehicleState.Idle;
            else rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
        }

        protected override void Use() => State = VehicleState.Forward;
        protected override void Restore() => State = VehicleState.Reverse;
        
        private void OnDrawGizmosSelected()
        {
            if (!showForwardDirection) return;

            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }
}
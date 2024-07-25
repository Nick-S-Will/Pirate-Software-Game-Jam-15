using ShadowAlchemy.Manipulation;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ShadowAlchemy.Player
{
    [RequireComponent(typeof(ShadowTraversal))]
    public class ShadowManipulation : MonoBehaviour
    {
        public UnityEvent OnTargetChanged, OnTargetCanBeManipulated, OnManipulated, OnManipulateFailed;

        private ShadowTraversal shadowTraversal;
        private ManipulationBehaviour target;

        public ManipulationBehaviour Target 
        {
            get => target;
            private set
            {
                if (target == value) return;

                if (target)
                {
                    target.OnCanBeManipulated.RemoveListener(PropogateEvent);
                    target.StopHighlight();
                }
                target = value;
                if (target)
                {
                    target.OnCanBeManipulated.AddListener(PropogateEvent);
                    target.Highlight();
                }

                OnTargetChanged.Invoke();
            }
        }

        private void Awake()
        {
            shadowTraversal = GetComponent<ShadowTraversal>();
        }

        private void OnDisable()
        {
            Target = null;
        }

        private void Update()
        {
            LookForTarget();
        }

        #region Look For Target
        private void LookForTarget()
        {
            var shadeColliders = shadowTraversal.ShadeColliders;
            if (shadeColliders.Length == 0)
            {
                Target = null;
                return;
            }

            var manipulationTargets = shadeColliders.Select(collider => collider.GetComponent<ManipulationBehaviour>()).Where(target => target != null).ToArray();
            if (manipulationTargets.Length == 0)
            {
                Target = null;
                return;
            }

            Target = GetClosestTarget(manipulationTargets);
        }

        private ManipulationBehaviour GetClosestTarget(ManipulationBehaviour[] targets)
        {
            var index = 0;
            var minDistance = float.MaxValue;
            for (int i = 0; i < targets.Count(); i++)
            {
                var distance = Vector3.Distance(transform.position, targets[i].transform.position);
                if (distance < minDistance)
                {
                    index = i;
                    minDistance = distance;
                }
            }

            return targets[index];
        }
        #endregion

        private void PropogateEvent() => OnTargetCanBeManipulated.Invoke();

        public void Manipulate(InputAction.CallbackContext context)
        {
            if (!enabled || !context.performed) return;

            if (Target && Target.CanBeManipulated)
            {
                Target.Manipulate();
                OnManipulated.Invoke();
            }
            else OnManipulateFailed.Invoke();
        }
    }
}
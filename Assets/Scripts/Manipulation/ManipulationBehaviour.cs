using UnityEngine;
using UnityEngine.Events;

namespace ShadowAlchemy.Manipulation
{
    [RequireComponent(typeof(Collider))]
    public abstract class ManipulationBehaviour : MonoBehaviour
    {
        [SerializeField] private string useActionName = "Use";
        [SerializeField] private string restoreActionName = "Restore";
        [Space]
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] [Range(0f, 1f)] private float highlightColorBias = .5f;

        private Material material;
        private Color startColor;
        private bool wasUsed, canBeManipulated = true;

        [HideInInspector] public UnityEvent OnCanBeManipulated;

        public string ActionName => wasUsed ? restoreActionName : useActionName;
        public virtual bool CanBeManipulated
        {
            get => canBeManipulated;
            protected set
            {
                var invokeEvent = !canBeManipulated && value;

                canBeManipulated = value;

                if (invokeEvent) OnCanBeManipulated.Invoke();
            }
        }

        protected virtual void Awake()
        {
            if (meshRenderer == null) Debug.LogError($"{nameof(meshRenderer)} not assigned");

            material = meshRenderer.material;
            startColor = material.color;
        }

        public void Highlight()
        {
            material.color = Color.Lerp(startColor, highlightColor, highlightColorBias);
        }

        public void StopHighlight()
        {
            material.color = startColor;
        }

        public void Manipulate()
        {
            if (!CanBeManipulated) return;

            if (wasUsed) Restore();
            else Use();

            wasUsed = !wasUsed;
        }
        protected abstract void Use();
        protected abstract void Restore();
    }
}
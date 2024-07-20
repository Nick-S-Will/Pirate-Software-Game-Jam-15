using UnityEngine;

namespace ShadowAlchemy.Manipulation
{
    [RequireComponent(typeof(Collider))]
    public abstract class ManipulationBehaviour : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] [Range(0f, 1f)] private float highlightColorBias = .5f;
        [Space]
        [SerializeField] private string useActionName = "Use";
        [SerializeField] private string restoreActionName = "Restore";

        private Color startColor;

        private bool wasUsed;

        public string ActionName => wasUsed ? restoreActionName : useActionName;

        private void Awake()
        {
            if (meshRenderer == null) Debug.LogError($"{nameof(meshRenderer)} not assigned");

            startColor = meshRenderer.material.color;
        }

        public void Highlight()
        {
            meshRenderer.material.color = Color.Lerp(startColor, highlightColor, highlightColorBias);
        }

        public void StopHighlight()
        {
            meshRenderer.material.color = startColor;
        }

        public void Manipulate()
        {
            if (wasUsed) Restore();
            else Use();

            wasUsed = !wasUsed;
        }
        protected abstract void Use();
        protected abstract void Restore();
    }
}
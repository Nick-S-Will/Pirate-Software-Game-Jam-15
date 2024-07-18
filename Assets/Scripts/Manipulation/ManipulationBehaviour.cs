using UnityEngine;

namespace ShadowAlchemy.Manipulation
{
    [RequireComponent(typeof(Collider))]
    public abstract class ManipulationBehaviour : MonoBehaviour
    {
        [SerializeField] private string useActionName = "Use", restoreActionName = "Restore";

        private bool wasUsed;

        public string ActionName => wasUsed ? restoreActionName : useActionName;

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
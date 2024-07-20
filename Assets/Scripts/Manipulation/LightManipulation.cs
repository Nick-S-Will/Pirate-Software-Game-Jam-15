using UnityEngine;

namespace ShadowAlchemy.Manipulation
{
    public class LightManipulation : ManipulationBehaviour
    {
        [Space]
        [SerializeField] private new Light light;

        private void Awake()
        {
            if (light == null) Debug.LogError($"{nameof(light)} not assigned");

            Manipulate();
        }

        protected override void Use()
        {
            light.enabled = true;
        }

        protected override void Restore()
        {
            light.enabled = false;
        }
    }
}
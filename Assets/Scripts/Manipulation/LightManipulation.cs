using UnityEngine;
using UnityEngine.Events;

namespace ShadowAlchemy.Manipulation
{
    public class LightManipulation : ManipulationBehaviour
    {
        [Space]
        [SerializeField] private new Light light;
        [Space]
        public UnityEvent OnEnableLight, OnDisableLight;

        protected override void Awake()
        {
            base.Awake();

            if (light == null) Debug.LogError($"{nameof(light)} not assigned");

            Manipulate();
        }

        protected override void Use()
        {
            light.enabled = true;
            OnEnableLight.Invoke();
        }

        protected override void Restore()
        {
            light.enabled = false;
            OnDisableLight.Invoke();
        }
    }
}
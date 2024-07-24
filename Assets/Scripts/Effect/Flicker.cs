using UnityEngine;

namespace ShadowAlchemy.Effect
{
    public class Flicker : MonoBehaviour
    {
        private const string EMISSION_KEYWORD = "_EMISSION", EMISSION_PROPERTY = "_EmissionColor";

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField][Min(0f)] private int materialIndex;
        [SerializeField] private new Light light;
        [Space]
        [SerializeField][Min(0f)] private float speedChangeInterval = 1f;
        [SerializeField][Min(0f)] private float minSpeed = .5f, maxSpeed = 1f, minScale = .8f, maxScale = 1f;

        private Material material;
        private Color startEmissionColor;
        private float startIntensity, lastSpeedChangeTime, speed, pingPongSample;

        private void Awake()
        {
            if (light == null) Debug.LogError($"{nameof(light)} not assigned");
            if (meshRenderer == null) Debug.LogError($"{nameof(meshRenderer)} not assigned");

            material = meshRenderer.materials[materialIndex];
            startEmissionColor = material.GetColor(EMISSION_PROPERTY);
            startIntensity = light.intensity;
            lastSpeedChangeTime = -speedChangeInterval;
        }

        private void OnEnable()
        {
            material.EnableKeyword(EMISSION_KEYWORD);
            light.enabled = true;
        }

        private void OnDisable()
        {
            material.DisableKeyword(EMISSION_KEYWORD);
            light.enabled = false;
        }

        private void Update()
        {
            if (Time.time >= lastSpeedChangeTime + speedChangeInterval)
            {
                speed = Random.Range(minSpeed, maxSpeed);
                lastSpeedChangeTime = Time.time;
            }

            var scale = Mathf.PingPong(pingPongSample += speed * Time.deltaTime, maxScale - minScale) + minScale;
            material.SetColor(EMISSION_PROPERTY, scale * startEmissionColor);
            light.intensity = scale * startIntensity;
        }
    }
}
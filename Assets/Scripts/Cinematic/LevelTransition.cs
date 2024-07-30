using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ShadowAlchemy.Cinematic
{
    public class LevelTransition : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [Header("Transition")]
        [SerializeField][Min(0f)] private float transitionRadius = 1f;
        [SerializeField] private Image fadeImage;
        [SerializeField][Min(0f)] private float fadeTime = 1f;
        [SerializeField] private GameObject uIParent;
        [SerializeField][Min(0f)] private float waitTime = 5f;
        [SerializeField] private string transitionSceneName = "Scene";
        [Space]
        public UnityEvent OnTransition;
        [Header("Debug")]
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private bool showTransitionRadius;

        private Coroutine transitionRoutine;

        private void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, playerInput.transform.position) <= transitionRadius) Transition();
        }

        private void Transition() => transitionRoutine ??= StartCoroutine(TransitionRoutine());

        private IEnumerator TransitionRoutine()
        {
            OnTransition.Invoke();

            fadeImage.enabled = true;
            var color = fadeImage.color;
            var fadeTimeElapsed = 0f;
            while (fadeTimeElapsed < fadeTime)
            {
                fadeImage.color = new Color(color.r, color.g, color.b, fadeTimeElapsed / fadeTime);

                fadeTimeElapsed += Time.deltaTime;

                yield return null;
            }
            fadeImage.color = new Color(color.r, color.g, color.b, 1f);

            playerInput.enabled = false;
            uIParent.SetActive(true);

            yield return new WaitForSeconds(waitTime);

            transitionRoutine = null;

            SceneManager.LoadScene(transitionSceneName);
        }

        private void OnDrawGizmosSelected()
        {
            if (!showTransitionRadius) return;

            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, transitionRadius);
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ShadowAlchemy.Cinematic
{
    public class StartCinematic : MonoBehaviour
    {
        [SerializeField] private Light startLight;
        [SerializeField] private AudioSource startAudio;
        [SerializeField][Min(0f)] private float delayAfterAudio = 1f;
        [SerializeField] private GameObject[] objectsToActivate;
        [SerializeField] private PlayerInput playerInput;
        [Space]
        public UnityEvent OnStartCinematic, OnCinematicFinished;

        private Coroutine cinematicRoutine;

        private void Awake()
        {
            if (startLight == null) Debug.LogError($"{nameof(startLight)} not assigned");
            if (startAudio == null) Debug.LogError($"{nameof(startAudio)} not assigned");
            if (playerInput == null) Debug.LogError($"{nameof(playerInput)} not assigned");
        }

        public void Play() => cinematicRoutine ??= StartCoroutine(CinematicRoutine());

        private IEnumerator CinematicRoutine()
        {
            OnStartCinematic.Invoke();

            startLight.enabled = true;
            startAudio.Play();

            yield return new WaitWhile(() => startAudio.isPlaying);
            yield return new WaitForSeconds(delayAfterAudio);

            foreach (var go in objectsToActivate) go.SetActive(true);
            playerInput.enabled = true;

            OnCinematicFinished.Invoke();

            cinematicRoutine = null;
        }
    }
}
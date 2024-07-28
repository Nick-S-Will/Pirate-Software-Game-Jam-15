using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShadowAlchemy.Cinematic
{
    public class StartCinematic : MonoBehaviour
    {
        [SerializeField] private Light startLight;
        [SerializeField] private AudioSource startAudio;
        [SerializeField] private PlayerInput playerInput;

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
            startLight.enabled = true;
            startAudio.Play();

            yield return new WaitWhile(() => startAudio.isPlaying);

            playerInput.enabled = true;

            cinematicRoutine = null;
        }
    }
}
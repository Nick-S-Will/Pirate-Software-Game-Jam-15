using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ShadowAlchemy.Cinematic
{
    public class EndCinematic : MonoBehaviour
    {
        [Header("Transition")]
        [SerializeField] private Image fadeImage;
        [SerializeField][Min(0f)] private float fadeTime = 1f;
        [SerializeField][Min(0f)] private float waitTime = 5f;
        [Header("Ending Audio")]
        [SerializeField] private AudioSource basementCircuitEndingAudio;
        [SerializeField] private AudioSource allCircuitEndingAudio;
        [Space]
        [SerializeField] private string titleSceneName = "BasementScene";
        [Space]
        public UnityEvent OnStartCinematic, OnFadeEnded, OnAudioEnded;

        private Coroutine endingRoutine;

        public void PlayBasementCircuitEnding() => endingRoutine ??= StartCoroutine(EndingRoutine(true));

        public void PlayAllCircuitEnding() => endingRoutine ??= StartCoroutine(EndingRoutine(false));

        private IEnumerator EndingRoutine(bool isBasementEnding)
        {
            OnStartCinematic.Invoke();

            yield return StartCoroutine(FadeOut());

            OnFadeEnded.Invoke();

            yield return new WaitForSeconds(waitTime);

            var audioSource = (isBasementEnding ? basementCircuitEndingAudio : allCircuitEndingAudio);
            audioSource.Play();
            yield return new WaitWhile(() => audioSource.isPlaying);

            OnAudioEnded.Invoke();

            yield return new WaitForSeconds(waitTime);

            endingRoutine = null;

            SceneManager.LoadScene(titleSceneName);
        }

        private IEnumerator FadeOut()
        {
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
        }
    }
}
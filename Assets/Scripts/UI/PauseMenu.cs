using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ShadowAlchemy.UI
{
    public class PauseMenu : MonoBehaviour
    {
        private static bool skipTitle;

        [SerializeField] private GameObject pauseMenuParent;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private string titleSceneName = "BasementScene";
        [Space]
        public UnityEvent OnPauseToggled, OnRestartTitleScene;

        public bool IsPaused => pauseMenuParent.activeSelf;

        private void Awake()
        {
            if (pauseMenuParent == null) Debug.LogError($"{nameof(pauseMenuParent)} not assigned");
            if (playerInput == null) Debug.LogError($"{nameof(playerInput)} not assigned");

            if (skipTitle)
            {
                OnRestartTitleScene.Invoke();
                skipTitle = false;
            }
        }

        public void Pause(InputAction.CallbackContext context)
        {
            if (context.performed) Pause();
        }

        public void Pause() => SetPaused(true);

        public void Resume() => SetPaused(false);

        public void SetPaused(bool paused)
        {
            if (paused == IsPaused) return;

            pauseMenuParent.SetActive(paused);
            playerInput.enabled = !paused;

            OnPauseToggled.Invoke();
        }

        public void RestartFloor()
        {
            var activeSceneName = SceneManager.GetActiveScene().name;
            if (activeSceneName == titleSceneName) skipTitle = true;
            SceneManager.LoadScene(activeSceneName);
        }

        public void Quit()
        {
            SceneManager.LoadScene(titleSceneName);
        }
    }
}
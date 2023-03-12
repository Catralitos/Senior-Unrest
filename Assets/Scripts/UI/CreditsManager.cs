using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Application = UnityEngine.Device.Application;

namespace UI
{
    public class CreditsManager : MonoBehaviour
    {
        public Button replayButton;
        public Button exitButton;
        private AudioManager _audioManager;

        private void Start()
        {
            replayButton.onClick.AddListener(ReplayGame);
            exitButton.onClick.AddListener(ExitGame);
            _audioManager = GetComponent<AudioManager>();
            _audioManager.Play("MenuMusic");
        }

        private static void ReplayGame()
        {
            SceneManager.LoadScene(1);
        }

        private static void ExitGame()
        {
            Application.Quit();
        }
    }
}
using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// This class manages the title screen.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class TitleScreenManager : MonoBehaviour
    {
        /// <summary>
        /// The start game button
        /// </summary>
        [Header("Buttons")] public Button startButton;

        /// <summary>
        /// The tutorial button
        /// </summary>
        public Button tutorialButton;
        /// <summary>
        /// The back button
        /// </summary>
        public Button backButton;
        /// <summary>
        /// The exit button
        /// </summary>
        public Button exitButton;

        /// <summary>
        /// The title screen
        /// </summary>
        [Header("Screens")] public GameObject titleScreen;

        /// <summary>
        /// The tutorial screen
        /// </summary>
        public GameObject tutorialScreen;

        /// <summary>
        /// The audio manager
        /// </summary>
        private AudioManager _audioManager;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            startButton.onClick.AddListener(StartGame);
            tutorialButton.onClick.AddListener(ShowTutorial);
            backButton.onClick.AddListener(ShowTitleScreen);
            exitButton.onClick.AddListener(ExitGame);
            _audioManager = GetComponent<AudioManager>();
            _audioManager.Play("MenuMusic");
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        private static void StartGame()
        {
            SceneManager.LoadScene(1);
        }

        /// <summary>
        /// Shows the tutorial.
        /// </summary>
        private void ShowTutorial()
        {
            //We hide the title screen and display the tutorial
            titleScreen.SetActive(false);
            tutorialScreen.SetActive(true);
        }

        /// <summary>
        /// Shows the title screen.
        /// </summary>
        private void ShowTitleScreen()
        {
            //We hide the tutorial and display the title screen
            tutorialScreen.SetActive(false);
            titleScreen.SetActive(true);
        }

        /// <summary>
        /// Exits the game.
        /// </summary>
        private static void ExitGame()
        {
            Application.Quit();
        }
    }
}
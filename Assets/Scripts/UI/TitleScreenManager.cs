using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class TitleScreenManager : MonoBehaviour
    {
        [Header("Buttons")]
        public Button startButton;
        public Button tutorialButton;
        public Button backButton;
        public Button exitButton;

        [Header("Screens")] 
        public GameObject titleScreen;
        public GameObject tutorialScreen;

        private void Start()
        {
            startButton.onClick.AddListener(StartGame);
            tutorialButton.onClick.AddListener(ShowTutorial);
            backButton.onClick.AddListener(ShowTitleScreen);
            exitButton.onClick.AddListener(ExitGame);
        }

        private void StartGame()
        {
            SceneManager.LoadScene(1);
        }

        private void ShowTutorial()
        {
            titleScreen.SetActive(false);
            tutorialScreen.SetActive(true);
        }

        private void ShowTitleScreen()
        {
            tutorialScreen.SetActive(false);
            titleScreen.SetActive(true);
        }

        private void ExitGame()
        {
            Application.Quit();
        }
        
        
    }
}

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wizards;
using Wizards.Animations;
using Wizards.Behaviours;
using Wizards.Behaviours.Lose;

namespace MainMenu
{
    public class MenuPause : MonoBehaviour
    {
        public WizardStateController _wizardStateController;
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button menuGameButton;
        [SerializeField] private Button exitGameButton;

        private void Start()
        {
            _wizardStateController = WizardStateController.Instance;
        }

        private void OnEnable()
        {
            resumeButton?.onClick.AddListener(() => StartGame());
            menuGameButton?.onClick.AddListener(() =>
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("MainMenu");
            });
            exitGameButton?.onClick.AddListener(() => Application.Quit());
        }
        
        private void Update()
        {
            // Если нажали ESC то меню пауза
            if (Input.GetKeyDown(KeyCode.Escape) &&  _pauseMenu != null)
            {
                _pauseMenu?.SetActive(true);
                Time.timeScale = 0;
            }
        }

        private void StartGame()
        {
            // продолжить игру
            Time.timeScale = 1;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wizards;
using Wizards.Behaviours;

namespace MainMenu
{
    public class MenuPause : MonoBehaviour
    {
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button menuGameButton;
        [SerializeField] private Button exitGameButton;

        private void OnEnable()
        {
            resumeButton?.onClick.AddListener(() => StartGame());
            menuGameButton?.onClick.AddListener(() =>
            {
                WizardsAI.Instance.StopAllStage();
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            });
            exitGameButton?.onClick.AddListener(() => Application.Quit());
        }
        
        private void Update()
        {
            // Если нажали ESC то меню пауза
            if (Input.GetKeyDown(KeyCode.Escape))
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
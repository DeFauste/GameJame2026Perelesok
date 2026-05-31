using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wizards;

namespace MainMenu
{
    public class MenuPause : MonoBehaviour
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button menuGameButton;
        [SerializeField] private Button exitGameButton;

        private void OnEnable()
        {
            resumeButton.onClick.AddListener(() => StartGame());
            menuGameButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu", LoadSceneMode.Single));
            exitGameButton.onClick.AddListener(() => Application.Quit());
        }

        private void StartGame()
        {
            WizardStateController.Instance.ChangeStage(StageWizard.FirstStage);
        }
    }
}
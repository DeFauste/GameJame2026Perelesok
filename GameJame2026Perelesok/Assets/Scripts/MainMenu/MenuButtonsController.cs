using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class MenuButtonsController : MonoBehaviour
    {
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button exitGameButton;

        private void OnEnable()
        {
            startGameButton.onClick.AddListener(() => SceneManager.LoadScene("MainScene", LoadSceneMode.Single));
            exitGameButton.onClick.AddListener(() => Application.Quit());
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class MenuPause : MonoBehaviour
    {
        [SerializeField] private Button menuGameButton;
        [SerializeField] private Button exitGameButton;
        
        
        private void OnEnable()
        {
            menuGameButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu", LoadSceneMode.Single));
            exitGameButton.onClick.AddListener(() => Application.Quit());
        }
    }
}
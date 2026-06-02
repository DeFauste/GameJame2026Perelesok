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
        [SerializeField] private string MusicName = "Music_MainMenu";
        private void OnEnable()
        {
            MusicService.Instance.Play(MusicName, true);
            startGameButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                MusicService.Instance.StopByName(MusicName);
            });
            exitGameButton.onClick.AddListener(() => Application.Quit());
        }
    }
}
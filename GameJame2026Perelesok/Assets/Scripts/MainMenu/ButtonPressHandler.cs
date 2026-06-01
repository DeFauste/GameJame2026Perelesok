using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu
{
    public class ButtonPressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        [SerializeField] Sprite offButton;
        [SerializeField] Sprite onButton;
        private Button button;
        private MusicService _musicService;
        public System.Action OnPressed;
        [SerializeField] public string SoundHover = "UI_hover";
        [SerializeField] public string SoundClick = "UI_buttonPressed";

        private void Start()
        {
            _musicService = MusicService.Instance;
            button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log($"Кнопка {gameObject.name} нажата!");
            _musicService.Play(SoundClick, false);
            if (button is not null && onButton is not null)
            {
                button.image.sprite = onButton;
            }            
            OnPressed?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log($"Кнопка {gameObject.name} Отпущена!");
            if (button is not null && onButton is not null)
            {
                button.image.sprite = offButton;
            }
            OnPressed?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _musicService.Play(SoundHover, false);
        }
    }
}
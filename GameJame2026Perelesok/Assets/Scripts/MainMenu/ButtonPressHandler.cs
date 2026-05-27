using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu
{
    public class ButtonPressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Sprite offButton;
        [SerializeField] Sprite onButton;
        private Button button;
        public System.Action OnPressed;

        private void Start()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log($"Кнопка {gameObject.name} нажата!");
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
    }
}
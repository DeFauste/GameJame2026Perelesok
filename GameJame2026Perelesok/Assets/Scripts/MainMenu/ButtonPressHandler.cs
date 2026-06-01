using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu
{
    public class ButtonPressHandler : MonoBehaviour, 
        IPointerDownHandler, 
        IPointerUpHandler, 
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [SerializeField] Sprite offButton;
        [SerializeField] Sprite onButton;
        private Button button;
        private MusicService _musicService;
        public System.Action OnPressed;
        [SerializeField] public string SoundHover = "UI_hover";
        [SerializeField] public string SoundClick = "UI_buttonPressed";

        
        [Header("Hover Scale")]
        [Tooltip("Во сколько раз увеличивается кнопка при наведении (например, 1.1 = +10%)")]
        [SerializeField] private float hoverScaleFactor = 1.1f;
        [Tooltip("Время анимации масштабирования в секундах")]
        [SerializeField] private float scaleDuration = 0.12f;

        private Coroutine _scaleCoroutine;
        private Vector3 _initialScale;
        private Transform _targetTransform;
        
        private void Awake()
        {
            // предпочитаем RectTransform для UI, но работаем и с обычным Transform
            _targetTransform = transform;
        }
        
        private void Start()
        {
            _musicService = MusicService.Instance;
            button = GetComponent<Button>();
            
            // Сохраняем начальный масштаб
            if (_targetTransform != null)
                _initialScale = _targetTransform.localScale;
            else
                _initialScale = Vector3.one;
        }

        private void OnDisable()
        {
            // при отключении вернуть исходный масштаб и остановить корутину
            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = null;
            }

            if (_targetTransform != null)
                _targetTransform.localScale = _initialScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _musicService?.Play(SoundClick, false);
            if (button is not null && onButton is not null)
            {
                button.image.sprite = onButton;
            }
            OnPressed?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (button is not null && onButton is not null)
            {
                button.image.sprite = offButton;
            }
            OnPressed?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Воспроизвести звук наведения
            _musicService?.Play(SoundHover, false);
            // Увеличить кнопку
            StartScaleTo(_initialScale * hoverScaleFactor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Вернуть к исходному размеру
            StartScaleTo(_initialScale);
        }

        private void StartScaleTo(Vector3 targetScale)
        {
            if (_targetTransform == null) return;

            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = null;
            }

            // если длительность почти нулевая, сделать сразу
            if (scaleDuration <= 0f)
            {
                _targetTransform.localScale = targetScale;
                return;
            }

            _scaleCoroutine = StartCoroutine(ScaleRoutine(_targetTransform, targetScale, scaleDuration));
        }

        private IEnumerator ScaleRoutine(Transform t, Vector3 target, float duration)
        {
            Vector3 start = t.localScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(elapsed / duration);
                // плавная интерполяция
                float s = Mathf.SmoothStep(0f, 1f, k);
                t.localScale = Vector3.Lerp(start, target, s);
                yield return null;
            }

            t.localScale = target;
            _scaleCoroutine = null;
        }
    }
}
using UnityEngine;
using Wizards.Animations;
using System.Collections;

namespace Wizards.Behaviours.Intro
{
    public class WizardWinStage : WizardStage
    {
        [SerializeField] protected float fadeDuration = 2f; // Длительность эффекта растворения в секундах
        [SerializeField] protected float blinkSpeed = 3f; // Скорость мигания (циклы в секунду)
        public GameObject _symbolSystem;

        private WizardAnimationService _wizardAnimationService;

        protected override void Start()
        {
            base.Start();
            _wizardAnimationService = WizardAnimationService.Instance;

        }

        public override void StartStage()
        {
            base.StartStage();
            stageActive = false;
            _wizardAnimationService?.Intro();
            Debug.Log("Запустили стадию Победы");
            playerController.enabled = false; // Отключаем управление игроком
            _symbolSystem.SetActive(false); // отключаем детект нажатий стрелочек
            // Запускаем эффект растворения спрайта
            StartCoroutine(FadeOutSpriteCoroutine());
        }

        /// <summary>
        /// Корутина для мигания и постепенного исчезновения спрайта
        /// </summary>
        private IEnumerator FadeOutSpriteCoroutine()
        {
            float elapsedTime = 0f;
            
            // Этап 1: Легкое мигание
            float blinkDuration = fadeDuration * 0.5f; // Первая половина времени на мигание
            float blinkElapsedTime = 0f;

            while (blinkElapsedTime < blinkDuration)
            {
                blinkElapsedTime += Time.deltaTime;
                float blinkPhase = (blinkElapsedTime / blinkDuration) * blinkSpeed * 2 * Mathf.PI;
                float alpha = Mathf.Cos(blinkPhase) * 0.5f + 0.5f; // Значение от 0.5 до 1.0
                
                SetSpriteAlpha(spriteDomeToCompress, alpha);
                SetSpriteAlpha(spriteCycleToCompress, alpha);
                
                yield return null;
            }

            // Этап 2: Плавное исчезновение (растворение)
            float fadeElapsedTime = 0f;
            float fadePhaseDuration = fadeDuration * 0.5f; // Вторая половина времени на исчезновение

            while (fadeElapsedTime < fadePhaseDuration)
            {
                fadeElapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, fadeElapsedTime / fadePhaseDuration);
                
                SetSpriteAlpha(spriteDomeToCompress, alpha);
                SetSpriteAlpha(spriteCycleToCompress, alpha);
                
                yield return null;
            }

            // Отключаем объект после завершения анимации
            gameObject.SetActive(false);
            Debug.Log("Объект волшебника отключен после растворения");
        }

        /// <summary>
        /// Вспомогательный метод для установки альфа-канала спрайта
        /// </summary>
        private void SetSpriteAlpha(SpriteRenderer spriteRenderer, float alpha)
        {
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Clamp01(alpha);
                spriteRenderer.color = color;
            }
        }
    }
}

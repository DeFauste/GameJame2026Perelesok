using System.Collections;
using UnityEngine;

namespace CameraEffects
{
    [DisallowMultipleComponent]
    public class CameraShake : MonoBehaviour
    {
        // Корутину можно остановить/перезапустить для нового шейка
        private Coroutine _shakeCoroutine;
        private Vector3 _originalLocalPosition;

        private void Awake()
        {
            // Запоминаем исходную локальную позицию, чтобы восстановить в конце
            _originalLocalPosition = transform.localPosition;
        }

        /// <summary>
        /// Запускает эффект дрожания камеры.
        /// duration - длительность в секундах, magnitude - максимальная амплитуда смещения.
        /// </summary>
        public void Shake(float duration = 0.5f, float magnitude = 0.3f)
        {
            if (duration <= 0f || magnitude <= 0f)
                return;

            // Если уже идёт шейк, остановим и начнём новый
            if (_shakeCoroutine != null)
                StopCoroutine(_shakeCoroutine);

            _shakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
        }

        private IEnumerator DoShake(float duration, float magnitude)
        {
            float elapsed = 0f;

            // Обновление оригинальной позиции на случай, если объект перемещался
            _originalLocalPosition = transform.localPosition;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                // Амплитуда убывает по мере прохождения времени
                float damper = 1f - Mathf.Clamp01(elapsed / duration);

                // Случайное смещение в пределах единичного круга для более плавной траектории
                Vector2 rand = Random.insideUnitCircle;
                Vector3 offset = new Vector3(rand.x, rand.y, 0f) * magnitude * damper;

                transform.localPosition = _originalLocalPosition + offset;

                yield return null;
            }

            // Восстанавливаем исходную позицию и очищаем ссылку
            transform.localPosition = _originalLocalPosition;
            _shakeCoroutine = null;
        }
    }
}
using System.Collections;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Класс для удаления объекта через указанное время
    /// </summary>
    public class DestroyTime : MonoBehaviour
    {
        [Header("Destroy Settings")]
        [SerializeField] private float _destroyDelay = 1f;
        [SerializeField] private bool _destroyOnStart = false;
        
        private Coroutine _destroyCoroutine;
        private bool _isScheduledForDestroy = false;

        private void Start()
        {
            if (_destroyOnStart)
            {
                ScheduleDestroy(_destroyDelay);
            }
        }

        /// <summary>
        /// Запланировать удаление объекта через время
        /// </summary>
        /// <param name="delay">Время до удаления в секундах</param>
        /// <param name="onBeforeDestroy">Коллбэк перед удалением</param>
        public void ScheduleDestroy(float delay, System.Action onBeforeDestroy = null)
        {
            if (delay < 0)
            {
                Debug.LogWarning("[DestroyTime] Время задержки не может быть отрицательным! Используется 0.", gameObject);
                delay = 0;
            }

            // Если уже запланировано удаление, отменяем старое
            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
            }

            _destroyCoroutine = StartCoroutine(DestroyCoroutine(delay, onBeforeDestroy));
            _isScheduledForDestroy = true;
        }

        /// <summary>
        /// Удалить объект немедленно
        /// </summary>
        /// <param name="onBeforeDestroy">Коллбэк перед удалением</param>
        public void DestroyImmediate(System.Action onBeforeDestroy = null)
        {
            onBeforeDestroy?.Invoke();
            Destroy(gameObject);
        }

        /// <summary>
        /// Отменить запланированное удаление
        /// </summary>
        public void CancelDestroy()
        {
            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
                _destroyCoroutine = null;
            }

            _isScheduledForDestroy = false;
            Debug.Log("[DestroyTime] Удаление объекта отменено.", gameObject);
        }

        /// <summary>
        /// Проверить, запланировано ли удаление
        /// </summary>
        public bool IsScheduledForDestroy => _isScheduledForDestroy;

        /// <summary>
        /// Изменить время удаления
        /// </summary>
        /// <param name="newDelay">Новое время удаления в секундах</param>
        public void SetDestroyDelay(float newDelay)
        {
            if (newDelay < 0)
            {
                Debug.LogWarning("[DestroyTime] Время задержки не может быть отрицательным!", gameObject);
                return;
            }

            _destroyDelay = newDelay;
            Debug.Log($"[DestroyTime] Время удаления изменено на {newDelay} сек.", gameObject);
        }

        /// <summary>
        /// Получить текущее время удаления
        /// </summary>
        public float GetDestroyDelay() => _destroyDelay;

        // ==================== COROUTINES ====================

        private IEnumerator DestroyCoroutine(float delay, System.Action onBeforeDestroy)
        {
            yield return new WaitForSeconds(delay);

            Debug.Log($"[DestroyTime] Объект '{gameObject.name}' удаляется через {delay} секунд.", gameObject);
            
            onBeforeDestroy?.Invoke();
            Destroy(gameObject);
            _isScheduledForDestroy = false;
        }

        private void OnDestroy()
        {
            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
            }
        }
    }
}
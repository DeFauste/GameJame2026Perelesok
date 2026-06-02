using System;
using Commons;
using UnityEngine;

namespace PeakSerg
{
    /// <summary>
    /// Сервис управления анимациями Peak.
    /// </summary>
    public class PeakController : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D colliderPeak; 
        [SerializeField] private float timeDelayAttack = 1f; // Время ожидания перед атакой. В этот момент появляется трещина
        [SerializeField] private float speedAttack = 1f; // Скорость роста шипа
        [SerializeField] private float speedEndAttack = 1f; // Скорость уменьшение шипа
        
        [SerializeField] private Animator animator;
        [SerializeField] private bool dontDestroyOnLoad = true;

        [Tooltip("Длительность перехода CrossFade (сек.)")]
        [SerializeField] private float crossFadeDuration = 0.12f;

        // Названия всех анимаций
        private const string EndAttackAnimation = "EndAttakPeak";
        private const string IdleAnimation = "IdlePeak";
        private const string AttackAnimation = "AttakPeak";
        private const string StopPeakAttackAnimation = "StopPeakAttak";

        // Хэши анимаций для быстрого доступа
        private int _stopAttackHash;
        private int _idleHash;
        private int _attackHash;
        private int _stopPeakAttackHash;

        // События для внешних подписчиков (опционально)
        public Action<string> OnAnimationPlayed;

        // Текущая проигрываемая анимация (для отслеживания)
        private string _currentAnimationName;

        protected void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            if (colliderPeak is null)
            {
                colliderPeak = GetComponent<BoxCollider2D>();
            }
            
            colliderPeak.enabled = false;
            
            if (animator == null)
                Debug.LogError("PeakAnimationService: Animator не найден!");
            else
                InitializeAnimationHashes();
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.R))
            {
                PlayAttack();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                PlayStopAttack();
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                PlayEndPeakAttack();
            }
        }

        /// <summary>
        /// Инициализирует хэши всех анимаций для CrossFade
        /// </summary>
        private void InitializeAnimationHashes()
        {
            _stopAttackHash = Animator.StringToHash(EndAttackAnimation);
            _idleHash = Animator.StringToHash(IdleAnimation);
            _attackHash = Animator.StringToHash(AttackAnimation);
            _stopPeakAttackHash = Animator.StringToHash(StopPeakAttackAnimation);
        }

        /// <summary>
        /// Проиграть анимацию по имени с CrossFade
        /// </summary>
        private void PlayAnimation(string animationName, int hash, float speedAnimation)
        {
            if (animator == null)
            {
                Debug.LogWarning("PeakAnimationService: Animator не инициализирован.");
                return;
            }
            
            animator.speed = speedAnimation;
            // CrossFade по хэшу
            animator.CrossFade(hash, crossFadeDuration);
            _currentAnimationName = animationName;
            OnAnimationPlayed?.Invoke(animationName);
        }

        /// <summary>
        /// Проиграть анимацию "StopAttakPeak"
        /// </summary>
        public void PlayStopAttack()
        {
            PlayAnimation(StopPeakAttackAnimation, _stopAttackHash, 1);
        }

        /// <summary>
        /// Проиграть анимацию "IdlePeak"
        /// </summary>
        public void PlayIdle()
        {
            PlayAnimation(IdleAnimation, _idleHash, 1);
        }

        /// <summary>
        /// Проиграть анимацию "AttakPeak"
        /// </summary>
        public void PlayAttack()
        {
            PlayAnimation(AttackAnimation, _attackHash, speedAttack);
        }

        /// <summary>
        /// Проиграть анимацию "EndPeakAttak"
        /// </summary>
        public void PlayEndPeakAttack()
        {
            PlayAnimation(EndAttackAnimation, _stopPeakAttackHash, speedEndAttack);
        }

        /// <summary>
        /// Получить текущую проигрываемую анимацию
        /// </summary>
        public string GetCurrentAnimationName() => _currentAnimationName;

        /// <summary>
        /// Изменить длительность CrossFade
        /// </summary>
        public void SetCrossFadeDuration(float duration)
        {
            crossFadeDuration = Mathf.Max(0f, duration);
        }

        // Включить колап
        public void ActiveCollider()
        {
            colliderPeak.enabled = true;
        }

        // Выключить колайдер
        public void DisableCollider()
        {
            colliderPeak.enabled = false;
        }
    }
}




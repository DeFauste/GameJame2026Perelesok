using System;
using System.Collections;
using System.Collections.Generic;
using CameraEffects;
using Commons;
using UnityEngine;

namespace Wizards.Animations
{
    /// <summary>
    /// Сервис управления анимациями волшебника.
    /// - Использует CrossFade для переходов.
    /// - Подписывается на изменения стадии и состояния (события контроллера) и реагирует — не изменяя внешние значения.
    /// - PlayBlink/PlayMouth/PlayIdle не бросают события OnStateChanged/OnStageChanged (только OnAnimationPlayed).
    /// - ChangeAnimationState явно меняет internal state и вызывает OnStateChanged.
    /// </summary>
    public class WizardAnimationService : SingletonMonoBehaviour<WizardAnimationService>
    {
        [SerializeField] private CameraShake _cameraShake;
        [SerializeField] private GameObject _hands;
        [SerializeField] private GameObject _fists;
        [SerializeField] private float timeWaitFists = 1f;
        [SerializeField] private Animator animator;
        [SerializeField] private bool dontDestroyOnLoad = true;

        private MusicService  musicService;
        
        [Tooltip("Длительность перехода CrossFade (сек.)")]
        [SerializeField] private float crossFadeDuration = 0.12f;
        
        // Перечисление состояний анимации
        public enum AnimationState
        {
            Intro,
            Idle,
            Blink,
            Mouth,
            Defeat
        }

        // Перечисление стадий
        public enum StageWizardNumer
        {
            None,
            Intro,
            First,
            Second,
            Third,
            Win,
            Lose,
        }

        // Словарь: (Stage, State) -> animation name
        private Dictionary<(StageWizardNumer, AnimationState), string> animationMap;

        // Хэши анимаций для CrossFade
        private Dictionary<(StageWizardNumer, AnimationState), int> animationHashMap;

        // Внутренные значения (локальные, чтение/реакция)
        private StageWizardNumer currentStage = StageWizardNumer.First;
        private AnimationState currentState = AnimationState.Idle;

        // События для внешних подписчиков
        public Action<StageWizardNumer> OnStageChanged { get; set; }
        public Action<AnimationState> OnStateChanged { get; set; }
        public Action<string> OnAnimationPlayed { get; set; }

        protected override void Awake()
        {
            base.Awake();

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            InitializeAnimationMap();

            if (animator == null)
                animator = GetComponent<Animator>();

            if (animator == null)
                Debug.LogError("WizardAnimationService: Animator не найден!");
            else
                InitializeHashMap();
        }


        private void Start()
        {
            musicService = MusicService.Instance;
        }

        private void OnEnable()
        {
            // Подписываемся на изменения стадии/состояния контроллера (реагируем, но не меняем контроллер)
            if (WizardStateController.Instance != null)
            {
                WizardStateController.Instance.ActionStage += HandleExternalStageChanged;
                WizardStateController.Instance.ActionCompressedDirection += HandleCompressedDirectionChanged;
            }
        }

        private void OnDisable()
        {
            if (WizardStateController.Instance != null)
            {
                WizardStateController.Instance.ActionStage -= HandleExternalStageChanged;
                WizardStateController.Instance.ActionCompressedDirection -= HandleCompressedDirectionChanged;
            }
        }

        private void InitializeAnimationMap()
        {
            animationMap = new Dictionary<(StageWizardNumer, AnimationState), string>
            {
                // Вступление
                { (StageWizardNumer.Intro, AnimationState.Intro), "Intro" },
                // Победа рыцаря
                { (StageWizardNumer.Lose, AnimationState.Defeat), "WizardDeath" },
                
                // Стадия 1
                { (StageWizardNumer.First, AnimationState.Idle), "1_IdleWizardStageFirst" },
                { (StageWizardNumer.First, AnimationState.Blink), "1_BlinkWizardStageFirst" },
                { (StageWizardNumer.First, AnimationState.Mouth), "1_MouthWizardStageFirst" },

                // Стадия 2
                { (StageWizardNumer.Second, AnimationState.Idle), "2_IdleWizardStageSecond" },
                { (StageWizardNumer.Second, AnimationState.Blink), "2_BlinkWizardStageSecond" },
                { (StageWizardNumer.Second, AnimationState.Mouth), "2_MouthWizardStageSecond" },

                // Стадия 3
                { (StageWizardNumer.Third, AnimationState.Idle), "3_IdleWizardThreeStage" },
                { (StageWizardNumer.Third, AnimationState.Blink), "3_BlinkWizardThreeStage" },
                { (StageWizardNumer.Third, AnimationState.Mouth), "3_MouthWizardThreeStage" }
            };
        }

        private void InitializeHashMap()
        {
            animationHashMap = new Dictionary<(StageWizardNumer, AnimationState), int>(animationMap.Count);
            foreach (var kv in animationMap)
            {
                animationHashMap[kv.Key] = Animator.StringToHash(kv.Value);
            }
        }

        // Внешний обработчик изменений стадии из контроллера.
        // РЕАКЦИЯ: обновить internal currentStage и тихо перейти в Idle (без вызова OnStageChanged/OnStateChanged)
        private void HandleExternalStageChanged(StageWizard stage)
        {
            var newStage = stage switch
            {
                StageWizard.None => StageWizardNumer.None,
                StageWizard.Intro => StageWizardNumer.Intro,
                StageWizard.FirstStage => StageWizardNumer.First,
                StageWizard.SecondStage => StageWizardNumer.Second,
                StageWizard.ThirdStage => StageWizardNumer.Third,
                StageWizard.Win => StageWizardNumer.Win,
                StageWizard.Lose => StageWizardNumer.Lose,
                _ => currentStage
            };

            if (newStage == currentStage) return;

            currentStage = newStage;

            // Переходим тихо в Idle (не дергаем OnStateChanged/OnStageChanged обратно в контроллер)
            PlayAnimationSilentByStageState(currentStage, AnimationState.Idle);

            // Уведомляем локальных подписчиков, что у сервиса изменилась стадия (опционально)
            // ВАЖНО: это событие относится к сервису — оно НЕ меняет Stage в WizardStateController.
            OnStageChanged?.Invoke(currentStage);
        }

        // Пример реакции на изменение "state" контроллера — здесь используем CompressedDirection как "state".
        // Вызывается при изменении сжатия/расширения. Реагируем визуально при необходимости, не изменяя state в контроллере.
        private void HandleCompressedDirectionChanged(CompressedDirection dir)
        {
            // Пример реакции: при Compress - проиграть небольшую анимацию Mouth, при Expansion - вернуться в Idle.
            // Эти вызовы НЕ должны генерировать OnStateChanged/OnStageChanged.
            if (dir == CompressedDirection.Compress)
            {
                PlayMouth(); // тихая проигравка — не генерирует OnStateChanged
            }
            else if (dir == CompressedDirection.Expansion)
            {
                PlayIdle();
            }
        }

        /// <summary>
        /// Явно изменить состояние анимации и оповестить слушателей (генерирует OnStateChanged)
        /// </summary>
        public void ChangeAnimationState(AnimationState state)
        {
            if (currentState == state) return;

            currentState = state;
            PlayAnimationByCurrent();
            OnStateChanged?.Invoke(currentState);
        }

        private void PlayAnimationByCurrent()
        {
            PlayAnimationSilentByStageState(StageWizardNumer.Intro, AnimationState.Intro);
        }

        /// <summary>
        /// Проиграть animation для указанной стадии и состояния, НЕ генерируя события OnStateChanged/OnStageChanged.
        /// Использует CrossFade.
        /// </summary>
        private void PlayAnimationSilentByStageState(StageWizardNumer stage, AnimationState state)
        {
            if (animator == null)
            {
                Debug.LogWarning("WizardAnimationService: Animator не инициализирован.");
                return;
            }

            if (!animationHashMap.TryGetValue((stage, state), out var hash))
            {
                Debug.LogWarning($"WizardAnimationService: Анимация не найдена для ({stage},{state})");
                return;
            }

            // CrossFade по хэшу
            animator.CrossFade(hash, crossFadeDuration);

            // Вызываем только OnAnimationPlayed (имя для логов)
            if (animationMap.TryGetValue((stage, state), out var animName))
                OnAnimationPlayed?.Invoke(animName);
        }

        // Публичные простые вызовы: НЕ генерируют OnStateChanged/OnStageChanged, только проигрывают анимацию.
        public void PlayBlink()
        {
            PlayAnimationSilentByStageState(currentStage, AnimationState.Blink);
        }

        public void PlayMouth()
        {
            PlayAnimationSilentByStageState(currentStage, AnimationState.Mouth);
        }

        public void PlayIdle()
        {
            PlayAnimationSilentByStageState(currentStage, AnimationState.Idle);
        }
        
        public void Intro()
        {
            PlayAnimationSilentByStageState(currentStage, AnimationState.Intro);
            musicService.Play("Sound_EnemyLaugh_Intro");
        }
        
        public void LoseStage()
        {
            PlayAnimationSilentByStageState(currentStage, AnimationState.Defeat);
        }

        // Получатели состояния
        public StageWizardNumer GetCurrentStage() => currentStage;
        public AnimationState GetCurrentState() => currentState;

        public string GetAnimationName(StageWizardNumer stage, AnimationState state)
        {
            return animationMap.TryGetValue((stage, state), out var name) ? name : null;
        }

        public void ActiveCameraShake()
        {
            _cameraShake?.Shake();
            StartCoroutine(WaitTime(timeWaitFists));
        }

        IEnumerator WaitTime(float timeWait)
        {
            MusicService.Instance.Play("Sound_IntroFists");
            yield return new WaitForSeconds(timeWait);
            _fists.SetActive(false);
            _hands.SetActive(true);
            PlayMouth();
            yield return new WaitForSeconds(timeWait);
            WizardStateController.Instance.ChangeStage(StageWizard.FirstStage);
        }
    }
}

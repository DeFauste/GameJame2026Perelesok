using System;
using System.Collections;
using System.Collections.Generic;
using CameraEffects;
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

        private MusicService musicService;

        [Tooltip("Длительность перехода CrossFade (сек.)")] [SerializeField]
        private float crossFadeDuration = 0.12f;

        // Перечисление состояний анимации
        public enum AnimationState
        {
            Intro,
            Idle,
            Blink,
            Mouth,
            Death
        }

        // Перечисление стадий
        public enum StageNumber
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
        private Dictionary<(StageNumber, AnimationState), string> animationMap;

        // Хэши анимаций для CrossFade
        private Dictionary<(StageNumber, AnimationState), int> animationHashMap;

        // Внутренные значения (локальные, чтение/реакция)
        private StageNumber currentStage = StageNumber.None;
        private AnimationState currentState = AnimationState.Idle;

        public Action<string> OnAnimationPlayed { get; set; }

        protected override void Awake()
        {
            InitializeAnimationMap();


            base.Awake();

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);


            if (animator == null)
                animator = GetComponent<Animator>();

            if (animator == null)
                Debug.LogError("WizardAnimationService: Animator не найден!");
            else
                InitializeHashMap();
        }


        private void Start()
        {
            WizardStateController.Instance.ActionStage += HandleExternalStageChanged;
            WizardStateController.Instance.ActionCompressedDirection += HandleCompressedDirectionChanged;
            musicService = MusicService.Instance;
        }

        private void OnDisable()
        {
            WizardStateController.Instance.ActionStage -= HandleExternalStageChanged;
            WizardStateController.Instance.ActionCompressedDirection -= HandleCompressedDirectionChanged;
        }

        private void InitializeAnimationMap()
        {
            animationMap = new Dictionary<(StageNumber, AnimationState), string>
            {
                { (StageNumber.Lose, AnimationState.Death), "WizardDeath" },
                { (StageNumber.First, AnimationState.Death), "WinStage" },
                { (StageNumber.Intro, AnimationState.Intro), "Intro" },
                // Стадия 1
                { (StageNumber.First, AnimationState.Idle), "1_IdleWizardStageFirst" },
                { (StageNumber.First, AnimationState.Blink), "1_BlinkWizardStageFirst" },
                { (StageNumber.First, AnimationState.Mouth), "1_MouthWizardStageFirst" },

                // Стадия 2
                { (StageNumber.Second, AnimationState.Idle), "2_IdleWizardStageSecond" },
                { (StageNumber.Second, AnimationState.Blink), "2_BlinkWizardStageSecond" },
                { (StageNumber.Second, AnimationState.Mouth), "2_MouthWizardStageSecond" },

                // Стадия 3
                { (StageNumber.Third, AnimationState.Idle), "3_IdleWizardThreeStage" },
                { (StageNumber.Third, AnimationState.Blink), "3_BlinkWizardThreeStage" },
                { (StageNumber.Third, AnimationState.Mouth), "3_MouthWizardThreeStage" }
            };
        }

        private void InitializeHashMap()
        {
            animationHashMap = new Dictionary<(StageNumber, AnimationState), int>(animationMap.Count);
            foreach (var kv in animationMap)
            {
                animationHashMap[kv.Key] = Animator.StringToHash(kv.Value);
            }
        }

        // Внешний обработчик изменений стадии из контроллера.
        // РЕАКЦИЯ: обновить internal currentStage и тихо перейти в Idle (без вызова OnStageChanged/OnStateChanged)
        private void HandleExternalStageChanged(StageWizard stage)
        {
            var newStage = MapStage(stage);

            if (newStage == currentStage) return;

            currentStage = newStage;
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

        private void PlayAnimationByCurrent()
        {
            PlayAnimationSilentByStageState(StageNumber.Intro, AnimationState.Intro);
        }

        /// <summary>
        /// Проиграть animation для указанной стадии и состояния, НЕ генерируя события OnStateChanged/OnStageChanged.
        /// Использует CrossFade.
        /// </summary>
        private void PlayAnimationSilentByStageState(StageNumber stage, AnimationState state)
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
            var currentStageLocal = MapStage(WizardStateController.Instance.CurrentStage);
            if (currentStageLocal == StageNumber.First)
            {
                animator.CrossFade("1_BlinkWizardStageFirst", crossFadeDuration);
            }
            else if (currentStageLocal == StageNumber.Second)
            {
                animator.CrossFade("2_BlinkWizardStageSecond", crossFadeDuration);
            }
            else if (currentStageLocal == StageNumber.Third)
            {
                animator.CrossFade("3_BlinkWizardThreeStage", crossFadeDuration);
            }        }

        public void PlayMouth()
        {
            var currentStageLocal = MapStage(WizardStateController.Instance.CurrentStage);
            if (currentStageLocal == StageNumber.First)
            {
                animator.CrossFade("1_MouthWizardStageFirst", crossFadeDuration);
            }
            else if (currentStageLocal == StageNumber.Second)
            {
                animator.CrossFade("2_MouthWizardStageSecond", crossFadeDuration);
            }
            else if (currentStageLocal == StageNumber.Third)
            {
                animator.CrossFade("3_MouthWizardThreeStage", crossFadeDuration);
            }
        }

        public void PlayIdle()
        {
            var currentStageLocal = MapStage(WizardStateController.Instance.CurrentStage);
            if (currentStageLocal == StageNumber.First)
            {
                animator.CrossFade("1_IdleWizardStageFirst", crossFadeDuration);
            }
            else if (currentStageLocal == StageNumber.Second)
            {
                animator.CrossFade("2_IdleWizardStageSecond", crossFadeDuration);
            }
            else if (currentStageLocal == StageNumber.Third)
            {
                animator.CrossFade("3_IdleWizardThreeStage", crossFadeDuration);
            }
            // PlayAnimationSilentByStageState(currentStage, AnimationState.Idle);
        }

        public void Intro()
        {
            var currentStageLocal = MapStage(WizardStateController.Instance.CurrentStage);
            animator.CrossFade("Intro", crossFadeDuration);
            MusicService.Instance.Play("Sound_EnemyLaugh_Intro");
        }

        public void LoseStage()
        {
            animator.CrossFade("WizardDeath", crossFadeDuration);
        }


        // Получатели состояния
        public StageNumber GetCurrentStage() => currentStage;
        public AnimationState GetCurrentState() => currentState;

        public string GetAnimationName(StageNumber stage, AnimationState state)
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

        private StageNumber MapStage(StageWizard stage) => stage switch
        {
            StageWizard.None => StageNumber.None,
            StageWizard.Intro => StageNumber.Intro,
            StageWizard.FirstStage => StageNumber.First,
            StageWizard.SecondStage => StageNumber.Second,
            StageWizard.ThirdStage => StageNumber.Third,
            StageWizard.Win => StageNumber.Win,
            StageWizard.Lose => StageNumber.Lose,
            _ => currentStage
        };
    }
}
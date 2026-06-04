using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Wizards.Animations;

namespace Wizards.Behaviours
{
    public sealed class WizardFirstStage : WizardStage
    {
        [SerializeField] private SpikeController _spikeController;
        private WizardAnimationService _wizardAnimationService;
        private Coroutine _randomAnimCoroutine;
        private Coroutine _spikeSpawnCoroutine;
        [SerializeField] private float timeChangeAnimation = 5f; // время через которое будет меняться анимация

        [SerializeField] private float _spikeTimeSpawn = 3f; // частота спавна
        [SerializeField] private int _spikeCountFowSpawn = 1; // количество за один тик
        [SerializeField] private float _spikeDuration = 3f;
        private void Awake()
        {
            _wizardAnimationService = WizardAnimationService.Instance;
        }

        protected override void Start()
        {
            base.Start();
            _spikeController = FindObjectOfType<SpikeController>();
        }

        private void FixedUpdate()
        {
            if (stageActive)
            {
                // Запуск логики первой стадии

                // Запускаем корутину случайных анимаций, если ещё не запущена
                if (_randomAnimCoroutine == null)
                {
                    _randomAnimCoroutine = StartCoroutine(RandomAnimationRoutine());
                }
                
                // Запускаем спавн шипов по таймеру
                if (_spikeSpawnCoroutine == null)
                {
                    _spikeSpawnCoroutine = StartCoroutine(SpanSmallSpike());
                }
            }
            else
            {
                // Останавливаем корутину, если стадия неактивна
                StopRandomAnimations();
            }
        }

        IEnumerator SpanSmallSpike()
        {
            while (stageActive)
            {
                yield return new WaitForSeconds(timeChangeAnimation);
                _spikeController.RandomSpawnSmallPeaks(_spikeCountFowSpawn, _spikeDuration);
                MusicService.Instance.Play("Sound_EarthFracture");
                yield return new WaitForSeconds(_spikeDuration);
                MusicService.Instance.Play("Sound_SpikeSmall");
            }
        }
        

        private IEnumerator RandomAnimationRoutine()
        {
            Debug.Log("Запустили рандомную анимацию");
            var wait = new WaitForSeconds(timeChangeAnimation);

            while (stageActive)
            {
                yield return wait;

                if (!stageActive) break;
                if (WizardStateController.Instance.CurrentStage != StageWizard.Intro &&
                    WizardStateController.Instance.CurrentStage != StageWizard.None)
                {
                    // Выбираем случайную анимацию: Idle / Blink / Mouth
                    int choice = UnityEngine.Random.Range(0, 3);
                
                    switch (choice)
                    {
                        case 0:
                            _wizardAnimationService?.PlayIdle();
                            break;
                        case 1:
                            _wizardAnimationService?.PlayBlink();
                            break;
                        case 2:
                            _wizardAnimationService?.PlayMouth();
                            break;
                    }
                }
            }

            _randomAnimCoroutine = null;
        }

        private void StopRandomAnimations()
        {
            if (_randomAnimCoroutine != null)
            {
                StopCoroutine(_randomAnimCoroutine);
                _randomAnimCoroutine = null;
            }
        }

        private void OnDisable()
        {
            StopRandomAnimations();
        }
    }
}
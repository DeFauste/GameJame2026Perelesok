using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Wizards.Animations;

namespace Wizards.Behaviours
{
    public sealed class WizardFirstStage : WizardStage
    {
        [SerializeField] private string NumberStage = "0";
        [SerializeField] private SpikeController _spikeController;
        private WizardAnimationService _wizardAnimationService;
        private Coroutine _randomAnimCoroutine;
        [SerializeField] private float timeChangeAnimation = 5f; // время через которое будет меняться анимация

        [SerializeField] private float _spikeTimeSpawn = 1f; // частота спавна

        public bool isSmallSpawning = false; // флаг, разрешающий спавн шипов
        [SerializeField] private int _spikeSmallCountFowSpawn = 3; // количество за один тик
        [SerializeField] private float _spikeSmallDuration = 3f;

        public bool isBigSpawning = false; // флаг, разрешающий спавн шипов
        [SerializeField] private int _spikeBiggCountFowSpawn = 3; // количество за один тик
        [SerializeField] private float _spikeBiggDuration = 3f;

        public bool isLaserSpawning = false; // флаг, разрешающий спавн лазера
        [SerializeField] private int _lazerBiggCountFowSpawn = 2; // количество за один тик
        [SerializeField] private float _lazerBiggDuration = 5f;
        [SerializeField] private Transform _lazerSpawnPont;

        private void Awake()
        {
            _wizardAnimationService = WizardAnimationService.Instance;
        }

        protected override void Start()
        {
            base.Start();
            _spikeController = FindObjectOfType<SpikeController>();
        }

        public override void StartStage()
        {
            base.StartStage();
            // Запускаем спавн шипов по таймеру
            if (isSmallSpawning)
            {
                coroutines.Add(StartCoroutine(SpanSmallSpike()));
            }

            if (isBigSpawning)
            {
                coroutines.Add(StartCoroutine(SpanBigSpike()));
            }

            if (isLaserSpawning)
            {
                coroutines.Add(StartCoroutine(SpanLaser()));
            }
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
                _spikeController.RandomSpawnSmallPeaks(_spikeSmallCountFowSpawn, _spikeSmallDuration);
                MusicService.Instance.Play("Sound_EarthFracture");
                yield return new WaitForSeconds(_spikeSmallDuration);
                MusicService.Instance.Play("Sound_SpikeSmall");
            }
        }

        IEnumerator SpanBigSpike()
        {
            while (stageActive)
            {
                yield return new WaitForSeconds(timeChangeAnimation);
                _spikeController.RandomSpawnSmallPeaks(_spikeSmallCountFowSpawn, _spikeSmallDuration);
                yield return new WaitForSeconds(_spikeSmallDuration);
                MusicService.Instance.Play("Sound_SpikeBig");
            }
        }

        IEnumerator SpanLaser()
        {
            while (stageActive)
            {
                yield return new WaitForSeconds(timeChangeAnimation);
                _spikeController.RandomSpawnLaser(_spikeSmallCountFowSpawn, _spikeSmallDuration,
                    _lazerSpawnPont.position);
                yield return new WaitForSeconds(_spikeSmallDuration);
                MusicService.Instance.Play("Sound_LaserCharge");
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
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Wizards.Animations;

namespace Wizards.Behaviours
{
    public sealed class WizardFirstStage : WizardStage
    {
        private WizardAnimationService _wizardAnimationService;
        private Coroutine _randomAnimCoroutine;
        private Coroutine _spikeSpawnCoroutine;
        [SerializeField] private float timeChangeAnimation = 5f; // время через которое будет меняться анимация

        private void Awake()
        {
            _wizardAnimationService = WizardAnimationService.Instance;
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
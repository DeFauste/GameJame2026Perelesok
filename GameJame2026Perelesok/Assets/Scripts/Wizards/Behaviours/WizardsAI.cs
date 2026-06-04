using System;
using UnityEngine;
using Wizards.Behaviours.Intro;
using Wizards.Behaviours.Lose;

namespace Wizards.Behaviours
{
    public sealed class WizardsAI : SingletonMonoBehaviour<WizardsAI>
    {
        public WizardStateController _wizardStateController;
        public WizardIntroStage _introStageController;
        public WizardFirstStage firstStageController;
        public LoseStage _loseStageController;
        public WizardWinStage _winStageController;
        private void Awake()
        {
            _wizardStateController = WizardStateController.Instance;
            Debug.Log($"Подписался на ActionStage в WizardsAI");
        }

        private void Start()
        {
            WizardStateController.Instance.ActionStage += OnStageChanged;
        }

        /// <summary>
        /// Метод для проверки состояния здоровья волшебника и выполнения действий в зависимости от текущего уровня здоровья.
        /// </summary>
        private void OnStageChanged(StageWizard stage)
        {
            if (stage == StageWizard.Intro)
            {
                _introStageController?.StartStage();
            }
            else if (stage == StageWizard.FirstStage)
            {
                firstStageController.StartStage();
                _introStageController.EndStage();
            }
            else if (stage == StageWizard.SecondStage)
            {
                // Действия для второй стадии
            }
            else if (stage == StageWizard.ThirdStage)
            {
                // Действия для третьей стадии
            }
            else if (stage == StageWizard.Win)
            {
                firstStageController.EndStage();
                // Действия для победы волшебника
                _winStageController.StartStage();
            }
            else if (stage == StageWizard.Lose)
            {
                // Действия для смерти волшебника
                _loseStageController.StartStage();
            }
        }

        private void OnDestroy()
        {
            if (_wizardStateController != null)
                _wizardStateController.ActionStage -= OnStageChanged;
        }

        public void StopAllStage()
        {
            _wizardStateController.ChangeStage(StageWizard.None);
            _wizardStateController.ChangeCompressedDirection(CompressedDirection.Static);
            _introStageController?.EndStage();
            firstStageController?.EndStage();
            MusicService.Instance.StopAll();
             // Остановить другие стадии, если необходимо
        }
    }
}
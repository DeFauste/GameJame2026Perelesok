using System;
using Commons;
using UnityEngine;
using Wizards.Behaviours.Intro;

namespace Wizards.Behaviours
{
    public sealed class WizardsAI : SingletonMonoBehaviour<WizardsAI>
    {
        private WizardStateController _wizardStateController;
        public WizardIntroStage _introStageController;
        public WizardFirstStage firstStageController;

        private void Awake()
        {
            _wizardStateController = WizardStateController.Instance;
            _wizardStateController.ActionStage += OnStageChanged;
            Debug.Log($"Подписался на ActionStage в WizardsAI");
        }


        /// <summary>
        /// Метод для проверки состояния здоровья волшебника и выполнения действий в зависимости от текущего уровня здоровья.
        /// </summary>
        private void OnStageChanged(StageWizard stage)
        {
            Debug.Log($"Стадия волшебника изменилась на: {stage} в WizardsAI");
            if (stage == StageWizard.Intro)
            {
                _introStageController.StartStage();
            }
            else if (stage == StageWizard.FirstStage)
            {
                firstStageController.StartStage();
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
                // Действия для победы
            }
            else if (stage == StageWizard.Lose)
            {
                // Действия для смерти
            }
        }

        private void OnDestroy()
        {
            if (_wizardStateController != null)
                _wizardStateController.ActionStage -= OnStageChanged;
        }
    }
}
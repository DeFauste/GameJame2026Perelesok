using UnityEngine;
using Wizards.Animations;

namespace Wizards.Behaviours.Intro
{
    public class WizardWinStage : WizardStage
    {
        private WizardAnimationService _wizardAnimationService;

        protected override void Start()
        {
            base.Start();
            _wizardAnimationService = WizardAnimationService.Instance;
            
            _wizardAnimationService?.Intro();
            Debug.Log("Запустили стадию Победы");
            playerController.enabled = false; // Отключаем управление игроком
            
            // Проиграть анимацию смерти игрока
        }
    }
}
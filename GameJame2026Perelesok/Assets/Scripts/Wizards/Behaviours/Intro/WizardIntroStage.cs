using UnityEngine;
using Wizards.Animations;

namespace Wizards.Behaviours.Intro
{
    public class WizardIntroStage : WizardStage
    {
        private WizardAnimationService _wizardAnimationService;

        protected override void Start()
        {
            base.Start();
            _wizardAnimationService = WizardAnimationService.Instance;
            
            _wizardAnimationService?.Intro();
            Debug.Log("Запустили стадию ИНТРО");
        }
    }
}
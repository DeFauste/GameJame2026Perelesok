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
        }
        public override void StartStage()
        {
            _wizardAnimationService.Intro();
            
            base.StartStage();
            stageActive = false; // Стадия интро не требует активного обновления, так как анимация управляет всем процессом
        }
    }
}
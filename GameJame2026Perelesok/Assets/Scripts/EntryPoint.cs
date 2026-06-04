using System;
using Wizards;

namespace DefaultNamespace
{
    public sealed class EntryPoint : SingletonMonoBehaviour<EntryPoint>
    {
        WizardStateController  wizardStateController;

        private void Start()
        {
            wizardStateController = WizardStateController.Instance;
        }

        public void StartGame()
        {
            // wizardStateController.ChangeStage(StageWizard.FirstStage);
        }
    }
}
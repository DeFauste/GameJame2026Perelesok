using System;
using Wizards;

namespace DefaultNamespace
{
    public sealed class EntryPoint : SingletonMonoBehaviour<EntryPoint>
    {

        private void Start()
        {
        }

        public void StartGame()
        {
            // wizardStateController.ChangeStage(StageWizard.FirstStage);
        }
    }
}
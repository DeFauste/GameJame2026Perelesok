using UnityEngine;

namespace Wizards.Behaviours
{
    public sealed class WizardsAI : MonoBehaviour
    {
        private WizardStateController _wizardStateController;
        [SerializeField]
        private WizardStage firstStageController;
        
        private void Start()
        {
            _wizardStateController = WizardStateController.Instance;
            _wizardStateController.ActionStage += OnHealthChanged;
        }

        /// <summary>
        /// Метод для проверки состояния здоровья волшебника и выполнения действий в зависимости от текущего уровня здоровья.
        /// </summary>
        private void OnHealthChanged(StageWizard stage)
        {
            if (stage == StageWizard.FirstStage) 
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
            _wizardStateController.ActionStage -= OnHealthChanged;
        }
    }
}
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
            _wizardStateController.ChangeHealth += OnHealthChanged;
        }

        /// <summary>
        /// Метод для проверки состояния здоровья волшебника и выполнения действий в зависимости от текущего уровня здоровья.
        /// </summary>
        private void OnHealthChanged(int health)
        {
            if (health == 3) // Запускаем первую стадию
            {
                // Действия для первой стадии (здоровье 3)
            }
            else if (health == 2) // Запускаем вторую стадию
            {
                // Действия для второй стадии (здоровье 2)
            }
            else if (health == 1) // Запускаем третью стадию
            {
                // Действия для третьей стадии (здоровье 1)
            }
            else if (health <= 0)
            {
                // Действия для смерти (здоровье 2)
            }
        }

        private void OnDestroy()
        {
            _wizardStateController.ChangeHealth -= OnHealthChanged;
        }
    }
}
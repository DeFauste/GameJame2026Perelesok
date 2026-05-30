using System;
using Commons;
using UnityEngine;

namespace Wizards
{
    public sealed class WizardStateController : SingletonMonoBehaviour<WizardStateController>
    {
        public int CurrentHealth { get; private set; } = 3; // Текущее значение здоровья волшебника
        
        public StageWizard CurrentStage { get; private set; }  = StageWizard.FirstStage; // Текущая стадия поведения волшебника

        // Методы для взаимодействия со стадиями волшебника
        #region Stage
        /// <summary>
        /// Делегат оповещения изменения стадии волшебника
        /// </summary>
        public Action<StageWizard> ActionState { get; set; } 
        
        /// <summary>
        /// Метод для изменения текущей стадии поведения волшебника
        /// </summary>
        public void ChangeStage(StageWizard stage)
        {
            if (CurrentStage != stage)
            {
                ActionState?.Invoke(stage);
                CurrentStage = stage;
            }
        }
        #endregion Stage
        
        // Методы для взаимодействия со здоровьем волшебника
        #region Health
        /// <summary>
        /// Метод для изменения текущего здоровья волшебника
        /// </summary>
        public event Action<int> ChangeHealth;
        
        /// <summary>
        /// Записать получения урона волшебнику
        /// </summary>
        public int TakeDamage(int damage)
        {
            // Убедиться, что урон положительный, от греха подальше
            damage = Mathf.Abs(damage); 
            CurrentHealth -= damage;
            
            // уведомляем, всех подписчиков, что здоровье изменилось
            ChangeHealth?.Invoke(CurrentHealth);
            
            return CurrentHealth;
        }
        #endregion Health
    }
}
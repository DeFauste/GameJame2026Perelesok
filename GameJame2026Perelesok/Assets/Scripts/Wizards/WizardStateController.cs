using System;
using Commons;
using UnityEngine;
using Wizards.Behaviours;

namespace Wizards
{
    public sealed class WizardStateController : SingletonMonoBehaviour<WizardStateController>
    {
        public int CurrentHealth { get; private set; } = 1; // Текущее значение здоровья волшебника

        private CompressedDirection
            _currentDirectionCompress = CompressedDirection.Static; // Текущее направление сжатия/расширения

        public CompressedDirection CurrentDirectionCompress => _currentDirectionCompress;

        private StageWizard _currentStage = StageWizard.None; // Текущая стадия поведения волшебника
        public StageWizard CurrentStage => _currentStage; // Текущая стадия поведения волшебника

        private void Start()
        {
            ChangeStage(StageWizard.Intro);
            SymbolSystem.OnAttackSuccess += () =>
            {
                Debug.Log("Нанесли дамаг волшебнику!");
                TakeDamage(1);
            };
        }

        // Методы для взаимодействия со стадиями волшебника

        #region Stage Direction

        /// <summary>
        /// Делегат оповещения изменения стадии волшебника
        /// </summary>
        public Action<StageWizard> ActionStage { get; set; }

        /// <summary>
        /// Метод для изменения текущей стадии поведения волшебника
        /// </summary>
        public void ChangeStage(StageWizard stage)
        {
            if (CurrentStage != stage)
            {

                ActionStage?.Invoke(stage);
                _currentStage = stage;
                Debug.Log($"Текущая стадия волшебника: {CurrentStage}");
            }
        }

        /// <summary>
        /// Делегат оповещения изменения направления сжатия/расширения волшебника
        /// </summary>
        public Action<CompressedDirection> ActionCompressedDirection { get; set; }

        /// <summary>
        /// Метод для изменения текущего направления сжатия/расширения волшебника
        /// </summary>
        public void ChangeCompressedDirection(CompressedDirection direction)
        {
            if (_currentDirectionCompress != direction)
            {
                ActionCompressedDirection?.Invoke(direction);
                _currentDirectionCompress = direction;
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
            Debug.Log($"Волшебник получил {damage} урона. Текущее здоровье: {CurrentHealth}");
            if(CurrentHealth <= 0)
            {
                Debug.Log("Волшебник погиб!");
                ChangeStage(StageWizard.Lose);
            }
            return CurrentHealth;
        }

        #endregion Health
    }

    public enum CompressedDirection
    {
        Static, // Статичная область
        Compress, // Сжатие области
        Expansion, // Расширение области
    }
}
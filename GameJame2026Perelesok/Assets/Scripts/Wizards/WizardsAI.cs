using System;
using UnityEngine;

namespace Wizards
{
    public sealed class WizardsAI : MonoBehaviour
    {
        [Header("Wizard Health")] 
        [SerializeField] private int health = 3;

        public void TakeDamage(int damage)
        {
            damage = Mathf.Abs(damage); // Убедиться, что урон положительный
            health -= damage;
            HealthState();
        }
        
    
        /// <summary>
        /// Метод для проверки состояния здоровья волшебника и выполнения действий в зависимости от текущего уровня здоровья.
        /// </summary>
        private void HealthState()
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
    }
}
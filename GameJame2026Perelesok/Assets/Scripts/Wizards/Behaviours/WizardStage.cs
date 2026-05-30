using System;
using System.Collections;
using UnityEngine;

namespace Wizards.Behaviours
{
    public class WizardStage : MonoBehaviour
    {
        protected WizardStateController _wizardStateController;
        protected bool stageActive = false;

        [SerializeField] protected GameObject leftHand; // Левая рука
        [SerializeField] protected GameObject rightHand; // Правая рука
        [SerializeField] protected GameObject targetCompress; // "Сжимаемая" цель

        [SerializeField] private float timeForСompression = 3; // Время через которое будет происходить сжатие рук/круга
        [SerializeField] private float speedСompression = 2; // Скорость сжатия рук/круга

        [SerializeField]
        private float minimalDistanceForeTargetCompress = 0.5f; // минимальное расстояние до сжимаемой цели

        private float threshold = 0.01f; // Допустимая погрешность достижения цели
        private bool isMoving = false; // Флаг, разрешающий движение

        private void Start()
        {
            _wizardStateController = WizardStateController.Instance;
            if (targetCompress.transform == null)
            {
                Debug.LogError("PointCompress не назначен в инспекторе!");
                return;
            }

            StartCoroutine(WaitAndMove());
        }

        private void Update()
        {
            if (isMoving)
            {
                CompressHand(leftHand.transform);
                CompressHand(rightHand.transform);
            }
        }

        public void CompressHand(Transform hand)
        {
            // Двигаем объект к цели с заданной скоростью
            hand.position = Vector3.MoveTowards(
                hand.position,
                targetCompress.transform.position,
                speedСompression * Time.deltaTime
            );

            // Проверяем достижение цели
            if (Vector3.Distance(hand.position, targetCompress.transform.position) < threshold)
            {
                isMoving = false;
                Debug.Log("Рука достигла точки PointCompress");
            }
        }

        IEnumerator WaitAndMove()
        {
            // Ожидаем заданный интервал времени
            yield return new WaitForSeconds(timeForСompression);
            isMoving = true;
            Debug.Log("Начинаем движение руки к точке PointCompress");
        }

        /// <summary>
        /// Запускаем стадию
        /// </summary>
        public void StartStage()
        {
            stageActive = true;
        }

        /// <summary>
        /// Выключаем стадию
        /// </summary>
        public void EndStage()
        {
            stageActive = false;
        }
    }
}
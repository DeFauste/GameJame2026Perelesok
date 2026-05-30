using System;
using System.Collections;
using System.Collections.Generic;
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

        [SerializeField] protected float timeForСompression = 3; // Время через которое будет происходить сжатие рук/круга
        [SerializeField] protected float speedСompression = 2; // Скорость сжатия рук/круга
        
        [SerializeField]
        protected float minimalDistanceForTargetPercent= 0.5f; // минимальное расстояние до сжимаемой цели
        [SerializeField] protected float sliceDistancePercent = 0.2f; // процент от начального расстояния, на который рука будет двигаться в каждом кадре
        
        private float threshold = 0.01f; // Допустимая погрешность достижения цели
        private bool isMoving = false; // Флаг, разрешающий движение

        private List<Coroutine> handMovementCoroutine = new List<Coroutine>();
        
        private void Start()
        {
            _wizardStateController = WizardStateController.Instance;
            if (targetCompress.transform == null)
            {
                Debug.LogError("PointCompress не назначен в инспекторе!");
                return;
            }

            handMovementCoroutine.Add(StartCoroutine(ApproachStepByStep(leftHand.transform))); // переместить в старт
            handMovementCoroutine.Add(StartCoroutine(ApproachStepByStep(rightHand.transform))); // переместить в старт
        }
        

        IEnumerator ApproachStepByStep(Transform hand)
        {
            // Первая пауза перед началом движения (опционально)
            yield return new WaitForSeconds(timeForСompression);
            var initialDistance = Vector3.Distance(hand.position, targetCompress.transform.position);

            while (true)
            {
                float currentDistance = Vector3.Distance(hand.transform.position, targetCompress.transform.position);
                if (currentDistance < initialDistance * minimalDistanceForTargetPercent)
                {
                    Debug.Log("Текущее расстояние меньше дозволенного");
                    yield break;
                }
                
                float currentDist = Vector3.Distance(hand.position, targetCompress.transform.position);
                if (currentDist <= threshold)
                {
                    Debug.Log("Рука достигла точки PointCompress");
                    yield break;
                }

                // Сколько нужно сократить расстояние на этом шаге (20% от начального)
                float reduction = sliceDistancePercent * initialDistance;
                // Нельзя сократить больше, чем осталось
                float moveDist = Mathf.Min(reduction, currentDist);
                float newDist = currentDist - moveDist;
                
                // Вычисляем позицию, куда должна прийти рука после этого шага
                Vector3 direction = (hand.position - targetCompress.transform.position).normalized;
                Vector3 targetPos = targetCompress.transform.position + direction * newDist;
                
                // Плавно двигаемся к targetPos с заданной скоростью
                while (Vector3.Distance(hand.position, targetPos) > 0.01f)
                {
                    hand.position = Vector3.MoveTowards(hand.position, targetPos, speedСompression * Time.deltaTime);
                    yield return null;
                }
                hand.position = targetPos; // фиксируем точное положение

                Debug.Log($"Шаг выполнен, осталось расстояние: {Vector3.Distance(hand.position, targetCompress.transform.position)}");

                // Ждём следующий интервал перед новым приближением
                yield return new WaitForSeconds(timeForСompression);
            }
        }

        /// <summary>
        /// Запускаем стадию
        /// </summary>
        public void StartStage()
        {
            stageActive = true;
            handMovementCoroutine.Add(StartCoroutine(ApproachStepByStep(leftHand.transform)));
            handMovementCoroutine.Add(StartCoroutine(ApproachStepByStep(rightHand.transform)));
        }

        /// <summary>
        /// Выключаем стадию
        /// </summary>
        public void EndStage()
        {
            stageActive = false;
            foreach (var coroutine in handMovementCoroutine)
            {
                StopCoroutine(coroutine);
            }
        }
    }
}
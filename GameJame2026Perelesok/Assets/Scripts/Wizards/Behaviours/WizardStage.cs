using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizards.Behaviours
{
    public class WizardStage : MonoBehaviour
    {
        public PlayerController playerController;
        SymbolSystem symbolSystem =  new ();
        
        [SerializeField] protected string NameMusicStage; // Название музыки для данной стадии
        protected WizardStateController _wizardStateController;
        protected bool stageActive = false;

        [SerializeField] protected GameObject leftHand; // Левая рука
        [SerializeField] protected GameObject rightHand; // Правая рука
        [SerializeField] protected GameObject targetCompress; // "Сжимаемая" цель

        [SerializeField] protected float delayTimeCompression = 3; // Время до начала сжатия
        [SerializeField] protected float timeFullCompression = 3; // Время до полного сжатия

        [SerializeField]
        protected float
            percentTimeExpansion = 0.2f; // Какой процент времени от максимального будет занимать полное расширение

        [SerializeField]
        protected float
            percentTimeExpansionDome =
                0.2f; // Какой процент времени от максимального будет занимать полное расширение купола

        [SerializeField]
        protected float minimalDistanceForTargetPercent = 0.5f; // минимальное расстояние до сжимаемой цели

        private float threshold = 0.01f; // Допустимая погрешность достижения цели
        private bool isMoving = false; // Флаг, разрешающий движение

        private Vector3 leftHandInitialPosition;
        private Vector3 rightHandInitialPosition;
        private float leftHandInitialDistance;
        private float rightHandInitialDistance;

        private List<Coroutine> coroutines = new List<Coroutine>();

        [SerializeField] protected SpriteRenderer spriteDomeToCompress; // Спрайт для сжатия

        private Vector3 spriteDomeInitialScale;
        private Vector3 spriteDomeMinimalScale;
        private Vector3 spriteDomeMaximalScale;

        [SerializeField] protected SpriteRenderer spriteCycleToCompress; // Спрайт для сжатия
        
        private Vector3 spriteCyicleInitialScale;
        private Vector3 spriteCyicleMinimalScale;
        private Vector3 spriteCyicleMaximalScale;
        
        // Переменная для синхронизации процессов сжатия/расширения
        private float compressionProgress = 0f; // 0 = расширено, 1 = сжато
        
        protected virtual void Start()
        {
            _wizardStateController = WizardStateController.Instance;
            if (targetCompress.transform == null)
            {
                Debug.LogError("PointCompress не назначен в инспекторе!");
                return;
            }

            SymbolSystem.OnDefenceSuccess += () =>
            {
                Debug.Log("Получили событие об успешной защите! Запускаем расширение.");
                _wizardStateController?.ChangeCompressedDirection(CompressedDirection.Expansion);
            };
            
            // Сохраняем начальные позиции и расстояния
            leftHandInitialPosition = leftHand.transform.position;
            rightHandInitialPosition = rightHand.transform.position;
            leftHandInitialDistance = Vector3.Distance(leftHand.transform.position, targetCompress.transform.position);
            rightHandInitialDistance =
                Vector3.Distance(rightHand.transform.position, targetCompress.transform.position);

            // Сохраняем начальный размер спрайта
            spriteDomeInitialScale = spriteDomeToCompress.transform.localScale;
            spriteDomeMinimalScale = spriteDomeInitialScale * minimalDistanceForTargetPercent; // Минимальный размер
            spriteDomeMaximalScale = spriteDomeInitialScale; // Максимальный размер
            
            // Сохраняем начальный размер спрайта
            spriteCyicleInitialScale = spriteCycleToCompress.transform.localScale;
            spriteCyicleMinimalScale = spriteCyicleInitialScale * minimalDistanceForTargetPercent; // Минимальный размер
            spriteCyicleMaximalScale = spriteCyicleInitialScale; // Максимальный размер
        }

        /// <summary>
        /// Главная корутина, управляющая прогрессом сжатия/расширения
        /// </summary>
        IEnumerator CompressionController()
        {
            yield return new WaitForSeconds(delayTimeCompression);

            while (true)
            {
                if (_wizardStateController.CurrentDirectionCompress == CompressedDirection.Compress)
                {
                    float compressionSpeed =
                        1f / timeFullCompression; // Прогресс в единицу за timeFullCompression секунд
                    compressionProgress = Mathf.Min(compressionProgress + compressionSpeed * Time.deltaTime, 1f);

                }
                else if (_wizardStateController.CurrentDirectionCompress == CompressedDirection.Expansion)
                {
                    float expansionSpeed = 1f / (timeFullCompression * (1f + percentTimeExpansion));
                    compressionProgress = Mathf.Max(compressionProgress - expansionSpeed * Time.deltaTime, 0f);
                }

                yield return null;
            }
        }

        /// <summary>
        /// Позиционирует руку на основе текущего прогресса сжатия (0 = начальная позиция, 1 = минимальное расстояние)
        /// </summary>
        private void UpdateHandPosition(Transform hand, Vector3 initialPosition, float initialDistance)
        {
            float minimalDistance = initialDistance * minimalDistanceForTargetPercent;
            float targetDistance = Mathf.Lerp(initialDistance, minimalDistance, compressionProgress);

            Vector3 directionToTarget = (targetCompress.transform.position - initialPosition).normalized;
            hand.position = initialPosition + directionToTarget * (initialDistance - targetDistance);
        }

        /// <summary>
        /// Масштабирует спрайт на основе текущего прогресса сжатия (0 = максимальный размер, 1 = минимальный размер)
        /// </summary>
        private void UpdateSpriteScale()
        {
            Vector3 targetScale = Vector3.Lerp(spriteDomeMaximalScale, spriteDomeMinimalScale, compressionProgress);
            spriteDomeToCompress.transform.localScale = targetScale;
            
            Vector3 targetScaleCycle = Vector3.Lerp(spriteCyicleMaximalScale, spriteCyicleMinimalScale, compressionProgress);
            spriteCycleToCompress.transform.localScale = targetScaleCycle;
            if (targetScale.x >= 0.95f)
            {
                Debug.Log("Получили событие об успешной защите! Запускаем расширение.");
                _wizardStateController?.ChangeCompressedDirection(CompressedDirection.Compress);
            }
            playerController?.ChangeWalkingZone(targetScale.x); // Предполагая, что размер зоны пропорционален размеру спрайта
            Debug.Log($"Updated sprite scale: {targetScale.x}");
        }

        private void LateUpdate()
        {
            if (stageActive)
            {
                // Обновляем позиции рук на основе текущего прогресса
                UpdateHandPosition(leftHand.transform, leftHandInitialPosition, leftHandInitialDistance);
                UpdateHandPosition(rightHand.transform, rightHandInitialPosition, rightHandInitialDistance);

                // Обновляем размер спрайта
                UpdateSpriteScale();
            }
        }

        public virtual void StartStage()
        {
            stageActive = true;
            compressionProgress = 0f; // Начинаем с полного расширения
            _wizardStateController?.ChangeCompressedDirection(CompressedDirection.Compress);

            coroutines.Add(StartCoroutine(CompressionController()));

            MusicService.Instance.Play(NameMusicStage, loop: true);
        }

        public void EndStage()
        {
            stageActive = false;
            foreach (var coroutine in coroutines)
            {
                StopCoroutine(coroutine);
            }

            MusicService.Instance.StopByName(NameMusicStage);

            coroutines.Clear();
        }
    }
}
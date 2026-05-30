using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizards.Behaviours
{
    public class WizardStage : MonoBehaviour
    {
        [SerializeField]
        protected string NameMusicStage; // Название музыки для данной стадии
        protected WizardStateController _wizardStateController;
        protected bool stageActive = false;

        [SerializeField] protected GameObject leftHand; // Левая рука
        [SerializeField] protected GameObject rightHand; // Правая рука
        [SerializeField] protected GameObject targetCompress; // "Сжимаемая" цель

        [SerializeField] protected float delayTimeCompression = 3; // Время до начала сжатия
        [SerializeField] protected float timeFullCompression = 3; // Время до полного сжатия
        [SerializeField] protected float percentTimeExpansion = 0.2f; // Какой процент времени от максимального будет занимать полное расширение

        [SerializeField]
        protected float minimalDistanceForTargetPercent = 0.5f; // минимальное расстояние до сжимаемой цели

        private float threshold = 0.01f; // Допустимая погрешность достижения цели
        private bool isMoving = false; // Флаг, разрешающий движение

        private Vector3 leftHandInitialPosition;
        private Vector3 rightHandInitialPosition;
        private float leftHandInitialDistance;
        private float rightHandInitialDistance;

        private List<Coroutine> coroutines = new List<Coroutine>();

        [SerializeField] protected SpriteRenderer spriteToCompress; // Спрайт для сжатия

        private Vector3 spriteInitialScale;
        private Vector3 spriteMinimalScale;
        private Vector3 spriteMaximalScale;

        private void Start()
        {
            _wizardStateController = WizardStateController.Instance;
            if (targetCompress.transform == null)
            {
                Debug.LogError("PointCompress не назначен в инспекторе!");
                return;
            }

            // Сохраняем начальные позиции и расстояния
            leftHandInitialPosition = leftHand.transform.position;
            rightHandInitialPosition = rightHand.transform.position;
            leftHandInitialDistance = Vector3.Distance(leftHand.transform.position, targetCompress.transform.position);
            rightHandInitialDistance =
                Vector3.Distance(rightHand.transform.position, targetCompress.transform.position);

            // Сохраняем начальный размер спрайта
            spriteInitialScale = spriteToCompress.transform.localScale;
            spriteMinimalScale = spriteInitialScale * minimalDistanceForTargetPercent; // Минимальный размер
            spriteMaximalScale = spriteInitialScale; // Максимальный размер
        }


        /// <summary>
        /// Универсальный метод для сжатия/расширения руки
        /// </summary>
        IEnumerator CompressExpandHand(Transform hand, Vector3 initialPosition, float initialDistance)
        {
            yield return new WaitForSeconds(delayTimeCompression);

            while (true)
            {
                if (_wizardStateController.CurrentDirectionCompress == CompressedDirection.Compress)
                {
                    // РЕЖИМ СЖАТИЯ: рука приближается к targetCompress
                    float currentDistance = Vector3.Distance(hand.position, targetCompress.transform.position);
                    float minimalDistance = initialDistance * minimalDistanceForTargetPercent;

                    // Проверяем, достигли ли минимального расстояния
                    if (!(currentDistance <= minimalDistance))
                    {
                        // Вычисляем скорость: расстояние / время сжатия
                        float compressionSpeed = currentDistance / timeFullCompression;

                        // Двигаемся к targetCompress
                        Vector3 direction = (targetCompress.transform.position - hand.position).normalized;
                        hand.position += direction * (compressionSpeed * Time.deltaTime);
                    }
                }
                else if (_wizardStateController.CurrentDirectionCompress == CompressedDirection.Expansion)
                {
                    // РЕЖИМ РАСШИРЕНИЯ: рука отдаляется от targetCompress к начальной позиции
                    float currentDistance = Vector3.Distance(hand.position, targetCompress.transform.position);

                    // Проверяем, вернулась ли рука в начальное положение
                    if (!(Vector3.Distance(hand.position, initialPosition) <= threshold))
                    {
                        // Вычисляем скорость: расстояние / время расширения
                        float expansionSpeed = currentDistance / timeFullCompression;

                        // Двигаемся к начальной позиции
                        Vector3 directionToInitial = (initialPosition - hand.position).normalized;
                        hand.position += directionToInitial *
                                         ((expansionSpeed + expansionSpeed *(1 - percentTimeExpansion)) * Time.deltaTime);
                    }
                    else
                    {
                        hand.position = initialPosition; // Фиксируем точное положение
                    }
                }

                yield return null;
            }
        }

        /// <summary>
        /// Увеличивает и уменьшает размер спрайта в зависимости от CompressedDirection
        /// </summary>
        IEnumerator CompressExpandSprite()
        {
            yield return new WaitForSeconds(delayTimeCompression);

            while (true)
            {
                if (_wizardStateController.CurrentDirectionCompress == CompressedDirection.Expansion)
                {
                    // РЕЖИМ Expansion: спрайт увеличивается
                    Vector3 currentScale = spriteToCompress.transform.localScale;

                    // Проверяем, достигли ли максимального размера
                    if (!(currentScale.x >= spriteMaximalScale.x))
                    {
                        // Вычисляем скорость изменения размера
                        float scaleChangeSpeed = (spriteMaximalScale.x - spriteMinimalScale.x) /
                                                 (timeFullCompression - timeFullCompression * (1 - percentTimeExpansion));

                        // Увеличиваем размер
                        Vector3 newScale = currentScale + Vector3.one * (scaleChangeSpeed * Time.deltaTime);
                        newScale = Vector3.Min(newScale, spriteMaximalScale);
                        spriteToCompress.transform.localScale = newScale;
                    }
                }
                else if (_wizardStateController.CurrentDirectionCompress == CompressedDirection.Compress)
                {
                    // РЕЖИМ Compress: спрайт уменьшается
                    Vector3 currentScale = spriteToCompress.transform.localScale;

                    // Проверяем, достигли ли минимального размера
                    if (!(currentScale.x <= spriteMinimalScale.x))
                    {
                        // Вычисляем скорость изменения размера с учетом ускорения
                        float scaleChangeSpeed = (spriteMaximalScale.x - spriteMinimalScale.x) / timeFullCompression;

                        // Уменьшаем размер (с ускорением на percentTimeExpansion)
                        Vector3 newScale = currentScale - Vector3.one *
                            ((scaleChangeSpeed + scaleChangeSpeed ) * Time.deltaTime);
                        newScale = Vector3.Max(newScale, spriteMinimalScale);
                        spriteToCompress.transform.localScale = newScale;
                    }
                    else
                    {
                        spriteToCompress.transform.localScale = spriteMinimalScale; // Фиксируем точный размер
                    }
                }

                yield return null;
            }
        }

        public void StartStage()
        {
            stageActive = true;
            _wizardStateController.ChangeCompressedDirection(CompressedDirection.Compress);
            coroutines.Add(StartCoroutine(CompressExpandHand(leftHand.transform, leftHandInitialPosition,
                leftHandInitialDistance)));
            coroutines.Add(StartCoroutine(CompressExpandHand(rightHand.transform, rightHandInitialPosition,
                rightHandInitialDistance)));
            coroutines.Add(StartCoroutine(CompressExpandSprite()));
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
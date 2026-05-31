using System;
using System.Collections;
using Commons;
using UnityEngine;

namespace Wizards.Animations
{
    public class WizardAnimationController : SingletonMonoBehaviour<WizardAnimationController>
    {
        #region HashAnimator
        // Слои
        private string firstLayerName = "First";
        private string secondLayerName = "Second";
        private string threeLayerName = "Three";
        private int firstIndex;
        private int secondIndex;
        private int threeIndex;
        private Coroutine currentFade;
        [SerializeField] private float fadeDuration = 0.25f;
        
        // Анимации
        private readonly int IdleHash = Animator.StringToHash("Idle");
        private readonly int BlinkHash = Animator.StringToHash("Blink");
        private readonly int MouthHash = Animator.StringToHash("Mouth");
        
        #endregion
        [SerializeField]
        private Animator _animator;

        private void Start()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            InitAnimatorLayer();
        }

        private void InitAnimatorLayer()
        {
            firstIndex = _animator.GetLayerIndex(firstLayerName);
            secondIndex = _animator.GetLayerIndex(secondLayerName);
            threeIndex = _animator.GetLayerIndex(threeLayerName);
            
            // Начальное состояние: First
            _animator.SetLayerWeight(firstIndex, 1f);
            _animator.SetLayerWeight(secondIndex, 0f);
            _animator.SetLayerWeight(threeIndex, 0f);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                SwitchToFirst();
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                SwitchToSecond();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                _animator.CrossFade(IdleHash, 0);
            }
            
            if (Input.GetKeyDown(KeyCode.N))
            {
                _animator.CrossFade(BlinkHash, 0);
            }
        }
        
        
        public void SwitchToFirst() => StartFade(firstIndex, secondIndex, threeIndex);
        public void SwitchToSecond() => StartFade(secondIndex, firstIndex, threeIndex);
        public void SwitchToThree() => StartFade(threeIndex, firstIndex, secondIndex);

        private void StartFade(int targetIndex, int layerToOff1, int layerToOff2)
        {
            if (currentFade != null)
                StopCoroutine(currentFade);
        
            currentFade = StartCoroutine(FadeToLayer(targetIndex, layerToOff1, layerToOff2));
        }

        private IEnumerator FadeToLayer(int targetIndex, int offIndex1, int offIndex2)
        {
            float startWeightTarget = _animator.GetLayerWeight(targetIndex);
            float startWeightOff1 = _animator.GetLayerWeight(offIndex1);
            float startWeightOff2 = _animator.GetLayerWeight(offIndex2);

            float time = 0f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float t = time / fadeDuration;
            
                // Плавно увеличиваем целевой слой до 1
                float newTargetWeight = Mathf.Lerp(startWeightTarget, 1f, t);
                // Плавно уменьшаем отключаемые слои до 0
                float newOff1Weight = Mathf.Lerp(startWeightOff1, 0f, t);
                float newOff2Weight = Mathf.Lerp(startWeightOff2, 0f, t);

                if (targetIndex != -1) _animator.SetLayerWeight(targetIndex, newTargetWeight);
                if (offIndex1 != -1) _animator.SetLayerWeight(offIndex1, newOff1Weight);
                if (offIndex2 != -1) _animator.SetLayerWeight(offIndex2, newOff2Weight);
            
                yield return null;
            }

            // Финальная установка
            if (targetIndex != -1) _animator.SetLayerWeight(targetIndex, 1f);
            if (offIndex1 != -1) _animator.SetLayerWeight(offIndex1, 0f);
            if (offIndex2 != -1) _animator.SetLayerWeight(offIndex2, 0f);
            _animator.StopPlayback();
            _animator.Play(IdleHash, targetIndex, 0);
            currentFade = null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wizards.Animations;

namespace Wizards.Behaviours.Lose
{
    public class LoseStage : WizardStage
    {
        public GameObject _symbolSystem;

        [Header("Dissolve settings")]
        [Tooltip("Длительность анимации растворения (в секундах)")]
        [SerializeField] private float dissolveDuration = 1.5f;

        [Tooltip("Пытаться применять шейдерный параметр (обычно \"_Cutoff\"). Если шейдеров с таким свойством нет — автоматический fallback на альфу цвета")] 
        [SerializeField] private bool tryUseCutoffShader = true;

        [Tooltip("Имя свойства шейдера для растворения (по умолчанию _Cutoff)")]
        [SerializeField] private string cutoffPropertyName = "_Cutoff";

        [Tooltip("Если true — собирать SpriteRenderer'ы во всех дочерних объектах; иначе — только на текущем объекте (если есть)")]
        [SerializeField] private bool includeChildrenRenderers = true;

        [Tooltip("Явный список SpriteRenderer'ов, если нужно контролировать конкретные рендереры (оставьте пустым, чтобы использовать GetComponentsInChildren)")]
        [SerializeField] private SpriteRenderer targetSpriteRenderer;
        [SerializeField] private Sprite targetSprite;
        [Header("On complete")]
        [SerializeField] private bool disableOnComplete = true;
        [SerializeField] private float completeDelay = 0f;

        private bool _isDissolving = false;

        public override void StartStage()
        {
            Debug.Log("Поражение! Стадия Lose началась.");
            base.StartStage();
            _wizardStateController.ChangeCompressedDirection(CompressedDirection.Expansion);
            MusicService.Instance.StopAll();
            MusicService.Instance.Play("Sound_EnemyLaugh_Defeat");
            _symbolSystem.SetActive(false);
            WizardAnimationService.Instance.LoseStage();
            rightHand.SetActive(false);
            leftHand.SetActive(false);
            
            StartDissolve();
        }

        private void StartDissolve()
        {
            if (_isDissolving) return;
            StartCoroutine(DissolveCoroutine());
        }

        private IEnumerator DissolveCoroutine()
        {
            _isDissolving = true;
            targetSpriteRenderer.sprite = targetSprite;
            // Собираем целевые SpriteRenderer'ы
            List<SpriteRenderer> srs = new List<SpriteRenderer>();
            if (targetSpriteRenderer != null)
            {
                srs.Add(targetSpriteRenderer);
            }
            else
            {
                if (includeChildrenRenderers)
                    srs.AddRange(GetComponentsInChildren<SpriteRenderer>());
                else
                {
                    var sr = GetComponent<SpriteRenderer>();
                    if (sr != null) srs.Add(sr);
                }
            }

            if (srs.Count == 0)
            {
                Debug.LogWarning("LoseStage: не найден ни один SpriteRenderer для растворения.");
            }

            // Проверим, есть ли у материалов свойство cutoff
            bool anySupportsCutoff = false;
            if (tryUseCutoffShader)
            {
                foreach (var sr in srs)
                {
                    if (sr == null) continue;
                    var mat = sr.sharedMaterial;
                    if (mat != null && mat.HasProperty(cutoffPropertyName))
                    {
                        anySupportsCutoff = true;
                        break;
                    }
                }

                // Если никто не поддерживает — переключаемся на fallback
                if (!anySupportsCutoff)
                {
                    tryUseCutoffShader = false;
                }
            }

            // Сохраним исходные цвета для fallback
            Color[] originalColors = new Color[srs.Count];
            for (int i = 0; i < srs.Count; i++)
            {
                originalColors[i] = srs[i] != null ? srs[i].color : Color.white;
            }

            MaterialPropertyBlock mpb = new MaterialPropertyBlock();

            float elapsed = 0f;
            while (elapsed < dissolveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / Mathf.Max(0.0001f, dissolveDuration));

                if (tryUseCutoffShader)
                {
                    float cutoffValue = t; // 0 - видим, 1 - исчез
                    for (int i = 0; i < srs.Count; i++)
                    {
                        var sr = srs[i];
                        if (sr == null) continue;
                        sr.GetPropertyBlock(mpb);
                        mpb.SetFloat(cutoffPropertyName, cutoffValue);
                        sr.SetPropertyBlock(mpb);
                    }
                }
                else
                {
                    for (int i = 0; i < srs.Count; i++)
                    {
                        var sr = srs[i];
                        if (sr == null) continue;
                        Color c = sr.color;
                        c.a = Mathf.Lerp(originalColors[i].a, 0f, t);
                        sr.color = c;
                    }
                }

                yield return null;
            }

            // Финальное состояние
            if (tryUseCutoffShader)
            {
                for (int i = 0; i < srs.Count; i++)
                {
                    var sr = srs[i];
                    if (sr == null) continue;
                    sr.GetPropertyBlock(mpb);
                    mpb.SetFloat(cutoffPropertyName, 1f);
                    sr.SetPropertyBlock(mpb);
                }
            }
            else
            {
                for (int i = 0; i < srs.Count; i++)
                {
                    var sr = srs[i];
                    if (sr == null) continue;
                    Color c = sr.color;
                    c.a = 0f;
                    sr.color = c;
                }
            }

            if (completeDelay > 0f)
                yield return new WaitForSeconds(completeDelay);

            if (disableOnComplete)
                gameObject.SetActive(false);

            _isDissolving = false;
            
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }
    }
}
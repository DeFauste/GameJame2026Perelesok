using System.Collections;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private float _duration = 0.25f;

    private Coroutine _coroutine;

    public void Play()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        _renderer.enabled = true;

        Color color = _renderer.color;

        float time = 0f;

        while (time < _duration)
        {
            float alpha = 1f - time / _duration;

            color.a = alpha;
            _renderer.color = color;

            time += Time.deltaTime;
            yield return null;
        }

        _renderer.enabled = false;

        color.a = 1f;
        _renderer.color = color;
    }
}

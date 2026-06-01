using System;
using System.Collections;
using UnityEngine;

namespace MainMenu
{
    public class LinearMovement : MonoBehaviour
    {
        [SerializeField] private float _duration = 10f;
        [SerializeField] private float _targetX = 1850f;
        [SerializeField] private float _initialX = -1850f;

        private RectTransform _rectTransform;
        private float _currentPositionX;
        private float _currentDuration;
        private Coroutine _coroutine;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            //_currentPositionX = _rectTransform.anchoredPosition.x;
        }

        private void Start()
        {
            _coroutine = StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            while (true)
            {
                var time = 0f;
                _currentPositionX = _rectTransform.anchoredPosition.x;
                _currentDuration = GetDuration();
                while (time < _currentDuration)
                {
                    var percentage = time / _currentDuration;
                    var vector2 = _rectTransform.anchoredPosition;
                    vector2.x = Mathf.Lerp(_currentPositionX, _targetX, percentage);
                    _rectTransform.anchoredPosition = vector2;
                    time += Time.deltaTime;
                    yield return null;
                }

                var position = _rectTransform.anchoredPosition;
                position.x = _initialX;
                _rectTransform.anchoredPosition = position; 
            }
        }

        private float GetDuration()
        {
            return (_targetX - _currentPositionX) / (_targetX - _initialX) * _duration;
        }
    }
}
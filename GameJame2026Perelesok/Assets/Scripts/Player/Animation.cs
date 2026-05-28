using UnityEngine;
using System.Collections.Generic;

public class Animation : MonoBehaviour
{
    [SerializeField] private List<Material> _idle;
    [SerializeField] private Material _walk1;
    [SerializeField] private Material _walk2;
    [SerializeField] private Material _walk3;
    private MeshRenderer _player;
    private int _state = 0;
    private float _timeDelay = 0;

    private void Awake()
    {
        _player = GetComponentInChildren<MeshRenderer>();
    }

    public void PlayAnimation(Vector2 inputs)
    {
        if (inputs == Vector2.zero)
            _player.material = _idle[_state];

        if (_timeDelay >= 0.2)
        {
            _state = (_state + 1) % 3;
            _timeDelay = 0;
        }

        _timeDelay += Time.deltaTime;
    }

    private void PlayIdleAnimation()
    {
        
    }
}

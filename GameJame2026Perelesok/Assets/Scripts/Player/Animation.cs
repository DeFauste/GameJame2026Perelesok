using UnityEngine;
using System.Collections.Generic;

public class Animation : MonoBehaviour
{
    [SerializeField] private List<Material> _idle;
    [SerializeField] private List<Material> _walk_right;
    [SerializeField] private List<Material> _walk_left;
    [SerializeField] private List<Material> _walk_forward;
    [SerializeField] private List<Material> _walk_backward;
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
            _player.material = _idle[_state % _idle.Count];
        else if (inputs.x != 0)
        {
            if (inputs.x > 0)
                _player.material = _walk_right[_state % _walk_right.Count];
            else
                _player.material = _walk_left[_state % _walk_left.Count];
        }
        else if (inputs.y != 0)
        {
            if (inputs.y > 0)
                _player.material = _walk_backward[_state % _walk_backward.Count];
            else
                _player.material = _walk_forward[_state % _walk_forward.Count];
        }

            if (_timeDelay >= 0.2)
        {
            _state = (_state + 1) % 6;
            _timeDelay = 0;
        }

        _timeDelay += Time.deltaTime;
    }

    private void PlayIdleAnimation()
    {
        
    }
}

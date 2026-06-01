using UnityEngine;
using System.Collections.Generic;

public class Animation : MonoBehaviour
{
    [SerializeField] private List<Sprite> _idle;
    [SerializeField] private List<Sprite> _walk_right;
    [SerializeField] private List<Sprite> _walk_left;
    [SerializeField] private List<Sprite> _walk_forward;
    [SerializeField] private List<Sprite> _walk_backward;
    private SpriteRenderer _player;
    private int _state = 0;
    private float _timeDelay = 0;

    private void Awake()
    {
        _player = GetComponentInChildren<SpriteRenderer>();
    }

    public void PlayAnimation(Vector2 inputs)
    {
        if (inputs == Vector2.zero)
        {
            _player.sprite = _idle[_state % _idle.Count];
        }
        else if (inputs.y != 0)
        {
            if (inputs.y > 0)
                _player.sprite = _walk_backward[_state % _walk_backward.Count];
            else
                _player.sprite = _walk_forward[_state % _walk_forward.Count];
        }
        else if (inputs.x != 0)
        {
            if (inputs.x > 0)
                _player.sprite = _walk_right[_state % _walk_right.Count];
            else
                _player.sprite = _walk_left[_state % _walk_left.Count];
        }


        if (_timeDelay >= 0.2)
        {
            _state = (_state + 1) % 6;
            _timeDelay = 0;
        }

        _timeDelay += Time.deltaTime;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class Animation : MonoBehaviour
{
    [SerializeField] private float _attackTime = .5f;

    private Animator _animator;
    private float _attackTimer;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void PlayAnimation(Vector2 inputs, bool attack, bool death)
    {
        if (death)
        {
            _animator.Play("Death");
        }
        else if (attack || _attackTimer > 0)
        {
            _animator.Play("Attack");
        }
        else if (inputs == Vector2.zero)
        {
            _animator.Play("Idle");
        }
        else if (inputs.y != 0)
        {
            if (inputs.y < 0)
                _animator.Play("Forward");
            else
                _animator.Play("Backward");
        }
        else if (inputs.x != 0)
        {
            if (inputs.x > 0)
                _animator.Play("Right");
            else
                _animator.Play("Left");
        }

        if (attack)
        {
            _attackTimer = _attackTime;
        }
        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
        }
    }
}

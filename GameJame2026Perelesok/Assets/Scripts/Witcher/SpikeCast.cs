using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeCast : MonoBehaviour
{
    [SerializeField] protected Vector2 _colliderSize = Vector2.one;
    [SerializeField] protected Vector2 _colliderCenter = Vector2.down;
    [SerializeField] protected float _castTime = 1;
    [SerializeField] protected float _castDelay = 1;

    private Animator _animator;
    protected bool _isHit = false;

    public bool IsHit { get { Destroy(gameObject); return _isHit; } }
    
    public static event Action IsHitAction;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    virtual public IEnumerator CastCorutine(float multypyer)
    {
        _animator.speed = multypyer;
        _animator.Play("SpikeShow");
        yield return new WaitForSecondsRealtime(_castDelay * multypyer);

        _animator.Play("AttackAinmation");

        for (float i = 0; i < _castTime; i += Time.deltaTime)
        {
            DrawCollider(Time.deltaTime * 2);

            if (CheckForCollision())
            {
                _isHit = true;
            }

            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
        
        if (!_isHit)
        {
            _animator.speed = multypyer;
            _animator.Play("RemoveSpike");
            yield return new WaitForSecondsRealtime(_castDelay * multypyer);
        }

        if (_isHit)
        {
            InvokeDeath();
        }

        yield return null;
    }

    protected void InvokeDeath()
    {
        IsHitAction.Invoke();
    }

    protected bool CheckForCollision()
    {
        PlayerController player = FindAnyObjectByType(typeof(PlayerController)).GetComponent<PlayerController>();
        if (player.CollisionDetection(_colliderSize, _colliderCenter + new Vector2(transform.position.x, transform.position.y)))
            return true;
        else
            return false;
    }

    protected void DrawCollider(float duration)
    {
        Debug.DrawLine(transform.position + Vector3.right * _colliderSize.x + Vector3.up * _colliderSize.y + (Vector3)_colliderCenter,
                       transform.position - Vector3.right * _colliderSize.x - Vector3.up * _colliderSize.y + (Vector3)_colliderCenter,
                       Color.blue, duration);
        Debug.DrawLine(transform.position + Vector3.right * _colliderSize.x - Vector3.up * _colliderSize.y + (Vector3)_colliderCenter,
                       transform.position - Vector3.right * _colliderSize.x + Vector3.up * _colliderSize.y + (Vector3)_colliderCenter,
                       Color.blue, duration);
    }
}

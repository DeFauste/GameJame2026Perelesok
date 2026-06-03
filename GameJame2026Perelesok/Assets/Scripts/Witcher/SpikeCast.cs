using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeCast : MonoBehaviour
{
    [SerializeField] protected Vector2 _colliderSize = Vector2.one;
    [SerializeField] protected Vector2 _colliderCenter = Vector2.down;
    [SerializeField] protected float _castTime = 1;
    [SerializeField] protected float _castDelay = 1;
    [SerializeField] private List<Sprite> _spike;

    private Animator _animator;

    private SpriteRenderer _spikeRender;
    private bool _isHit = false;

    private void Awake()
    {
        _spikeRender = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    virtual public IEnumerator CastCorutine(float multypyer)
    {
        _animator.speed = multypyer;
        _animator.Play("AttackAnimation");

        yield return new WaitForSecondsRealtime(_castDelay * multypyer);

        for (float i = 0; i < _castTime; i += Time.deltaTime)
        {
            DrawCollider(Time.deltaTime * 2);

            if (CheckForCollision())
            {
                _isHit = true;
                Debug.Log("hitted");
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
            yield break;
        }

        Destroy(gameObject);
        yield return null;
    }

    protected bool CheckForCollision()
    {
        PlayerController player = FindAnyObjectByType(typeof(PlayerController)).GetComponent<PlayerController>();
        if (player.CollisionDetection(_colliderSize, _colliderCenter + new Vector2(transform.position.x, transform.position.y)))
            return true;
        else
            return false;
    }

    private void SpikeAnimationCicle(float time)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            _spikeRender.sprite = _spike[(int)(i / time * _spike.Count)];
        }
    }

    private void ReverseSpikeAnimationCicle(float time)
    {
        for (float i = time * (-1); i < 0; i += Time.deltaTime)
        {
            _spikeRender.sprite = _spike[(int)(i / time * (_spike.Count - 1)) * (-1)];
        }
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

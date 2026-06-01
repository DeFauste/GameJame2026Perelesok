using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private Vector2 _colliderSize = Vector2.one;
    [SerializeField] private Vector2 _colliderCenter = Vector2.down;
    [SerializeField] private List<Sprite> _laser;
    [SerializeField] private float _castTime = 1;
    [SerializeField] private float _castDelay = 1;

    private SpriteRenderer _spikeRender;
    private int _state = 0;
    private bool _isHit = false;

    public bool IsHit {  get { return _isHit; } }

    private void Awake()
    {
        _spikeRender = GetComponentInChildren<SpriteRenderer>();
        DrawCollider(_castTime + _castDelay);
        StartCoroutine(CastCorutine(1));
    }

    public IEnumerator CastCorutine(float multypyer)
    {
        for (float i = 0; i < _castDelay; i += Time.deltaTime)
        {

        }

        for (float i = 0; i < _castTime; i += Time.deltaTime)
        {
            if (CheckForCollision())
                _isHit = true;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (!_isHit)
        {
            _spikeRender.sprite = _laser[-1];
        }

        yield return null;
    }

    private bool CheckForCollision()
    {
        PlayerController player = FindAnyObjectByType(typeof(PlayerController)).GetComponent<PlayerController>();
        if (player.CollisionDetection(_colliderSize, _colliderCenter + new Vector2(transform.position.x, transform.position.y)))
            return true;
        else
            return false;
    }

    private void DrawCollider(float duration)
    {
        Debug.DrawLine(transform.position + Vector3.right * _colliderSize.x + Vector3.up * _colliderSize.y + (Vector3)_colliderCenter,
                       transform.position - Vector3.right * _colliderSize.x - Vector3.up * _colliderSize.y + (Vector3)_colliderCenter,
                       Color.blue, duration);
        Debug.DrawLine(transform.position + Vector3.right * _colliderSize.x - Vector3.up * _colliderSize.y + (Vector3)_colliderCenter,
                       transform.position - Vector3.right * _colliderSize.x + Vector3.up * _colliderSize.y + (Vector3)_colliderCenter,
                       Color.blue, duration);
    }
}

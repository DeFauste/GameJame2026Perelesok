using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeCast : MonoBehaviour
{
    [SerializeField] private Vector2 _colliderSize = Vector2.one;
    [SerializeField] private Vector2 _colliderCenter = Vector2.down;
    [SerializeField] private Sprite _preCast;
    [SerializeField] private Sprite _spike;
    [SerializeField] private float _castTime = 1;
    [SerializeField] private float _castDelay = 1;

    private SpriteRenderer _spikeRender;
    private bool _isHit = false;

    private void Awake()
    {
        _spikeRender = GetComponentInChildren<SpriteRenderer>();
        DrawCollider(_castTime + _castDelay);
        StartCoroutine(CastCorutine());
    }

    public IEnumerator CastCorutine()
    {
        _spikeRender.sprite = _preCast;

        yield return new WaitForSeconds(_castDelay);

        _spikeRender.sprite = _spike;
        for (float i = 0; i < _castTime; i += Time.deltaTime)
        {

            if (CheckForCollision())
                _isHit = true;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (!_isHit)
        {
            _spikeRender.sprite = _preCast;
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

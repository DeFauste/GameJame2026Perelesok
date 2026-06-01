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
    }

    public IEnumerator CastCorutine(float multypyer)
    {
        for (int i = 0; i < 3; i++)
        {
            LaserAnimationCicle(_castDelay * multypyer / 3);
        }

        for (float i = 0; i < _castTime; i += Time.deltaTime)
        {
            _spikeRender.sprite = _laser[(int)(Mathf.Min(i * 4 / _castTime, 1) * 5)];

            if (CheckForCollision())
                _isHit = true;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (!_isHit)
        {
            for (float i = 0; i < 0.1 * multypyer; i += Time.deltaTime)
            {
                _spikeRender.sprite = _laser[(int)(i / 0.1f * (_laser.Count - 5)) + 5];
            }
        }

        yield return null;
    }

    public void Pose(Vector2 place, Vector2 eyea)
    {
        Vector2 buf = (place + eyea) / 2;
    }

    private void LaserAnimationCicle(float time)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            _spikeRender.sprite = _laser[(int)((i/time) * _laser.Count)];
        }
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

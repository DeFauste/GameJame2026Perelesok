using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : SpikeCast
{
    [SerializeField] private List<Sprite> _laser;


    private SpriteRenderer _spikeRender;
    private bool _isHit = false;

    public bool IsHit {  get { return _isHit; } }

    private void Awake()
    {
        _spikeRender = GetComponentInChildren<SpriteRenderer>();
        DrawCollider(_castTime + _castDelay);
    }

    override public IEnumerator CastCorutine(float multypyer)
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

        if (_isHit)
        {
            yield break;
        }

        Destroy(gameObject);
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
}

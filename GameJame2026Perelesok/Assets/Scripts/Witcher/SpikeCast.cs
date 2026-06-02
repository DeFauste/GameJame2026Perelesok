using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SpikeCast : MonoBehaviour
{
    [SerializeField] private Vector2 _colliderSize = Vector2.one;
    [SerializeField] private Vector2 _colliderCenter = Vector2.down;
    [SerializeField] private List<Sprite> _spike;
    [SerializeField] private float _castTime = 1;
    [SerializeField] private float _castDelay = 1;

    private SpriteRenderer _spikeRender;
    private bool _isHit = false;

    private void Awake()
    {
        _spikeRender = GetComponentInChildren<SpriteRenderer>();
    }

    virtual public IEnumerator CastCorutine(float multypyer)
    {
        DrawCollider(_castTime + _castDelay);
        SpikeAnimationCicle(_castDelay * multypyer);

        for (float i = 0; i < _castTime; i += Time.deltaTime)
        {

            if (CheckForCollision())
            {
                _isHit = true;
                Debug.Log("hitted");
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (!_isHit)
        {
            ReverseSpikeAnimationCicle(_castDelay * multypyer);
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
        for (float i = time; i > 0; i -= Time.deltaTime)
        {
            _spikeRender.sprite = _spike[(int)(i / time * _spike.Count)];
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

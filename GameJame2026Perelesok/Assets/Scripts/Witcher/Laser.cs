using System.Collections;
using UnityEngine;

public class Laser : SpikeCast
{

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    override public IEnumerator CastCorutine(float multypyer)
    {
        _animator.speed = 1;
        for (int i = 0; i < 3; i++)
        {
            _animator.Play("ShowLaser");
            yield return new WaitForSecondsRealtime(_castDelay * multypyer / 3 / 2);
            _animator.Play("RemoveLaser");
            yield return new WaitForSecondsRealtime(_castDelay * multypyer / 3 / 2);
        }

        _animator.Play("ShowLaser");

        for (float i = 0; i < _castTime * multypyer; i += Time.deltaTime)
        {
            DrawCollider(Time.deltaTime * 2);

            if (CheckForCollision())
            {
                _isHit = true;
                break;
            }

            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }

        if (!_isHit)
        {
            _animator.Play("RemoveLaser");
        }

        if (_isHit)
        {
            InvokeDeath();
        }

        Destroy(gameObject);
    }

    public void Pose(Vector2 eye)    
    {    
        Vector2 shotPosition = new Vector2(transform.position.x, transform.position.y);    
        Vector2 midlePoint = (shotPosition + eye) / 2;    
        float angle = 180 - Mathf.Atan2(shotPosition.x - midlePoint.x, shotPosition.y - midlePoint.y) * 180 / Mathf.PI;    
        // Debug.Log(shotPosition + " " + midlePoint + " | " + angle);    
        _colliderCenter = (Vector2)transform.position - midlePoint;    
        transform.position = midlePoint;    
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));    
        transform.localScale += Vector3.up * (eye - _colliderCenter).magnitude / 5f;    
    }
}

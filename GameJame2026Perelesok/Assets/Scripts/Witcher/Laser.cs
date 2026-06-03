using System.Collections;
using UnityEngine;

public class Laser : SpikeCast
{
    private Animator _animator;

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
    }

    public void Pose(Vector2 eye)
    {
        Debug.Log(transform.position);
        Vector2 place = new Vector2(transform.position.x, transform.position.y);
        Vector2 buf = (place + eye) / 2;
        float angle = Mathf.Atan2(buf.y, buf.x);
        if (place.x > 0)
        {
            angle *= -1;
        }
        transform.position = buf;
        transform.rotation *= Quaternion.Euler(angle, 0, 0);
    }
}

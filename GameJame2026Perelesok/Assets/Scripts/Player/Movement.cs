using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float _horizontalSpeed = 1f;
    [SerializeField] private float _verticalSpeed = 1f;
    [SerializeField] private float _hight = .5f;
    [SerializeField] private float _wight = 1f;
    [SerializeField] private Vector3 _zero = Vector3.zero;
    private GameObject _player;

    public float SetElipce_Hight { set { _hight = value; } }
    public float SetElipce_Wight { set { _wight = value; } }

    private void Awake()
    {
        _player = GetComponent<GameObject>();
        _zero = transform.position;
    }

    public void Move(Vector2 move)
    {
        Vector3 velocity = new Vector3(move.x * _horizontalSpeed, move.y * _verticalSpeed, 0);
        Vector3 new_pos = _player.transform.position + velocity;
        float distance = elipce_formula(new_pos);
        if (distance < 1)
        {
            _player.transform.position = new_pos;
        }
        else if (distance >= 1)
        {
            _player.transform.position = (_zero - _player.transform.position).normalized;
        }
    }

    // x^2/a^2 + y^2/b^2 = 1
    float elipce_formula(Vector3 pos)
    {
        return pos.x * pos.x / _wight * _wight + pos.y * pos.y / _hight * _hight;
    }
}

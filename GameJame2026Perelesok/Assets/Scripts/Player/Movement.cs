using UnityEngine;
using System;

public class Movement : MonoBehaviour
{
    [Header("Настройка игрока")]
    [SerializeField] private float _horizontalSpeed = 1f;
    [SerializeField] private float _verticalSpeed = 1f;
    [Header("Настройка поля")]
    [SerializeField] private float _hight = .5f;
    [SerializeField] private float _wight = 1f;

    private Vector3 _zero = Vector3.zero;
    private Transform _player;

    private void Awake()
    {
        _player = GetComponent<Transform>();
        _zero = _player.position;
    }

    public void Move(Vector2 move)
    {
        Vector3 velocity = new Vector3(move.x * _horizontalSpeed, move.y * _verticalSpeed, 0);
        Vector3 new_pos = _player.position + velocity - Vector3.forward * _player.position.z;
        float distance = elipce_formula(new_pos - _zero);
        if (distance < 1)
        {
            _player.position = new_pos;
            //_player.position = Vector3.Lerp(_player.position, new_pos, Time.deltaTime);
        }
        else if (distance >= 1)
        {
            _player.position += (_zero - _player.position) * _horizontalSpeed * _verticalSpeed;
            //_player.position = Vector3.Lerp(_player.position, _zero, Time.deltaTime);
        }
    }

    public void Scaler()
    {
        // 5 is scale range (.25 for 8, 1 for 2 and .4 for 5)
        // .8f is scale addition constant (.8 + .4 = 1.2)
        _player.localScale = Vector3.one * ((_zero.y - _player.position.y + _hight)/(_hight * 5) + .8f); 
    }

    public void LayerCalculation()
    {
        _player.position += Vector3.forward * _player.position.y;
    }

    public void ChangeZoneSize(float changeValue)
    {
        _hight *= changeValue;
        _wight *= changeValue;
    }

    // x^2/a^2 + y^2/b^2 = 1; a - length of halth diameter, b - lenth
    private float elipce_formula(Vector3 pos)
    {
        return pos.x * pos.x / (_wight * _wight) + pos.y * pos.y / (_hight * _hight);
    }
}

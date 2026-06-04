using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Íàñòðîéêà èãðîêà")]
    [SerializeField] private float _horizontalSpeed = 1f;
    [SerializeField] private float _verticalSpeed = 1f;
    [Header("Íàñòðîéêà ïîëÿ")]
    [SerializeField] private float _hight = .5f;
    [SerializeField] private float _wight = 1f;
    [SerializeField] private float _hightCorrection = .01f;

    private Vector3 _zero = Vector3.zero;
    private Transform _player;
    private float _elipseMultyplyer = 1;

    public Vector3 Zero { get { return _zero; } }

    private void Awake()
    {
        _player = GetComponent<Transform>();
        _zero = _player.position;
        _player.position += Vector3.up * _hight;
    }

    public void Move(Vector2 move)
    {
        Vector3 velocity = new Vector3(move.x * _horizontalSpeed, move.y * _verticalSpeed, 0);
        Vector3 new_pos = new Vector3((_player.position + velocity).x, (_player.position + velocity).y, 0);
        float distance = ElipceFormula(new_pos - _zero - new Vector3(0, (_hight + _hightCorrection) * _elipseMultyplyer, 0));
        if (distance < 1)
        {
            _player.position = new_pos;
        }
        else if (distance >= 1)
        {
            _player.position += (_zero - _player.position + new Vector3(0, (_hight + _hightCorrection) * _elipseMultyplyer)) * 0.01f;
        }
        DrawElipce(.1f);
    }

    public void Scaler()
    {
        // 5 is scale range (.25 for 8, 1 for 2 and .4 for 5)
        // .8f is scale addition constant (.8 + .4 = 1.2)
        _player.localScale = Vector3.one * ((_zero.y - _player.position.y + _hight)/(_hight * 5) + .8f); 
    }

    public void LayerCalculation()
    {
        _player.position = new Vector3(_player.position.x, _player.position.y, _player.position.y);
    }

    public void ChangeZoneSize(float changeValue)
    {
        _elipseMultyplyer = changeValue;
    }

    public Vector2 GetScales()
    {
        return new Vector2(_wight * _elipseMultyplyer, _hight * _elipseMultyplyer);
    }

    // x^2/a^2 + y^2/b^2 = 1; a - length of halth diameter, b - lenth of whight half diameter
    private float ElipceFormula(Vector3 pos)
    {
        return pos.x * pos.x / (_wight * _wight * _elipseMultyplyer * _elipseMultyplyer) + pos.y * pos.y / (_hight * _hight * _elipseMultyplyer * _elipseMultyplyer);
    }

    private void DrawElipce(float duration)
    {
        Debug.DrawLine(new Vector3(0, CalculateElipce_Y(0), _player.position.z) + _zero + new Vector3(0, (_hight + _hightCorrection) * _elipseMultyplyer, 0),
                       new Vector3(0, CalculateElipce_Y(0) * (-1), _player.position.z) + _zero + new Vector3(0, (_hight + _hightCorrection) * _elipseMultyplyer, 0), Color.red, duration);
        Debug.DrawLine(new Vector3(CalculateElipce_X(0), 0, _player.position.z) + _zero + new Vector3(0, (_hight + _hightCorrection) * _elipseMultyplyer, 0),
                       new Vector3(CalculateElipce_X(0) * (-1), 0, _player.position.z) + _zero + new Vector3(0, (_hight + _hightCorrection) * _elipseMultyplyer, 0), Color.red, duration);
    }

    public float CalculateElipce_X(float y)
    {
        float a = (float)Math.Pow(_wight * _elipseMultyplyer, 2);
        float y_b = (float)(Math.Pow(y, 2) / Math.Pow(_hight * _elipseMultyplyer, 2));
        return (float)Math.Sqrt(Math.Abs(a * y_b + a));
    }

    public float CalculateElipce_Y(float x)
    {
        float b = (float)Math.Pow(_hight * _elipseMultyplyer, 2);
        float x_a = (float)(Math.Pow(x, 2) / Math.Pow(_wight * _elipseMultyplyer, 2));
        return (float)Math.Sqrt(Math.Abs(b * x_a - b));
    }
}

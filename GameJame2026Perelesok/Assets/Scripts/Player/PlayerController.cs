using UnityEngine;


public enum Inputs{
    None = 0,
    left = 1,
    right = 2,
    up = 3,
    down = 4
}

[RequireComponent(typeof(Movement))]
public class PlayerController : MonoBehaviour
{
    private const string _left = "a";
    private const string _right = "d";
    private const string _forward = "w";
    private const string _backward = "s";
    private const string _attack = "space";
    private const string _deffence_1 = "left";
    private const string _deffence_2 = "right";
    private const string _deffence_3 = "up";
    private const string _deffence_4 = "down";

    [SerializeField] private float _multyplayer = 1.1f;
    [SerializeField] private WalkingZoneScaler _zone;
    [SerializeField] private Vector2 _colliderSize = Vector2.one;
    [SerializeField] private Vector2 _colliderCenter = Vector2.down;
    
    private Movement _playerMove;
    private Animation _playerAnim;

    private float _inputTimer;

    void Awake()
    {
        _playerMove = GetComponent<Movement>();
        _playerAnim = GetComponentInChildren<Animation>();
    }

    void Update()
    {
        Vector2 move = MovementInput();
        _playerMove.Move(move);
        _playerMove.LayerCalculation();
        _playerMove.Scaler();
        _playerAnim.PlayAnimation(move);
        if (_inputTimer > 0)
            ChooseDeffence();

        DrawCollider(.1f);
    }

    public bool CollisionDetection(Vector2 size, Vector2 center)
    {
        if (Collisin_X(size.x / 2, center.x) && Collisin_Y(size.y / 2, center.y))
            return true;
        else
            return false;
    }

    private Vector2 MovementInput()
    {
        Vector2 move = Vector2.zero;
        if (Input.GetKey(_left))
        {
            move.x -= 1;
        }
        if (Input.GetKey(_right))
        {
            move.x += 1;
        }
        if (Input.GetKey(_forward))
        {
            move.y += 1;
        }
        if (Input.GetKey(_backward))
        {
            move.y -= 1;
        }

        return move;
    }

    private void ChangeWalkingZone(float zoneMultiplyer)
    {
        _playerMove.ChangeZoneSize(zoneMultiplyer);
        _zone.ChangeScalesConst(new Vector3(zoneMultiplyer, _playerMove.GetScales().y * zoneMultiplyer / _playerMove.GetScales().x, 1));
    }

    private int ChooseDeffence()
    {
        float rand = Random.Range(0f, 1f);

        if (rand < .25f)
        {
            return 1;
        }
        if (rand < .5f)
        {
            return 2;
        }
        if (rand < .75f)
        {
            return 3;
        }
        else
        {
            return 4;
        }
    }

    private bool Collisin_X(float halfSize, float center)
    {
        if (transform.position.x + _colliderSize.x + _colliderCenter.x > center - halfSize || transform.position.x - _colliderSize.x + _colliderCenter.x < center + halfSize)
            return true;
        else
            return false;
    }

    private bool Collisin_Y(float halfSize, float center)
    {
        if (transform.position.y + _colliderSize.y + _colliderCenter.y > center - halfSize || transform.position.y - _colliderSize.y + _colliderCenter.y < center + halfSize)
            return true;
        else
            return false;
    }

    private void DrawCollider(float duration)
    {
        Debug.DrawLine(transform.position + Vector3.right * _colliderSize.x + Vector3.up * _colliderSize.y + (Vector3)_colliderCenter,
                       transform.position - Vector3.right * _colliderSize.x - Vector3.up * _colliderSize.y + (Vector3)_colliderCenter,
                       Color.green, duration);
        Debug.DrawLine(transform.position + Vector3.right * _colliderSize.x - Vector3.up * _colliderSize.y + (Vector3)_colliderCenter,
                       transform.position - Vector3.right * _colliderSize.x + Vector3.up * _colliderSize.y + (Vector3)_colliderCenter,
                       Color.green, duration);
    }
}

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

        Debug.Log(_multyplayer);
        ChangeWalkingZone(_multyplayer);
        _multyplayer -= Time.deltaTime / 10;
    }

    Vector2 MovementInput()
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


}

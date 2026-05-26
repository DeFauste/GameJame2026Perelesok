using UnityEngine;


public enum Inputs{
    None = 0,
    w = 1,
    a = 2,
    s = 3,
    d = 4
}

[RequireComponent(typeof(Movement))]
public class PlayerController : MonoBehaviour
{
    private const string _left = "left";
    private const string _right = "right";
    private const string _forward = "up";
    private const string _backward = "down";
    private const string _attack = "space";
    private const string _deffence_w = "w";
    private const string _deffence_a = "a";
    private const string _deffence_s = "s";
    private const string _deffence_d = "d";

    private Movement _playerMove;

    void Awake()
    {
        _playerMove = GetComponent<Movement>();
    }

    void Update()
    {
        _playerMove.Move(MovementInput());
    }

    Vector2 MovementInput()
    {
        Vector2 move = Vector2.zero;
        if (Input.GetButton(_left))
        {
            move.x -= 1;
        }
        if (Input.GetButton(_right))
        {
            move.x -= 1;
        }
        if (Input.GetButton(_forward))
        {
            move.y += 1;
        }
        if (Input.GetButton(_backward))
        {
            move.y -= 1;
        }

        return move;
    }


}

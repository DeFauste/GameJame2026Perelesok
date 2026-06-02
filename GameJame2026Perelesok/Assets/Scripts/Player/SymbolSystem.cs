using System;
using System.Collections;
using UnityEngine;

public class SymbolSystem : MonoBehaviour
{
    public static event Action OnDefenceSuccess;
    public static event Action OnAttackSuccess;

    private enum Symbol
    {
        Up,
        Down,
        Left,
        Right,
        Space
    }

    [Header("Settings")] [SerializeField] private float _waitTime = 8f;
    [SerializeField] private float _showDuration = 1f;

    [Header("Sprites")] [SerializeField] private SpriteRenderer _symbolRenderer;
    [SerializeField] private Sprite _upSprite;
    [SerializeField] private Sprite _downSprite;
    [SerializeField] private Sprite _leftSprite;
    [SerializeField] private Sprite _rightSprite;
    [SerializeField] private Sprite _spaceSprite;

    [Header("Effects")] [SerializeField] private FlashEffect[] _flashEffects;
    
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _symbolAppearClip;

    private bool _waitingInput;
    private Symbol _currentSymbol;

    private int _defenceCounter;

    private void Start()
    {
        StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_waitTime);

            bool attackPhase = _defenceCounter >= 4;

            if (attackPhase)
            {
                yield return ShowSymbol(Symbol.Space);
                _defenceCounter = 0;
            }
            else
            {
                Symbol randomArrow = (Symbol)UnityEngine.Random.Range(0, 4);

                yield return ShowSymbol(randomArrow);

                _defenceCounter++;
            }
        }
    }

    private IEnumerator ShowSymbol(Symbol symbol)
    {
        _currentSymbol = symbol;
        _waitingInput = true;

        _symbolRenderer.sprite = GetSprite(symbol);
        _symbolRenderer.enabled = true;
        
        PlayAppearSound();

        if (_flashEffects != null)
        {
            foreach (var effect in _flashEffects)
            {
                effect.Play();
            }
        }

        float timer = 0f;

        while (timer < _showDuration && _waitingInput)
        {
            HandleInput();
            timer += Time.deltaTime;
            yield return null;
        }

        _waitingInput = false;
        _symbolRenderer.enabled = false;
    }

    private void HandleInput()
    {
        switch (_currentSymbol)
        {
            case Symbol.Up:
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    DefenceSuccess();
                break;

            case Symbol.Down:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    DefenceSuccess();
                break;

            case Symbol.Left:
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    DefenceSuccess();
                break;

            case Symbol.Right:
                if (Input.GetKeyDown(KeyCode.RightArrow))
                    DefenceSuccess();
                break;

            case Symbol.Space:
                if (Input.GetKeyDown(KeyCode.Space))
                    AttackSuccess();
                break;
        }
    }

    private void DefenceSuccess()
    {
        _waitingInput = false;
        _symbolRenderer.enabled = false;

        OnDefenceSuccess?.Invoke();
    }

    private void AttackSuccess()
    {
        _waitingInput = false;
        _symbolRenderer.enabled = false;

        OnAttackSuccess?.Invoke();
    }

    private Sprite GetSprite(Symbol symbol)
    {
        return symbol switch
        {
            Symbol.Up => _upSprite,
            Symbol.Down => _downSprite,
            Symbol.Left => _leftSprite,
            Symbol.Right => _rightSprite,
            Symbol.Space => _spaceSprite,
            _ => null
        };
    }
    
    private void PlayAppearSound()
    {
        if (_audioSource == null || _symbolAppearClip == null)
            return;

        _audioSource.PlayOneShot(_symbolAppearClip);
    }
}
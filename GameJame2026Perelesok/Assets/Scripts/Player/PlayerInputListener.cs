using System.Collections;
using UnityEngine;

public class PlayerInputListener : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    private void Awake()
    {
        SymbolSystem.OnDefenceSuccess += Defence;
        SymbolSystem.OnAttackSuccess += Attack;
    }

    private void OnDestroy()
    {
        SymbolSystem.OnDefenceSuccess -= Defence;
        SymbolSystem.OnAttackSuccess -= Attack;
    }

    private void Defence()
    {
        _renderer.color = Color.blue;
        StartCoroutine(Wait());
    }

    private void Attack()
    {
        _renderer.color = Color.red;
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        _renderer.color = Color.white;
    }
}

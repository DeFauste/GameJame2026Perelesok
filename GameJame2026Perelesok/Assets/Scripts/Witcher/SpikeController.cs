using UnityEngine;

public class SpikeController : MonoBehaviour
{
    [SerializeField] private GameObject _small_spike;
    [SerializeField] private GameObject _big_spike;
    [SerializeField] private GameObject _laser;
    [SerializeField] private Movement _calculateElipce;

    private void Awake()
    {
        if (_calculateElipce == null)
            _calculateElipce = FindAnyObjectByType<Movement>();

        if (_small_spike == null)
            Debug.LogError("no smal spike prefab");
        if (_big_spike == null)
            Debug.LogError("no big spike prefab");
        if (_laser == null)
            Debug.LogError("no laser prefab");
    }

    public void RandomSpawnDamagers(int smalSpike, int bigSpike, int laser, float durationMultypyer)
    {
        for (int i = 0; i < smalSpike; i++)
        {
            SpawnSmallSpike(GetRandomPosition(), _small_spike, durationMultypyer);
        }

        for (int i = 0; i < bigSpike; i++)
        {
            SpawnSmallSpike(GetRandomPosition(), _big_spike, durationMultypyer);
        }

        for (int i = 0; i < laser; i++)
        {
            SpawnSmallSpike(GetRandomPosition(), _laser, durationMultypyer);
        }
    }

    private void SpawnSmallSpike(Vector2 position, GameObject damager, float durationMultypyer)
    {
        GameObject spike = Instantiate(damager);

        spike.transform.position = new Vector3(position.x, position.y, position.y);
        StartCoroutine(spike.GetComponent<SpikeCast>().CastCorutine(durationMultypyer));
    }

    private Vector2 GetRandomPosition()
    {
        float rand_Y = Random.value * (_calculateElipce.GetScales().y * 2) - _calculateElipce.GetScales().y;
        float rand_X = Random.value * (_calculateElipce.CalculateElipce_X(rand_Y) * 2) - _calculateElipce.CalculateElipce_X(rand_Y);
        return new Vector2(rand_X, rand_Y);
    }
}

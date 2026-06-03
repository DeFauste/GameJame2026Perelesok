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

    private void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            RandomSpawnSmallPeaks(2, 1);
            RandomSpawnBigPeaks(1, 1);
            RandomSpawnLaser(1, 1, new Vector2(5, 0));
        }
    }

    public void RandomSpawnSmallPeaks(int smallSpikeAmount, float smallSpikeDuration)
    {
        for (int i = 0; i < smallSpikeAmount; i++)
        {
            SpawnSpike(GetRandomPosition(), _small_spike, smallSpikeDuration);
        }
    }

    public void RandomSpawnBigPeaks(int bigSpikeAmount, int bigSpikeDuration)
    {
        for (int i = 0; i < bigSpikeAmount; i++)
        {
            SpawnSpike(GetRandomPosition(), _big_spike, bigSpikeDuration);
        }
    }

    public void RandomSpawnLaser(int laserAmount, float laserDuration, Vector2 eyePosition)
    {
        for (int i = 0; i < laserAmount; i++)
        {
            Laser laser = SpawnSpike(GetRandomPosition(), _laser, laserDuration).GetComponent<Laser>();
            laser.Pose(eyePosition);
        }
    }

    private GameObject SpawnSpike(Vector2 position, GameObject damager, float durationMultypyer)
    {
        GameObject spike = Instantiate(damager);

        spike.transform.position = new Vector3(position.x, position.y, position.y);
        StartCoroutine(spike.GetComponent<SpikeCast>().CastCorutine(durationMultypyer));

        return spike;
    }

    private Vector2 GetRandomPosition()
    {
        float rand_Y = (Random.value * (_calculateElipce.GetScales().y * 2)) - _calculateElipce.GetScales().y;
        float rand_X = (Random.value * (_calculateElipce.CalculateElipce_X(rand_Y) * 2)) - _calculateElipce.CalculateElipce_X(rand_Y);
        return new Vector2(rand_X, rand_Y);
    }
}

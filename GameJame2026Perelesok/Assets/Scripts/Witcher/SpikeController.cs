using System;
using UnityEngine;

public class SpikeController : MonoBehaviour
{
    [SerializeField] private GameObject _small_spike;
    [SerializeField] private GameObject _big_spike;
    [SerializeField] private GameObject _laser;
    [SerializeField] private Movement _calculateElipce;

    private float _enemysDistribution = 0.5f;

    public static Action ActionHitPlayer;
    public float EnemysDistribution { get { return _enemysDistribution; } set { _enemysDistribution = value; } }

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

        SpikeCast.IsHitAction += HitDetection;
    }

    private void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            RandomSpawnSmallPeaks(1, 1);
            //RandomSpawnBigPeaks(1, 1);
            //RandomSpawnLaser(1, 1, new Vector2(0.0006081462f, 7.933138f));
        }
    }

    public void RandomSpawnSmallPeaks(int smallSpikeAmount, float smallSpikeDuration)
    {
        for (int i = 0; i < smallSpikeAmount; i++)
        {
            SpawnSpike(GetRandomPosition(), _small_spike, smallSpikeDuration);
        }
    }

    public void RandomSpawnBigPeaks(int bigSpikeAmount, float bigSpikeDuration)
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

    public void HitDetection()
    {
        ActionHitPlayer?.Invoke();
    }

    private GameObject SpawnSpike(Vector2 position, GameObject damager, float durationMultypyer)
    {
        GameObject spike = Instantiate(damager);

        spike.transform.position = new Vector3(position.x, position.y, position.y);
        StartCoroutine(spike.GetComponent<SpikeCast>().CastCorutine(durationMultypyer));

        return spike;
    }

    //private Vector2 GetRandomPosition()
    //{
    //    float halfRange = _calculateElipce.GetScales().x;
    //    float rand_X = (UnityEngine.Random.value * halfRange * 2) - halfRange;
    //    halfRange = _calculateElipce.CalculateElipce_Y(rand_X);
    //    float rand_Y = (UnityEngine.Random.value * halfRange * 2) - halfRange;
    //    Debug.Log(halfRange * 2 + " | " + rand_Y);
    //    return new Vector2(rand_X, rand_Y);
    //}

    private Vector2 GetRandomPosition()
    {
        Vector2 _spikePosition = (Vector2)_calculateElipce.CurrentPosition;
        //Debug.Log(_spikePosition);
        //_spikePosition.x += UnityEngine.Random.value * _enemysDistribution * 2 - _enemysDistribution;
        //_spikePosition.y += UnityEngine.Random.value * _enemysDistribution * 2 - _enemysDistribution;
        float distace = _calculateElipce.ElipceFormula(_spikePosition) - 1;
        //Debug.Log(distace + " " + _spikePosition);
        if (distace > 0)
        {
            _spikePosition += ((Vector2)_calculateElipce.Zero - _spikePosition + new Vector2(0, _enemysDistribution)) * distace;
            Debug.Log("Spike out of bunds");
        }
        return _spikePosition;
    }
}

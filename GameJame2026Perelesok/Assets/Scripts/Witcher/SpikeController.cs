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
            SpawnSpike(GetRandomPosition(.5f), _small_spike, smallSpikeDuration);
        }
    }

    public void RandomSpawnBigPeaks(int bigSpikeAmount, float bigSpikeDuration)
    {
        for (int i = 0; i < bigSpikeAmount; i++)
        {
            SpawnSpike(GetRandomPosition(1), _big_spike, bigSpikeDuration);
        }
    }

    public void RandomSpawnLaser(int laserAmount, float laserDuration, Vector2 eyePosition)
    {
        for (int i = 0; i < laserAmount; i++)
        {
            Laser laser = SpawnSpike(GetRandomPosition(.7f), _laser, laserDuration).GetComponent<Laser>();
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

    private Vector2 GetRandomPosition(float enemysDistribution)
    {
        float x_pos = _calculateElipce.CurrentLocalPosition.x;
        float y_pos = _calculateElipce.CurrentLocalPosition.y;
        Vector2 spikePosition =  new Vector2(x_pos, y_pos);
        spikePosition.x += UnityEngine.Random.value * enemysDistribution * 2 - enemysDistribution;
        spikePosition.y += UnityEngine.Random.value * enemysDistribution * 2 - enemysDistribution;
        float distace = _calculateElipce.ElipceFormula(spikePosition - new Vector2(0, _calculateElipce.GetScales().y));
        //Debug.Log(distace + " " + (1 / distace));
        //Debug.DrawLine(_calculateElipce.Zero, spikePosition, Color.azure, 3);
        if (distace > 1)
        {
            spikePosition *= (1 / distace);
            //Debug.Log("Spike out of bunds");
        }
        //Debug.DrawLine(_calculateElipce.Zero, spikePosition, Color.yellowNice, 3);
        return spikePosition;
    }
}

using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SpikeController : MonoBehaviour
{
    [SerializeField] private GameObject _small_spike;
    [SerializeField] private GameObject _big_spike;
    [SerializeField] private GameObject _laser;
    [SerializeField] private Movement _calculateElipce;

    public static Action ActionHitPlayer;

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
            RandomSpawnSmallPeaks(2, 1);
            RandomSpawnBigPeaks(1, 1);
            RandomSpawnLaser(1, 1, new Vector2(0.0006081462f, 7.933138f));
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

    public void HitDetection()
    {
        ActionHitPlayer?.Invoke();
    }

    private GameObject SpawnSpike(Vector2 position, GameObject damager, float durationMultypyer)
    {
        GameObject spike = Instantiate(damager);

        spike.transform.position = new Vector3(position.x, position.y, position.y) + _calculateElipce.Zero;
        StartCoroutine(spike.GetComponent<SpikeCast>().CastCorutine(durationMultypyer));

        return spike;
    }

    private Vector2 GetRandomPosition()
    {
        float halfRange = _calculateElipce.GetScales().x;
        float rand_X = (UnityEngine.Random.value * halfRange * 2) - halfRange;
        halfRange = _calculateElipce.CalculateElipce_Y(rand_X);
        float rand_Y = (UnityEngine.Random.value * halfRange * 2) - halfRange;
        Debug.Log(halfRange * 2 + " | " + rand_Y);
        return new Vector2(rand_X, rand_Y);
    }

    //private Vector2 GetRandomPosition()
    //{
    //    Vector2 playerPosition = (Vector2)_calculateElipce.CurrentPosition;
    //    float halfRange = 0.5f;
    //    playerPosition.x += UnityEngine.Random.value * halfRange * 2 - halfRange;
    //    playerPosition.y += UnityEngine.Random.value * halfRange - halfRange;
    //    float distace = _calculateElipce.ElipceFormula(playerPosition) - 1;
    //    if (distace > 0)
    //    {
    //        playerPosition += ((Vector2)_calculateElipce.Zero - playerPosition + new Vector2(0, halfRange)) * distace;
    //    }
    //    return playerPosition;
    //}
}

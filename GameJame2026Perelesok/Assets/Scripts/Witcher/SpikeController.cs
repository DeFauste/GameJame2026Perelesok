using UnityEngine;

public class SpikeController : MonoBehaviour
{
    [SerializeField] private GameObject _small_spike;
    [SerializeField] private GameObject _big_spike;
    [SerializeField] private GameObject _laser;

    public void RandomSpawnDamagers(int smalSpike, int bigSpike, int laser)
    {

    }

    private void SpawnSmallSpike(Vector2 position)
    {
        GameObject spike = Instantiate(_small_spike);

        spike.transform.position = new Vector3(position.x, position.y, position.y);
        StartCoroutine(spike.GetComponent<SpikeCast>().CastCorutine());
    }
}

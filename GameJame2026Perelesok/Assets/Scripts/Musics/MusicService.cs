using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MusicService: singleton сервис для загрузки и проигрывания аудио из Resources/Musics
/// - Загружает все AudioClip из Resources/<resourcePath>
/// - Поддерживает проигрывание по имени (case-insensitive) с опцией loop
/// - Поддерживает одновременное проигрывание нескольких файлов (пул AudioSource)
/// - Может динамически расширять пул до maxPoolSize
/// </summary>
public class MusicService : MonoBehaviour
{
    public static MusicService Instance { get; private set; }

    [Tooltip("Путь внутри Resources, где лежат аудиофайлы. По умолчанию 'Musics' -> Assets/Resources/Musics")]
    [SerializeField] private string resourcePathMusics = "Musics";
    [SerializeField] private string resourcePathSound = "Sounds";
    [SerializeField] private string resourcePathUI = "UI";

    [Tooltip("Начальный размер пула AudioSource-ов")]
    [SerializeField] private int initialPoolSize = 8;

    [Tooltip("Максимально допустимый размер пула (при 0 — без ограничения)")]
    [SerializeField] private int maxPoolSize = 32;

    [Tooltip("Делать ли объект сервисом в единственном экземпляре и не уничтожать при загрузке сцены")]
    [SerializeField] private bool dontDestroyOnLoad = true;

    // Словарь загруженных клипов: имя -> AudioClip (comparer игнорирует регистр)
    private Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>(System.StringComparer.OrdinalIgnoreCase);

    // Пул объектов
    private List<PooledAudio> pool = new List<PooledAudio>();
    private Transform poolRoot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            poolRoot = new GameObject("[MusicService_Pool]").transform;
            poolRoot.SetParent(transform, false);
            clips.Clear();
            LoadAllClips(resourcePathMusics);
            LoadAllClips(resourcePathSound);
            LoadAllClips(resourcePathUI);
            CreatePool(initialPoolSize);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Loading
    /// <summary>
    /// Загружает все AudioClip из Resources/resourcePath
    /// </summary>
    public void LoadAllClips(string resourcePath)
    {
        AudioClip[] loaded = Resources.LoadAll<AudioClip>(resourcePath);
        if (loaded == null || loaded.Length == 0)
        {
            Debug.LogWarning($"MusicService: не найдено AudioClip-ов в Resources/{resourcePath}");
            return;
        }

        foreach (var clip in loaded)
        {
            if (clip == null) continue;
            clips[clip.name] = clip; // перезапишет при совпадении
        }

        Debug.Log($"MusicService: загружено {clips.Count} клипов из Resources/{resourcePath}");
    }

    /// <summary>
    /// Пополнить/создать пул аудиосорсов
    /// </summary>
    private void CreatePool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            pool.Add(CreatePooledAudio());
        }
    }

    private PooledAudio CreatePooledAudio()
    {
        GameObject go = new GameObject("PooledAudio");
        go.transform.SetParent(poolRoot, false);
        AudioSource src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 0f; // 2D по умолчанию, можно настроить
        return new PooledAudio { source = src, playingClipName = null, isLoop = false, parent = go };
    }
    #endregion

    #region Pool management
    private PooledAudio GetFreeSource(bool allowExpand = true)
    {
        // Найдём свободный: не воспроизводится и не в режиме loop
        foreach (var p in pool)
        {
            if (!p.isLoop && !p.source.isPlaying)
                return p;
        }

        // Если все заняты, можем расширить пул (если разрешено)
        if (allowExpand && (maxPoolSize <= 0 || pool.Count < maxPoolSize))
        {
            var newP = CreatePooledAudio();
            pool.Add(newP);
            return newP;
        }

        // Если нельзя расширять, попробуем принудительно взять первый не-loop (даже если он играет — остановим)
        foreach (var p in pool)
        {
            if (!p.isLoop)
            {
                p.source.Stop();
                p.source.clip = null;
                p.playingClipName = null;
                p.isLoop = false;
                return p;
            }
        }

        return null;
    }
    #endregion

    #region Public API
    /// <summary>
    /// Проиграть клип по имени. Имя регистронезависимо.
    /// Если loop == true, вернёт AudioSource который будет проигрывать непрерывно (можно затем остановить методом StopByName/StopAll).
    /// Если loop == false, проигрывание будет one-shot (несколько одинаковых one-shot могут играть одновременно).
    /// Возвращает AudioSource используемый для проигрывания (или null если не найден клип или нет доступных источников).
    /// </summary>
    public AudioSource Play(string clipName, bool loop = false, float volume = 1f, float pitch = 1f)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            Debug.LogWarning("MusicService.Play: пустое имя клипа");
            return null;
        }

        if (!clips.TryGetValue(clipName, out var clip))
        {
            Debug.LogWarning($"MusicService.Play: клип с именем '{clipName}' не найден в Resources/.");
            return null;
        }

        var pooled = GetFreeSource(allowExpand: true);
        if (pooled == null)
        {
            Debug.LogWarning("MusicService.Play: нет доступных AudioSource в пуле");
            return null;
        }

        pooled.source.pitch = pitch;
        pooled.source.volume = volume;

        if (loop)
        {
            pooled.source.clip = clip;
            pooled.source.loop = true;
            pooled.isLoop = true;
            pooled.playingClipName = clip.name;
            pooled.source.Play();
            return pooled.source;
        }
        else
        {
            // для one-shot назначаем имя и ставим корутину, чтобы очистить через длину клипа
            pooled.source.loop = false;
            pooled.playingClipName = clip.name;
            pooled.isLoop = false;
            pooled.source.PlayOneShot(clip, volume);

            // очистим запись после окончания воспроизведения (учитываем pitch)
            StartCoroutine(ClearAfterPlay(pooled, clip.length / Mathf.Max(0.01f, Mathf.Abs(pooled.source.pitch))));
            return pooled.source;
        }
    }

    /// <summary>
    /// Остановить все looping источники с указанным именем
    /// (one-shot остановить невозможно, они короткие; можно StopAll чтобы остановить все).
    /// </summary>
    public void StopByName(string clipName)
    {
        if (string.IsNullOrEmpty(clipName)) return;

        foreach (var p in pool)
        {
            if (p.playingClipName != null && string.Equals(p.playingClipName, clipName, System.StringComparison.OrdinalIgnoreCase))
            {
                p.source.Stop();
                p.source.clip = null;
                p.isLoop = false;
                p.playingClipName = null;
            }
        }
    }

    /// <summary>
    /// Остановить всё воспроизведение (перезапишет loop/one-shot)
    /// </summary>
    public void StopAll()
    {
        foreach (var p in pool)
        {
            p.source.Stop();
            p.source.clip = null;
            p.isLoop = false;
            p.playingClipName = null;
        }
    }

    /// <summary>
    /// Проверить, проигрывается ли клип с данным именем
    /// </summary>
    public bool IsPlaying(string clipName)
    {
        if (string.IsNullOrEmpty(clipName)) return false;
        foreach (var p in pool)
        {
            if (p.playingClipName != null && string.Equals(p.playingClipName, clipName, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Принудительно перезагрузить клипы из Resources (например после добавления новых в папку)
    /// </summary>
    public void ReloadClips()
    {
        LoadAllClips(resourcePathMusics);
        LoadAllClips(resourcePathSound);
        LoadAllClips(resourcePathUI);
    }
    #endregion

    #region Helpers
    private IEnumerator ClearAfterPlay(PooledAudio p, float delay)
    {
        if (delay <= 0f) delay = 0.01f;
        yield return new WaitForSeconds(delay);
        // убедимся, что источник не в loop режиме (кто-то мог переключить)
        if (!p.source.loop)
        {
            p.playingClipName = null;
            p.source.clip = null;
            p.isLoop = false;
        }
    }

    // Вспомогательный класс для описания объекта пула
    private class PooledAudio
    {
        public AudioSource source;
        public string playingClipName;
        public bool isLoop;
        public GameObject parent;
    }
    #endregion
}

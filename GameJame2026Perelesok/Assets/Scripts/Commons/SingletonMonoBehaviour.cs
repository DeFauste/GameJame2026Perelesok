using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Базовый Singleton для MonoBehaviour.
/// </summary>
/// <typeparam name="T">Тип класса-одиночки (должен наследовать MonoBehaviour).</typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    /// <summary>
    /// Глобальный доступ к единственному экземпляру.
    /// Если экземпляр отсутствует, он будет создан автоматически.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Пытаемся найти существующий объект в сцене
                _instance = FindObjectOfType<T>();

                // Если не найден – создаём новый GameObject с нужным компонентом
                if (_instance == null)
                {
                    GameObject singletonGO = new GameObject(typeof(T).Name + " (Singleton)");
                    _instance = singletonGO.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// При активации проверяем, нет ли уже другого экземпляра.
    /// При необходимости делаем объект неразрушаемым между сценами.
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // уничтожаем дубликат
        }
    }
}
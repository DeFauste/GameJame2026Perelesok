using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Класс для спавна объектов из префаба на основе переданного Transform
    /// </summary>
    public class SpawnShine : MonoBehaviour
    {
        public Transform spawnPoint;

        [Header("Prefab Settings")] [SerializeField]
        private GameObject _prefabMiniShine;

        [SerializeField] private GameObject _prefabBigShine;

        [Header("Spawn Settings")] [SerializeField]
        private bool _useWorldSpace = true;

        [SerializeField] private bool _matchRotation = true;
        [SerializeField] private bool _matchScale = false;


        private void Start()
        {
            spawnPoint = GameObject.Find("PointSpawenShine").GetComponent<Transform>();
            SymbolSystem.OnDefenceSuccess += () => Spawn(true, spawnPoint);
            SymbolSystem.OnAttackSuccess += () => Spawn(false, spawnPoint);
        }

        private void OnValidate()
        {
            if (_prefabMiniShine == null)
            {
                Debug.LogWarning("[SpawnShine] Префаб не установлен!", gameObject);
            }
        }

        /// <summary>
        /// Спавнить объект из префаба по переданному Transform
        /// </summary>
        /// <param name="targetTransform">Transform с позицией и ротацией для спавна</param>
        /// <returns>Созданный объект, или null если префаб не установлен</returns>
        public GameObject Spawn(bool isMini, Transform targetTransform)
        {
            if (targetTransform == null)
            {
                spawnPoint = GameObject.Find("PointSpawenShine").GetComponent<Transform>();
            }

            return SpawnAt(isMini, targetTransform.position, targetTransform.rotation, targetTransform.lossyScale);
        }

        /// <summary>
        /// Спавнить объект из префаба в указанной позиции
        /// </summary>
        /// <param name="position">Позиция спавна</param>
        /// <returns>Созданный объект, или null если префаб не установлен</returns>
        public GameObject SpawnAt(bool isMini, Vector3 position)
        {
            return SpawnAt(isMini, position, Quaternion.identity, Vector3.one);
        }

        /// <summary>
        /// Спавнить объект из префаба в указанной позиции и ротации
        /// </summary>
        /// <param name="position">Позиция спавна</param>
        /// <param name="rotation">Ротация спавна</param>
        /// <returns>Созданный объект, или null если префаб не установлен</returns>
        public GameObject SpawnAt(bool isMini, Vector3 position, Quaternion rotation)
        {
            return SpawnAt(isMini, position, rotation, Vector3.one);
        }

        /// <summary>
        /// Спавнить объект из префаба со всеми параметрами трансформа
        /// </summary>
        /// <param name="position">Позиция спавна</param>
        /// <param name="rotation">Ротация спавна</param>
        /// <param name="scale">Масштаб спавна</param>
        /// <returns>Созданный объект, или null если префаб не установлен</returns>
        public GameObject SpawnAt(bool isMini, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (_prefabMiniShine == null)
            {
                Debug.LogError("[SpawnShine] Префаб не установлен! Невозможно спавнить объект.", gameObject);
                return null;
            }

            GameObject spawnedObject = Instantiate(isMini ? _prefabMiniShine : _prefabBigShine, position, rotation);

            if (_matchScale)
            {
                spawnedObject.transform.localScale = scale;
            }

            Debug.Log($"[SpawnShine] Объект '{spawnedObject.name}' успешно спавнен в позиции {position}",
                spawnedObject);
            return spawnedObject;
        }
    }
}
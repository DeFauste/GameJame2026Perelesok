using UnityEngine;

namespace Player
{
    /// <summary>
    /// Класс для управления движением игрока в 2D с ограничением эллиптической зоной.
    /// Игрок двигается по WASD, не может покидать зону эллипса, размер которой можно изменять.
    /// </summary>
    [AddComponentMenu("Player/PlayerMove2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMove2D : MonoBehaviour
    {
        [Header("Движение")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private bool normalizeDiagonal = true;

        [Header("Эллиптическая зона")]
        [SerializeField] private Vector3 zoneCenter = Vector3.zero;
        [SerializeField] private float zoneWidth = 2f;
        [SerializeField] private float zoneHeight = 1f;

        [Header("Отладка")]
        [SerializeField] private bool drawZoneGizmos = true;

        private Rigidbody2D _rb;
        private Vector2 _inputDir;
        private ContactFilter2D _contactFilter;
        private RaycastHit2D[] _castHits = new RaycastHit2D[8];

        [SerializeField] private float skinWidth = 0.02f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb != null)
            {
                // Отключаем гравитацию и фиксируем вращение
                _rb.gravityScale = 0f;
                _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            // Инициализация фильтра для кастов: не учитывать триггеры
            _contactFilter = new ContactFilter2D();
            _contactFilter.useTriggers = false;
            _contactFilter.useLayerMask = false;
        }

        private void Update()
        {
            // Читаем ввод каждый кадр и сохраняем направление для FixedUpdate
            _inputDir = ReadInputWasd();

            if (normalizeDiagonal && _inputDir.sqrMagnitude > 1f)
                _inputDir = _inputDir.normalized;
        }

        private void FixedUpdate()
        {
            if (_rb == null)
                return;

            if (_inputDir.sqrMagnitude < 0.0001f)
                return;

            float moveAmount = speed * Time.fixedDeltaTime;
            Vector2 movement = _inputDir * moveAmount;
            Vector2 nextPosition = _rb.position + movement;

            // Проверяем, остаётся ли новая позиция внутри эллипса
            if (IsPointInsideEllipse(nextPosition))
            {
                // Проверяем столкновения с коллайдерами
                ApplyMovementWithCollisionDetection(_rb.position, movement, nextPosition);
            }
            else
            {
                // Попытка выхода за границы зоны — применяем скольжение вдоль краёв
                ApplyMovementWithSliding(_rb.position, movement);
            }
        }

        /// <summary>
        /// Применяет движение с проверкой столкновений с коллайдерами.
        /// </summary>
        private void ApplyMovementWithCollisionDetection(Vector2 currentPos, Vector2 movement, Vector2 nextPosition)
        {
            float distance = movement.magnitude;
            if (distance > 0f)
            {
                Vector2 dir = movement / distance;
                int hitCount = _rb.Cast(dir, _contactFilter, _castHits, distance + skinWidth);

                if (hitCount > 0)
                {
                    // Находим ближайшее препятствие
                    float minDist = float.MaxValue;
                    for (int i = 0; i < hitCount; i++)
                    {
                        var hit = _castHits[i];
                        if (hit.collider == null) continue;
                        if (hit.distance < minDist) minDist = hit.distance;
                    }

                    // Вычисляем максимально возможное смещение (без пересечения)
                    float allowed = Mathf.Max(0f, minDist - skinWidth);
                    Vector2 allowedMove = dir * Mathf.Min(allowed, distance);
                    if (allowedMove.sqrMagnitude > 0f)
                    {
                        _rb.MovePosition(currentPos + allowedMove);
                    }
                }
                else
                {
                    // Свободный путь — двигаемся полностью
                    _rb.MovePosition(nextPosition);
                }
            }
        }

        /// <summary>
        /// Применяет скольжение вдоль границы эллипса.
        /// Разложает движение на компоненты X и Y, позволяя скольжение по краю.
        /// </summary>
        private void ApplyMovementWithSliding(Vector2 currentPos, Vector2 movement)
        {
            Vector2 resultMove = Vector2.zero;

            // Пытаемся двигаться полностью
            Vector2 fullMove = currentPos + movement;
            if (IsPointInsideEllipse(fullMove))
            {
                ApplyMovementWithCollisionDetection(currentPos, movement, fullMove);
                return;
            }

            // Если полное движение выходит за границу, пытаемся разложить его на компоненты
            // Сначала пытаемся X компоненту
            if (movement.x != 0f)
            {
                Vector2 moveX = new Vector2(movement.x, 0f);
                Vector2 testPosX = currentPos + moveX;
                if (IsPointInsideEllipse(testPosX))
                {
                    resultMove.x = moveX.x;
                }
                else
                {
                    // X выходит за границу, пытаемся скользить вдоль Y
                    float slideX = FindMaxSlideAmount(currentPos, Vector2.right, movement.x);
                    resultMove.x = slideX;
                }
            }

            // Теперь пытаемся Y компоненту
            if (movement.y != 0f)
            {
                Vector2 moveY = new Vector2(0f, movement.y);
                Vector2 testPosY = currentPos + resultMove + moveY;
                if (IsPointInsideEllipse(testPosY))
                {
                    resultMove.y = moveY.y;
                }
                else
                {
                    // Y выходит за границу, пытаемся скользить вдоль X
                    float slideY = FindMaxSlideAmount(currentPos + Vector2.right * resultMove.x, Vector2.up, movement.y);
                    resultMove.y = slideY;
                }
            }

            // Применяем оставшееся движение с проверкой столкновений
            if (resultMove.sqrMagnitude > 0.0001f)
            {
                Vector2 newPos = currentPos + resultMove;
                ApplyMovementWithCollisionDetection(currentPos, resultMove, newPos);
            }
        }

        /// <summary>
        /// Находит максимальное расстояние скольжения вдоль заданного направления,
        /// чтобы остаться внутри эллипса.
        /// </summary>
        private float FindMaxSlideAmount(Vector2 startPos, Vector2 direction, float desiredAmount)
        {
            float low = 0f;
            float high = desiredAmount;
            float result = 0f;

            // Бинарный поиск максимального допустимого смещения
            for (int i = 0; i < 8; i++)
            {
                float mid = (low + high) * 0.5f;
                Vector2 testPos = startPos + direction * mid;
                
                if (IsPointInsideEllipse(testPos))
                {
                    result = mid;
                    low = mid;
                }
                else
                {
                    high = mid;
                }
            }

            return desiredAmount > 0f ? result : -result;
        }

        /// <summary>
        /// Проверяет, находится ли точка внутри эллиптической зоны.
        /// Формула эллипса: (x-cx)²/a² + (y-cy)²/b² ≤ 1
        /// </summary>
        private bool IsPointInsideEllipse(Vector2 point)
        {
            float dx = point.x - zoneCenter.x;
            float dy = point.y - zoneCenter.y;
            float a = zoneWidth * 0.5f;
            float b = zoneHeight * 0.5f;

            if (a <= 0f || b <= 0f) return true; // Защита от деления на 0

            float ellipseValue = (dx * dx) / (a * a) + (dy * dy) / (b * b);
            return ellipseValue <= 1f;
        }


        /// <summary>
        /// Изменяет размер эллиптической зоны перемещения.
        /// </summary>
        public void SetZoneSize(float width, float height)
        {
            zoneWidth = Mathf.Max(0.1f, width);
            zoneHeight = Mathf.Max(0.1f, height);
        }

        /// <summary>
        /// Устанавливает центр эллиптической зоны.
        /// </summary>
        public void SetZoneCenter(Vector3 center)
        {
            zoneCenter = center;
        }

        /// <summary>
        /// Масштабирует текущий размер зоны на множитель.
        /// </summary>
        public void ScaleZone(float multiplier)
        {
            multiplier = Mathf.Max(0.1f, multiplier);
            zoneWidth *= multiplier;
            zoneHeight *= multiplier;
        }

        /// <summary>
        /// Получает текущие параметры зоны.
        /// </summary>
        public (Vector3 center, float width, float height) GetZoneInfo()
        {
            return (zoneCenter, zoneWidth, zoneHeight);
        }

        /// <summary>
        /// Читает ввод WASD и возвращает направление движения.
        /// </summary>
        private Vector2 ReadInputWasd()
        {
            float x = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
            float y = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);
            return new Vector2(x, y);
        }

        private void OnDrawGizmos()
        {
            if (!drawZoneGizmos) return;

            // Рисуем эллипс как окружность, масштабированную по осям
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            float segmentCount = 32;
            float angleStep = 360f / segmentCount;

            for (int i = 0; i < segmentCount; i++)
            {
                float angle1 = i * angleStep * Mathf.Deg2Rad;
                float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

                float x1 = zoneCenter.x + (zoneWidth * 0.5f) * Mathf.Cos(angle1);
                float y1 = zoneCenter.y + (zoneHeight * 0.5f) * Mathf.Sin(angle1);
                Vector3 pos1 = new Vector3(x1, y1, zoneCenter.z);

                float x2 = zoneCenter.x + (zoneWidth * 0.5f) * Mathf.Cos(angle2);
                float y2 = zoneCenter.y + (zoneHeight * 0.5f) * Mathf.Sin(angle2);
                Vector3 pos2 = new Vector3(x2, y2, zoneCenter.z);

                Gizmos.DrawLine(pos1, pos2);
            }

            // Рисуем центр
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(zoneCenter, 0.15f);
        }
    }
}


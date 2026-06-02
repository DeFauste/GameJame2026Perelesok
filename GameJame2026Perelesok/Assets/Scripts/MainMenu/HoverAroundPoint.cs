using UnityEngine;

namespace MainMenu
{
    public class HoverAroundPoint : MonoBehaviour
    {
        public enum HoverType
        {
            Circular, // круговое/эллиптическое движение
            Vertical, // вертикальное покачивание (вверх-вниз)
            Horizontal // горизонтальное покачивание (влево-вправо)
        }

        public bool isActive = true; // включить/выключить эффект парения

        [Header("Target Point")]
        public Transform centerPoint; // точка, вокруг которой парим (если null, используем startPosition)

        public bool useStartPositionAsCenter = true; // если нет центра, использовать начальную позицию

        [Header("Movement Settings")] public HoverType hoverType = HoverType.Circular;
        public float radiusX = 1.0f; // радиус/амплитуда по X
        public float radiusY = 1.0f; // радиус/амплитуда по Y
        public float speed = 1.0f; // скорость движения

        [Header("Rotation (optional)")] public bool rotateObject = false; // вращать ли объект в направлении движения
        public float rotationSpeed = 360f; // скорость вращения (при Circular)
        public bool directionRotation = true; // Исходное значение
        public float timeSwapDirectionSeconds = 1f; // время через которое происходит смена направления вращения
            
        private Vector3 centerPos;
        private float angle = 0f;
        private Vector3 startPos;

        void Start()
        {
            // Определяем начальный центр парения (используется только для инициализации)
            if (centerPoint != null)
                centerPos = centerPoint.position;
            else if (useStartPositionAsCenter)
                centerPos = transform.position;
            else
                centerPos = Vector3.zero;

            startPos = transform.position;

            // Случайный начальный угол для разнообразия
            angle = Random.Range(0f, 360f);

            StartToggleTimer();
        }

        void Update()
        {
            if (!isActive)
            {
                return;
            }

            // Обновляем центр в каждом кадре, если centerPoint назначен и движется
            if (centerPoint != null)
            {
                centerPos = centerPoint.position;
            }

            // Прогрессия угла или времени
            float t = Time.time * speed;

            switch (hoverType)
            {
                case HoverType.Circular:
                    Vector3 newPos = centerPos;
                    newPos.x += Mathf.Cos(t) * radiusX;
                    newPos.y += Mathf.Sin(t) * radiusY;
                    transform.position = newPos;

                    if (rotateObject)
                    {
                        // Вращение вокруг своей оси (как по орбите)
                        var direction = directionRotation ? 1 : -1;
                        transform.Rotate(Vector3.forward, direction * rotationSpeed * Time.deltaTime);
                    }

                    break;

                case HoverType.Vertical:
                    Vector3 verticalPos = centerPos;
                    verticalPos.y += Mathf.Sin(t) * radiusY;
                    // Сохраняем исходный X (можно оставить без изменений или использовать радиусX как offset)
                    if (radiusX > 0)
                        verticalPos.x += Mathf.Sin(t * 1.3f) * radiusX; // небольшая модуляция
                    transform.position = verticalPos;
                    break;

                case HoverType.Horizontal:
                    Vector3 horizontalPos = centerPos;
                    horizontalPos.x += Mathf.Sin(t) * radiusX;
                    if (radiusY > 0)
                        horizontalPos.y += Mathf.Cos(t * 1.3f) * radiusY;
                    transform.position = horizontalPos;
                    break;
            }
        }

        // Опционально: отображаем траекторию в редакторе
        void OnDrawGizmosSelected()
        {
            if (centerPoint != null)
                Gizmos.color = Color.cyan;
            else if (useStartPositionAsCenter && Application.isPlaying == false)
                Gizmos.color = Color.gray;
            else
                Gizmos.color = Color.white;

            Vector3 drawCenter = (centerPoint != null)
                ? centerPoint.position
                : (useStartPositionAsCenter ? transform.position : Vector3.zero);
            Gizmos.DrawWireSphere(drawCenter, 0.2f);

            // Рисуем примерную траекторию
            Gizmos.color = Color.green;
            Vector3 prev = drawCenter + new Vector3(Mathf.Cos(0) * radiusX, Mathf.Sin(0) * radiusY, 0);
            for (int i = 1; i <= 100; i++)
            {
                float angle = i * Mathf.PI * 2f / 100f;
                Vector3 curr = drawCenter + new Vector3(Mathf.Cos(angle) * radiusX, Mathf.Sin(angle) * radiusY, 0);
                Gizmos.DrawLine(prev, curr);
                prev = curr;
            }
        }
        
        // Запуск таймера
        public void StartToggleTimer()
        {
            // Останавливаем предыдущий, чтобы не было дублей
            CancelInvoke(nameof(ToggleValue));
            InvokeRepeating(nameof(ToggleValue), timeSwapDirectionSeconds, timeSwapDirectionSeconds);
        }

        // Остановка таймера
        public void StopToggleTimer()
        {
            CancelInvoke(nameof(ToggleValue));
        }

        // Метод, который меняет bool на противоположный
        private void ToggleValue()
        {
            directionRotation = !directionRotation;
            Debug.Log($"isRight изменилось на: {directionRotation}");
        }
    }
}
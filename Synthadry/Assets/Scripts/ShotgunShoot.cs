using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [Header("Настройки дробовика")]
    public float damage = 25f;
    public float range = 50f;
    public float coneAngle = 20f; // Угол конуса в градусах
    public float fireRate = 1f; // Выстрелов в секунду
    public Transform barrelEnd; // Точка, из которой вылетает выстрел (конец дробовика)
    public Camera playerCamera; // Камера игрока (для прицеливания)
    public LayerMask targetLayers; // Слои, которые считаются врагами
    public int coneVertices = 8; // Число вершин для визуализации конуса
    public Vector3 positionOffset; // Смещение позиции дробовика от камеры
    public Vector3 rotationOffset; // Смещение вращения дробовика от камеры

    [Header("Эффекты")]
    public GameObject muzzleFlashPrefab;
    public GameObject hitEffectPrefab;
    public AudioClip shotSound;
    private AudioSource audioSource;

    private float nextTimeToFire = 0f;

    void Start()
    {
        if (barrelEnd == null)
        {
            Debug.LogError("Не назначена точка barrelEnd!");
            enabled = false;
            return;
        }

        if (playerCamera == null)
        {
            Debug.LogError("Не назначена камера игрока!");
            enabled = false;
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = shotSound; // Звук выстрела
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }


    void LateUpdate()
    {
        // Установка позиции и поворота дробовика в соответствии с камерой
        transform.position = playerCamera.transform.position + playerCamera.transform.rotation * positionOffset;
        transform.rotation = playerCamera.transform.rotation * Quaternion.Euler(rotationOffset);
    }

    void Shoot()
    {
        //Эффект выстрела
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, barrelEnd.position, barrelEnd.rotation);
        Destroy(muzzleFlash, 1f); // Удаление эффекта через небольшое время

        // Воспроизведение звука выстрела
        audioSource.PlayOneShot(shotSound);

        // Направление из камеры
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Центр экрана
        // Определение направления выстрела
        Vector3 shootDirection = ray.direction;

        // Обнаружение врагов в конусе
        Collider[] hitColliders = Physics.OverlapSphere(barrelEnd.position, range, targetLayers); // Поиск целей в радиусе

        foreach (Collider collider in hitColliders)
        {
            // Нахождение цели в конусе
            Vector3 targetDirection = (collider.transform.position - barrelEnd.position).normalized;
            float angleToTarget = Vector3.Angle(shootDirection, targetDirection);

            if (angleToTarget <= coneAngle / 2)
            {
                // Нанесение урона
                Debug.Log("Попали во врага: " + collider.name);
                // Есть ли у цели скрипт здоровья или другой способ получения урона
                MobController targetHealth = collider.GetComponent<MobController>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damage);

                    // Эффект попадания
                    GameObject hitEffect = Instantiate(hitEffectPrefab, collider.ClosestPoint(barrelEnd.position), Quaternion.identity);
                    Destroy(hitEffect, 1f);
                }
                else
                {
                    Debug.LogWarning("У цели " + collider.name + " нет скрипта Health или другого скрипта для получения урона!");
                }
            }
        }
    }


    // Визуализация конуса в редакторе
    void OnDrawGizmosSelected()
    {
        if (barrelEnd == null) return;

        Gizmos.color = Color.yellow;
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(barrelEnd.position, barrelEnd.rotation, Vector3.one);

        // Рисуем пирамиду
        float angleStep = 360f / coneVertices;
        float coneRadius = Mathf.Tan(coneAngle / 2 * Mathf.Deg2Rad) * range;

        Vector3 apex = Vector3.zero; // Вершина пирамиды (совпадает с barrelEnd)

        Vector3[] baseVertices = new Vector3[coneVertices];
        for (int i = 0; i < coneVertices; i++)
        {
            float angle = i * angleStep;
            baseVertices[i] = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * coneRadius, Mathf.Sin(angle * Mathf.Deg2Rad) * coneRadius, range);
        }

        // Рисуем грани пирамиды
        for (int i = 0; i < coneVertices; i++)
        {
            Gizmos.DrawLine(apex, baseVertices[i]);
            Gizmos.DrawLine(baseVertices[i], baseVertices[(i + 1) % coneVertices]); // Соединяем вершины основания
        }
        Gizmos.matrix = originalMatrix;
    }
}

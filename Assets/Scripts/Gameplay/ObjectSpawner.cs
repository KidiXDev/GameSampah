using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    public GameObject[] objectPrefabs; // Changed from Sprite[] to GameObject[] for prefabs
    [SerializeField]
    private float initialSpawnInterval = 6f;
    [SerializeField]
    private float minSpawnInterval = 1f;
    [SerializeField]
    private float spawnAcceleration = 0.1f; // How much faster per spawn
    [SerializeField]
    private static float moveGlobalCurrentSpeed;
    [SerializeField]
    private float initialSpeed = 2f;
    [SerializeField]
    private float speedIncreaseRate = 0.01f;
    [SerializeField]
    private float maxSpeed = 8f;

    private float spawnTimer;
    private float currentSpawnInterval;
    private float currentSpeed;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        spawnTimer = currentSpawnInterval;
        currentSpeed = initialSpeed;
        ObjectMover.CurrentSpeed = currentSpeed;
    }

    void Update()
    {
        // Timer buat spawn
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnObject();
            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - spawnAcceleration);
            spawnTimer = currentSpawnInterval;
        }

        // Speed increase terus tiap detik (ABSOLUTE)
        currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate * Time.deltaTime, maxSpeed);
        ObjectMover.CurrentSpeed = currentSpeed;

        // Debug.Log("Current Speed: " + currentSpeed);
    }


    private void SpawnObject()
    {
        // Use objectPrefabs instead of objectSampah
        if (objectPrefabs == null || objectPrefabs.Length == 0 || spawnPoint == null)
            return;

        int randomIndex = Random.Range(0, objectPrefabs.Length);
        GameObject prefabToSpawn = objectPrefabs[randomIndex];
        GameObject spawned = Instantiate(prefabToSpawn, spawnPoint.transform.position, Quaternion.identity);

        // Ensure the spawned object uses the current speed (redundant if ObjectMover uses static)
        // var mover = spawned.GetComponent<ObjectMover>();
        // if (mover != null) mover.SetSpeed(currentSpeed);
    }
}

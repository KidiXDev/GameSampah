using UnityEngine;
using TMPro;

public class CharacterReq : MonoBehaviour
{
    [SerializeField] private GameObject goldIncomeAnimGameObject;
    [SerializeField] private GameObject itemRequestedDialog;
    [SerializeField] private ObjectSpawner objSpawnerItem;
    [SerializeField] private Sprite angrySprite;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private SpriteRenderer charSpriteRenderer;
    [SerializeField] private GameObject itemReqPanel;
    [SerializeField] private TextMeshProUGUI goldIncomeText;

    private GameObject[] itemList;

    public float moveSpeed = 5f;
    public float requestTimeout = 20f;
    public float angryTime = 10f;

    private bool isMoving = false;
    private Vector3 targetPosition;
    private Vector3 startPosition;

    private int currentRequestedItemIndex;
    private bool hasActiveRequest = false;

    private float requestTimer;
    private bool isAngry = false;

    private bool isReturningToStart = false;
    private GameState gameState;
    public CharacterHandler contoh;

    void Awake()
    {
        goldIncomeAnimGameObject.SetActive(false);
        gameState = FindFirstObjectByType<GameState>();
        contoh = GameObject.Find("GameManager").GetComponent<CharacterHandler>();
    }

    void Start()
    {
        itemList = objSpawnerItem.objectPrefabs;
        startPosition = transform.position;

        if (itemRequestedDialog == null)
        {
            Debug.LogError("Item Requested Dialog is not assigned in the inspector.");
            return;
        }

        if (GetComponent<BoxCollider2D>() == null)
        {
            Debug.LogWarning("Adding BoxCollider2D to CharacterReq GameObject");
            gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
        }

        RequestRandomItem();
    }

    void Update()
    {

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            itemReqPanel.SetActive(false);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;

                if (isReturningToStart)
                {
                    isReturningToStart = false;
                    charSpriteRenderer.sprite = normalSprite;
                    RequestRandomItem();
                }
            }
        }
        else
        {
            itemReqPanel.SetActive(true);
        }

        if (hasActiveRequest)
        {
            requestTimer -= Time.deltaTime;

            if (!isAngry && requestTimer <= requestTimeout - angryTime)
            {
                BecomeAngry();
            }

            if (requestTimer <= 0f)
            {
                GoAwayBecauseAngry();
            }
        }
    }

    public void RequestRandomItem()
    {
        if (itemList == null || itemList.Length == 0)
        {
            Debug.LogError("No item prefabs available for requests.");
            return;
        }

        currentRequestedItemIndex = Random.Range(0, itemList.Length);
        GameObject requestedItemPrefab = itemList[currentRequestedItemIndex];

        SpriteRenderer requestedItemSprite = itemRequestedDialog.GetComponent<SpriteRenderer>();
        if (requestedItemSprite != null)
        {
            SpriteRenderer prefabRenderer = requestedItemPrefab.GetComponent<SpriteRenderer>();
            if (prefabRenderer != null)
            {
                requestedItemSprite.sprite = prefabRenderer.sprite;
                hasActiveRequest = true;
                requestTimer = requestTimeout;
                isAngry = false;
                charSpriteRenderer.sprite = normalSprite;
            }
            else
            {
                Debug.LogError("Requested item prefab doesn't have a SpriteRenderer component");
            }
        }
        else
        {
            Debug.LogError("Item Requested Dialog doesn't have a SpriteRenderer component");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasActiveRequest)
            return;

        GameObject collidedObject = collision.gameObject;
        GameObject requestedPrefab = itemList[currentRequestedItemIndex];

        if (collidedObject.name.Contains(requestedPrefab.name) ||
            requestedPrefab.name.Contains(collidedObject.name))
        {
            Debug.Log("Correct item delivered: " + requestedPrefab.name);

            if (isAngry)
            {
                int goldIncome = Random.Range(30, 120);
                goldIncomeText.text = "+" + goldIncome.ToString();
                gameState.gold += goldIncome;
            }
            else
            {
                int goldIncome = 200;
                goldIncomeText.text = "+" + 200.ToString();
                gameState.gold += goldIncome;
            }

            MoveAndReturn();

            goldIncomeAnimGameObject.SetActive(true);
            hasActiveRequest = false;
            Destroy(collidedObject);

            Invoke("HideGoldAnimation", 1.0f);
        }
    }

    private void HideGoldAnimation()
    {
        goldIncomeAnimGameObject.SetActive(false);
    }

    private void BecomeAngry()
    {
        Debug.Log("Character is angry!");
        isAngry = true;

        if (charSpriteRenderer != null && angrySprite != null)
        {
            charSpriteRenderer.sprite = angrySprite;
        }
    }

    private void GoAwayBecauseAngry()
    {
        Debug.Log("Character is going away because angry!");

        hasActiveRequest = false;

        // Karakter kabur dulu ke samping random
        // float randomDirection = Random.value > 0.5f ? 20f : -20f;
        // targetPosition = transform.position + new Vector3(randomDirection, 0f, 0f);
        // isMoving = true;

        gameState.health -= 1;

        float randomDirection = Random.value > 0.5f ? 20f : -20f;
        targetPosition = transform.position + new Vector3(randomDirection, 0f, 0f);
        isMoving = true;

        float oppositeDirection = -randomDirection;

        // Setelah sampai, harus jalan balik ke startPosition
        Invoke(nameof(ReturnToStartPosition), 4.0f); // Kasih delay dikit biar keren

        _oppositeReturnDirection = oppositeDirection;
    }

    private void MoveAndReturn()
    {
        // Move away in a random direction, then return from the opposite direction
        float randomDirection = Random.value > 0.5f ? 20f : -20f;
        targetPosition = transform.position + new Vector3(randomDirection, 0f, 0f);
        isMoving = true;

        // Store the direction to use the opposite when returning
        float oppositeDirection = -randomDirection;

        // Use a lambda to pass the opposite direction to ReturnToStartPosition
        Invoke(nameof(ReturnToStartPosition), 2.0f);

        // Store the opposite direction for use in ReturnToStartPosition
        _oppositeReturnDirection = oppositeDirection;
    }

    // Add a private field to store the opposite direction
    private float _oppositeReturnDirection = 0f;

    private void ReturnToStartPosition()
    {
        Debug.Log("Character returning to start position...");

        if (charSpriteRenderer != null && angrySprite != null)
        {
            charSpriteRenderer.sprite = normalSprite;
        }

        // Start from the opposite direction before moving to startPosition
        transform.position = startPosition + new Vector3(_oppositeReturnDirection, 0f, 0f);
        targetPosition = startPosition;
        isMoving = true;
        isReturningToStart = true;
    }
}

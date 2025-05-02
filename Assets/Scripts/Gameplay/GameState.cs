using UnityEngine;
using TMPro;

public class GameState : MonoBehaviour
{
    public float gameTimeSeconds;
    public float gold;
    public int health = 3;
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI dayTime;
    public TextMeshProUGUI goldText;

    [SerializeField]
    private GameObject[] healthBar;
    private const float timeScale = 3600f / 15f; // 240

    void Start()
    {
        gameTimeSeconds = 8f * 3600f; // 8 AM in seconds
    }

    void Update()
    {
        gameTimeSeconds += Time.deltaTime * timeScale;

        goldText.text = $"{gold}";

        if (gameTimeSeconds >= 86400f)
            gameTimeSeconds -= 86400f;

        int hours = (int)(gameTimeSeconds / 3600) % 24;
        int minutes = (int)(gameTimeSeconds / 60) % 60;
        clockText.text = $"{hours:D2}:{minutes:D2}";

        // set day time text based on current hour
        if (hours >= 5 && hours < 12)
        {
            dayTime.text = "Morning";
        }
        else if (hours >= 12 && hours < 15)
        {
            dayTime.text = "Afternoon";
        }
        else if (hours >= 15 && hours < 18)
        {
            dayTime.text = "Evening";
        }
        else
        {
            dayTime.text = "Night";
        }

        if (health <= 0)
        {
            Time.timeScale = 0f;
        }

        for (int i = 0; i < healthBar.Length; i++)
        {
            if (i < health)
                healthBar[i].SetActive(true);
            else
                healthBar[i].SetActive(false);
        }
    }
}

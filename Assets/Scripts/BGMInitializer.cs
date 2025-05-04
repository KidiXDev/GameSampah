using UnityEngine;

public class BGMInitializer : MonoBehaviour
{
    // Referensi ke AudioClip untuk musik latar
    public AudioClip backgroundMusic;
    
    // Volume awal
    [Range(0f, 1f)]
    public float initialVolume = 0.5f;
    
    void Start()
    {
        // Cek apakah BGMManager sudah ada
        if (BGMManager.Instance == null)
        {
            // Jika belum ada, buat GameObject baru
            GameObject bgmObject = new GameObject("BGM Manager");
            
            // Tambahkan komponen BGMManager
            BGMManager bgmManager = bgmObject.AddComponent<BGMManager>();
            
            // Set propertinya
            bgmManager.backgroundMusic = backgroundMusic;
            bgmManager.musicVolume = initialVolume;
            
            Debug.Log("BGM Manager created and initialized");
        }
        else
        {
            // Jika sudah ada instance, kita bisa mengganti musik jika diperlukan
            if (backgroundMusic != null && BGMManager.Instance.backgroundMusic != backgroundMusic)
            {
                BGMManager.Instance.ChangeBackgroundMusic(backgroundMusic);
                Debug.Log("Changed background music in existing BGM Manager");
            }
        }
    }
}
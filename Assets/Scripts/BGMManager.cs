using UnityEngine;

public class BGMManager : MonoBehaviour
{
    // Singleton instance
    public static BGMManager Instance { get; private set; }
    
    // Audio Source component
    private AudioSource audioSource;
    
    // Audio clip untuk background music
    public AudioClip backgroundMusic;
    
    // Volume musik
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    
    // Flag untuk mengaktifkan/menonaktifkan loop
    public bool loopMusic = true;
    
    private void Awake()
    {
        // Penerapan Singleton Pattern
        // Jika instance sudah ada dan itu bukan game object ini
        if (Instance != null && Instance != this)
        {
            // Hancurkan object ini, hanya butuh satu instance
            Destroy(gameObject);
            return;
        }
        
        // Set instance ke object ini
        Instance = this;
        
        // Jangan hancurkan gameobject ini saat scene berubah
        DontDestroyOnLoad(gameObject);
        
        // Dapatkan atau tambahkan komponen AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Setup AudioSource
        SetupAudioSource();
    }
    
    private void SetupAudioSource()
    {
        // Set audio clip jika ada
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
        }
        
        // Set volume
        audioSource.volume = musicVolume;
        
        // Set untuk terus berputar (loop)
        audioSource.loop = loopMusic;
        
        // Set agar music tidak terpengaruh oleh perubahan waktu dalam game
        audioSource.ignoreListenerPause = true;
        
        // Mulai memutar music
        audioSource.Play();
    }
    
    // Method untuk mengubah volume musik
    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }
    
    // Method untuk mengganti background music
    public void ChangeBackgroundMusic(AudioClip newMusic, bool playImmediately = true)
    {
        if (audioSource != null && newMusic != null)
        {
            // Stop musik yang sedang diputar
            audioSource.Stop();
            
            // Ganti clip
            audioSource.clip = newMusic;
            backgroundMusic = newMusic;
            
            // Mulai memutar jika diperlukan
            if (playImmediately)
            {
                audioSource.Play();
            }
        }
    }
    
    // Method untuk pause/unpause musik
    public void TogglePause()
    {
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.UnPause();
            }
        }
    }
}
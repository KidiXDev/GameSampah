using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    // Referensi untuk tombol-tombol
    public Button playButton;
    public Button shopButton;
    public Button exitButton;
    
    // Warna saat normal dan saat hover
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Menambahkan listener untuk tombol play
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayGame);
            AddHoverEffect(playButton);
        }
        
        // Menambahkan listener untuk tombol shop
        if (shopButton != null)
        {
            shopButton.onClick.AddListener(OpenShop);
            AddHoverEffect(shopButton);
        }
        
        // Menambahkan listener untuk tombol exit
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
            AddHoverEffect(exitButton);
        }
    }
    
    // Fungsi untuk memulai game
    public void PlayGame()
    {
        // Pindah ke scene game (SampleScene)
        SceneManager.LoadScene("SampleScene");
    }
    
    // Fungsi untuk membuka shop
    public void OpenShop()
    {
        // Untuk sementara hanya menampilkan debug log
        Debug.Log("Shop button clicked - Fitur ini belum tersedia");
    }
    
    // Fungsi untuk keluar dari game
    public void ExitGame()
    {
        // Pindah ke scene Exit (jika ada)
        // Jika tidak ada scene Exit, bisa langsung menutup aplikasi
        // Coba load scene Exit
        // if (SceneExistsInBuildSettings("Exit"))
        {
            SceneManager.LoadScene("Exit");
        }
        // else
        // {
        //     // Jika tidak ada scene Exit, langsung keluar dari aplikasi
        //     Debug.Log("Exiting Application");
        //     #if UNITY_EDITOR
        //         UnityEditor.EditorApplication.isPlaying = false;
        //     #else
        //         Application.Quit();
        //     #endif
        // }
    }
    
    // Helper method untuk memeriksa apakah scene ada di build settings
    private bool SceneExistsInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
                return true;
        }
        return false;
    }
    
    // Menambahkan efek hover pada tombol
    private void AddHoverEffect(Button button)
    {
        // Mendapatkan komponen image dari button
        Image buttonImage = button.GetComponent<Image>();
        
        // Menambahkan event trigger jika belum ada
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();
        
        // Menambahkan event untuk pointer enter (hover)
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { 
            if (buttonImage != null) 
                buttonImage.color = highlightColor; 
        });
        trigger.triggers.Add(entryEnter);
        
        // Menambahkan event untuk pointer exit (selesai hover)
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { 
            if (buttonImage != null) 
                buttonImage.color = normalColor; 
        });
        trigger.triggers.Add(entryExit);
    }
}

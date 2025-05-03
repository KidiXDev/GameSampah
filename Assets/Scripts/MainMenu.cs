using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    // Referensi untuk tombol-tombol
    public Button playButton;
    public Button shopButton;
    public Button exitButton;
    
    // Referensi untuk popup Shop
    public GameObject shopPopup;
    public Button closeShopButton; // Tombol X untuk menutup popup shop
    
    // Referensi untuk video player di dalam SHOP
    public VideoPlayer shopVideo;
    
    // Referensi untuk popup Exit
    public GameObject exitPopup;
    public Button yesButton;
    public Button noButton;
    
    // Warna saat normal dan saat hover
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;
    
    // Parameter untuk animasi fisika
    [Header("Physics Animation")]
    public float fallDelay = 0.3f;           // Jeda antar elemen yang jatuh
    public float fallHeight = 800f;          // Ketinggian awal (di luar layar)
    public float fallDuration = 0.8f;        // Durasi jatuh
    public float bounceAmount = 30f;         // Besarnya efek bounce
    public float swingAmount = 5f;           // Besarnya ayunan untuk efek berhenti
    public int swingCount = 3;               // Jumlah ayunan sebelum berhenti
    public float buttonFallDelay = 0.15f;    // Jeda untuk tombol jatuh setelah panel utama

    // Flag untuk mengontrol animasi
    private bool animationInProgress = false;
    private List<RectTransform> menuElements = new List<RectTransform>();
    
    // Tambahkan variabel untuk menyimpan posisi awal popup
    private Vector2 shopPopupOriginalPosition;
    private Vector2 exitPopupOriginalPosition;
    
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
            exitButton.onClick.AddListener(ShowExitPopup);
            AddHoverEffect(exitButton);
        }
        
        // Menambahkan listener untuk tombol close shop (X)
        if (closeShopButton != null)
        {
            closeShopButton.onClick.AddListener(CloseShop);
            AddHoverEffect(closeShopButton);
        }
        
        // Menambahkan listener untuk tombol Yes dan No pada popup Exit
        if (yesButton != null)
        {
            yesButton.onClick.AddListener(ConfirmExit);
            AddHoverEffect(yesButton);
        }
        
        if (noButton != null)
        {
            noButton.onClick.AddListener(CancelExit);
            AddHoverEffect(noButton);
        }
        
        // Simpan posisi awal popup untuk penggunaan kemudian
        if (shopPopup != null)
        {
            shopPopupOriginalPosition = shopPopup.GetComponent<RectTransform>().anchoredPosition;
            shopPopup.SetActive(false);
        }
        
        if (exitPopup != null)
        {
            exitPopupOriginalPosition = exitPopup.GetComponent<RectTransform>().anchoredPosition;
            exitPopup.SetActive(false);
        }
        
        // Cari video player jika belum ditentukan
        if (shopVideo == null && shopPopup != null)
        {
            // Coba cari child dengan nama "Lmao" dan ambil komponen VideoPlayer
            Transform lmaoChild = shopPopup.transform.Find("Lmao");
            if (lmaoChild != null)
            {
                shopVideo = lmaoChild.GetComponent<VideoPlayer>();
                if (shopVideo == null)
                {
                    // Jika tidak ada komponen VideoPlayer, tambahkan
                    shopVideo = lmaoChild.gameObject.AddComponent<VideoPlayer>();
                }
            }
        }
        
        // Siapkan elemen menu untuk animasi - PERBAIKAN: Mencari dalam canvas
        menuElements.Clear(); // Pastikan list kosong dulu

        // Cari Canvas yang berisi UI - Perbaikan untuk menemukan elemen UI
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            // Telusuri semua child langsung dari Canvas
            foreach (Transform child in canvas.transform)
            {
                // Exclude Background dari animasi
                if (child.name.Equals("Background", System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Menemukan Background: " + child.name + " - akan diabaikan dari animasi");
                    continue; // Lewati Background
                }
                
                if (child.gameObject.activeSelf)
                {
                    RectTransform rectTransform = child.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        menuElements.Add(rectTransform);
                        Debug.Log("Menemukan elemen UI: " + child.name);
                    }
                }
            }
        }
        else
        {
            // Jika tidak menemukan Canvas, gunakan child langsung dari transform ini
            foreach (Transform child in transform)
            {
                // Exclude Background dari animasi
                if (child.name.Equals("Background", System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Menemukan Background: " + child.name + " - akan diabaikan dari animasi");
                    continue; // Lewati Background
                }
                
                if (child.gameObject.activeSelf)
                {
                    RectTransform rectTransform = child.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        menuElements.Add(rectTransform);
                        Debug.Log("Menemukan elemen UI: " + child.name);
                    }
                }
            }
        }
        
        // Log jumlah elemen yang ditemukan (untuk debug)
        Debug.Log("Total elemen UI yang ditemukan: " + menuElements.Count);

        // Jalankan animasi menu di awal jika elemen ditemukan
        if (menuElements.Count > 0)
        {
            StartCoroutine(AnimateMenuEntrance());
        }
        else
        {
            Debug.LogWarning("Tidak ada elemen UI yang ditemukan untuk dianimasikan!");
        }
    }
    
    // Animasi entrance menu dengan efek fisika yang lebih menarik - Disesuaikan untuk struktur UI terpisah
    private IEnumerator AnimateMenuEntrance()
    {
        if (animationInProgress || menuElements.Count == 0)
            yield break;

        animationInProgress = true;
        
        // Debug informasi untuk semua elemen yang ditemukan
        foreach (RectTransform element in menuElements)
        {
            Debug.Log("Elemen untuk animasi: " + element.name);
        }
        
        // Cari elemen-elemen UI penting berdasarkan nama yang tepat
        GameObject papanObj = null;
        GameObject playBtnObj = null;
        GameObject shopBtnObj = null; 
        GameObject exitBtnObj = null;
        GameObject backgroundObj = null;
        
        // Cari elemen berdasarkan nama yang spesifik
        foreach (RectTransform element in menuElements)
        {
            string elementName = element.name;
            Debug.Log("Memeriksa elemen: " + elementName);
            
            // Identifikasi elemen berdasarkan nama
            if (elementName.Equals("papan", System.StringComparison.OrdinalIgnoreCase))
            {
                papanObj = element.gameObject;
                Debug.Log("Menemukan papan: " + elementName);
            }
            else if (elementName.Equals("Play", System.StringComparison.OrdinalIgnoreCase))
            {
                playBtnObj = element.gameObject;
                Debug.Log("Menemukan Play button: " + elementName);
            }
            else if (elementName.Equals("Shop", System.StringComparison.OrdinalIgnoreCase))
            {
                shopBtnObj = element.gameObject;
                Debug.Log("Menemukan Shop button: " + elementName);
            }
            else if (elementName.Equals("Exit", System.StringComparison.OrdinalIgnoreCase))
            {
                exitBtnObj = element.gameObject;
                Debug.Log("Menemukan Exit button: " + elementName);
            }
            else if (elementName.Equals("Background", System.StringComparison.OrdinalIgnoreCase))
            {
                backgroundObj = element.gameObject;
                Debug.Log("Menemukan Background: " + elementName);
            }
        }
        
        // Sembunyikan semua elemen dulu (kecuali Background)
        foreach (RectTransform element in menuElements)
        {
            // Jika ini Background, lewati
            if (element.name.Equals("Background", System.StringComparison.OrdinalIgnoreCase))
                continue;
                
            // Simpan posisi asli di property tambahan
            Vector2 originalPos = element.anchoredPosition;
            // Geser ke atas
            element.anchoredPosition = new Vector2(originalPos.x, originalPos.y + fallHeight);
            Debug.Log("Menyembunyikan elemen: " + element.name + " posisi awal: " + originalPos + ", posisi baru: " + element.anchoredPosition);
        }

        // Animasikan papan utama dulu jika ditemukan
        if (papanObj != null)
        {
            RectTransform boardRect = papanObj.GetComponent<RectTransform>();
            StartCoroutine(AnimateElementFall(boardRect));
            Debug.Log("Memulai animasi untuk papan");
            yield return new WaitForSeconds(fallDelay * 2); // Tunggu lebih lama untuk papan
        }
        else
        {
            Debug.LogWarning("Papan tidak ditemukan untuk animasi!");
        }
        
        // Animasikan tombol play, shop, dan exit secara berurutan
        if (playBtnObj != null)
        {
            RectTransform playRect = playBtnObj.GetComponent<RectTransform>();
            StartCoroutine(AnimateElementFall(playRect));
            Debug.Log("Memulai animasi untuk Play button");
            yield return new WaitForSeconds(fallDelay);
        }
        
        if (shopBtnObj != null)
        {
            RectTransform shopRect = shopBtnObj.GetComponent<RectTransform>();
            StartCoroutine(AnimateElementFall(shopRect));
            Debug.Log("Memulai animasi untuk Shop button");
            yield return new WaitForSeconds(fallDelay);
        }
        
        if (exitBtnObj != null)
        {
            RectTransform exitRect = exitBtnObj.GetComponent<RectTransform>();
            StartCoroutine(AnimateElementFall(exitRect));
            Debug.Log("Memulai animasi untuk Exit button");
            yield return new WaitForSeconds(fallDelay);
        }
        
        // Animasikan elemen-elemen lain yang belum dianimasikan
        foreach (RectTransform element in menuElements)
        {
            // Lewati elemen yang sudah dianimasikan
            if ((papanObj != null && element.gameObject == papanObj) ||
                (playBtnObj != null && element.gameObject == playBtnObj) ||
                (shopBtnObj != null && element.gameObject == shopBtnObj) ||
                (exitBtnObj != null && element.gameObject == exitBtnObj))
                continue;
                
            StartCoroutine(AnimateElementFall(element));
            Debug.Log("Memulai animasi untuk elemen lain: " + element.name);
            yield return new WaitForSeconds(fallDelay);
        }

        // Tunggu hingga semua selesai
        yield return new WaitForSeconds(fallDuration + 0.5f);
        animationInProgress = false;
        Debug.Log("Animasi menu selesai");
    }

    // Animasi jatuh untuk elemen individual
    private IEnumerator AnimateElementFall(RectTransform element)
    {
        Vector2 startPos = element.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, startPos.y - fallHeight);
        
        // Cari tombol di dalam elemen ini
        List<RectTransform> childButtons = new List<RectTransform>();
        foreach (Button btn in element.GetComponentsInChildren<Button>())
        {
            if (btn.transform.parent == element)
            {
                childButtons.Add(btn.GetComponent<RectTransform>());
                // Sembunyikan tombol dulu
                Vector2 btnPos = btn.GetComponent<RectTransform>().anchoredPosition;
                btn.GetComponent<RectTransform>().anchoredPosition = 
                    new Vector2(btnPos.x, btnPos.y + fallHeight * 0.5f);
            }
        }

        // Animasi jatuh dengan physics
        float time = 0;
        while (time < fallDuration)
        {
            time += Time.deltaTime;
            float progress = time / fallDuration;
            
            // Kurva non-linear untuk efek gravitasi dan bounce
            float posY;
            
            if (progress < 0.7f) {
                // Efek jatuh dengan percepatan
                posY = Mathf.Lerp(startPos.y, targetPos.y, EaseOutQuad(progress / 0.7f));
            } else {
                // Efek bounce setelah mendarat
                float bounceProgress = (progress - 0.7f) / 0.3f;
                float bounceCurve = Mathf.Sin(bounceProgress * Mathf.PI * swingCount);
                posY = targetPos.y + bounceCurve * bounceAmount * (1 - bounceProgress);
            }
            
            element.anchoredPosition = new Vector2(startPos.x, posY);
            yield return null;
        }
        
        // Pastikan posisi akhir tepat
        element.anchoredPosition = targetPos;
        
        // Animasi tombol-tombol di dalam panel ini
        if (childButtons.Count > 0)
        {
            yield return new WaitForSeconds(buttonFallDelay);
            
            foreach (RectTransform btnRect in childButtons)
            {
                StartCoroutine(AnimateButtonAfterShock(btnRect));
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    
    // Animasi tombol setelah panel jatuh
    private IEnumerator AnimateButtonAfterShock(RectTransform buttonRect)
    {
        Vector2 startPos = buttonRect.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, startPos.y - fallHeight * 0.5f);
        
        float time = 0;
        float buttonFallDuration = fallDuration * 0.6f;
        
        while (time < buttonFallDuration)
        {
            time += Time.deltaTime;
            float progress = time / buttonFallDuration;
            
            // Kurva untuk efek bounce tombol
            float posY;
            if (progress < 0.6f) {
                // Jatuh
                posY = Mathf.Lerp(startPos.y, targetPos.y, EaseOutQuad(progress / 0.6f));
            } else {
                // Bounce
                float bounceProgress = (progress - 0.6f) / 0.4f;
                float bounceCurve = Mathf.Sin(bounceProgress * Mathf.PI * (swingCount + 2));
                posY = targetPos.y + bounceCurve * (bounceAmount * 0.7f) * (1 - bounceProgress);
            }
            
            // Tambahkan swing ke kiri-kanan
            float swingX = Mathf.Sin(progress * Mathf.PI * (swingCount + 1)) * swingAmount * (1 - progress);
            
            buttonRect.anchoredPosition = new Vector2(startPos.x + swingX, posY);
            yield return null;
        }
        
        // Pastikan posisi akhir tepat
        buttonRect.anchoredPosition = targetPos;
    }
    
    // Easing function untuk efek physics
    private float EaseOutQuad(float t)
    {
        return t * (2 - t);
    }

    // Fungsi untuk memulai game
    public void PlayGame()
    {
        // Pindah ke scene game (SampleScene)
        SceneManager.LoadScene("SampleScene");
    }
    
    // Fungsi untuk membuka shop dengan animasi fisika
    public void OpenShop()
    {
        if (shopPopup != null)
        {
            // Pastikan popup tidak sedang dalam animasi
            StopAllCoroutinesForObject(shopPopup);
            
            // Aktifkan popup dan pastikan posisi di awal reset ke posisi asli
            shopPopup.SetActive(true);
            RectTransform shopRect = shopPopup.GetComponent<RectTransform>();
            shopRect.rotation = Quaternion.identity; // Reset rotasi ke normal
            
            // Reset ke posisi asli dulu (penting!)
            shopRect.anchoredPosition = shopPopupOriginalPosition;
            
            // Lalu posisikan di luar layar
            shopRect.anchoredPosition = new Vector2(shopPopupOriginalPosition.x, shopPopupOriginalPosition.y + fallHeight);
            
            // Putar video jika ada
            if (shopVideo != null)
            {
                shopVideo.Play();
                Debug.Log("Shop video started playing");
            }
            
            // Animasikan popup turun
            StartCoroutine(AnimateShopPopupFall(shopRect, shopPopupOriginalPosition));
            
            Debug.Log("Shop popup opened with physics animation");
        }
        else
        {
            Debug.LogWarning("Shop popup tidak ditemukan!");
        }
    }
    
    // Metode untuk menghentikan semua coroutine untuk objek tertentu
    private void StopAllCoroutinesForObject(GameObject obj)
    {
        // Cari MonoBehaviour pada objek
        MonoBehaviour[] components = obj.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in components)
        {
            if (component != null && component.enabled)
            {
                component.StopAllCoroutines();
            }
        }
    }
    
    // Animasi untuk popup shop
    private IEnumerator AnimateShopPopupFall(RectTransform popupRect, Vector2 targetPos)
    {
        Vector2 startPos = popupRect.anchoredPosition;
        float time = 0;
        
        while (time < fallDuration)
        {
            time += Time.deltaTime;
            float progress = time / fallDuration;
            
            // Kurva dengan efek physics
            float posY;
            if (progress < 0.7f) {
                // Jatuh
                posY = Mathf.Lerp(startPos.y, targetPos.y, EaseOutQuad(progress / 0.7f));
            } else {
                // Bounce
                float bounceProgress = (progress - 0.7f) / 0.3f;
                float bounceCurve = Mathf.Sin(bounceProgress * Mathf.PI * swingCount);
                posY = targetPos.y + bounceCurve * bounceAmount * (1 - bounceProgress);
            }
            
            // Tambahkan sedikit gerakan horizontal untuk efek alami
            float swingX = Mathf.Sin(progress * Mathf.PI * 2) * (swingAmount * 0.3f) * (1 - progress);
            
            popupRect.anchoredPosition = new Vector2(targetPos.x + swingX, posY);
            yield return null;
        }
        
        // Pastikan posisi akhir tepat
        popupRect.anchoredPosition = targetPos;
        
        // Animasi elemen-elemen di dalam popup
        yield return new WaitForSeconds(0.1f);
        
        // Cari tombol-tombol dalam popup ini
        foreach (RectTransform child in popupRect)
        {
            if (child.GetComponent<Button>() != null && child != closeShopButton.transform)
            {
                StartCoroutine(AnimateButtonAfterShock(child));
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    
    // Menutup shop dengan animasi fisika
    public void CloseShop()
    {
        if (shopPopup != null)
        {
            RectTransform shopRect = shopPopup.GetComponent<RectTransform>();
            StartCoroutine(AnimateShopPopupClose(shopRect));
            
            // Hentikan video jika sedang diputar
            if (shopVideo != null && shopVideo.isPlaying)
            {
                shopVideo.Stop();
                Debug.Log("Shop video stopped");
            }
            
            Debug.Log("Shop popup closing with physics animation");
        }
    }
    
    // Animasi untuk menutup popup shop
    private IEnumerator AnimateShopPopupClose(RectTransform popupRect)
    {
        Vector2 startPos = popupRect.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, startPos.y + fallHeight);
        
        float time = 0;
        float closeDuration = fallDuration * 0.7f;
        
        while (time < closeDuration)
        {
            time += Time.deltaTime;
            float progress = time / closeDuration;
            
            // Kurva untuk efek terbang ke atas
            float posY = Mathf.Lerp(startPos.y, targetPos.y, EaseOutQuad(progress));
            
            // Tambahkan rotasi untuk efek dramatis
            float rotation = progress * 10f;
            popupRect.rotation = Quaternion.Euler(0, 0, rotation);
            
            popupRect.anchoredPosition = new Vector2(startPos.x, posY);
            yield return null;
        }
        
        // Sembunyikan popup
        shopPopup.SetActive(false);
        
        // Reset rotasi dan posisi untuk penggunaan berikutnya
        popupRect.rotation = Quaternion.identity;
        popupRect.anchoredPosition = shopPopupOriginalPosition;
    }

    // Fungsi untuk menampilkan popup konfirmasi exit dengan animasi
    public void ShowExitPopup()
    {
        if (exitPopup != null)
        {
            // Pastikan popup tidak sedang dalam animasi
            StopAllCoroutinesForObject(exitPopup);
            
            // Aktifkan popup
            exitPopup.SetActive(true);
            RectTransform exitRect = exitPopup.GetComponent<RectTransform>();
            exitRect.rotation = Quaternion.identity; // Reset rotasi ke normal
            
            // Reset ke posisi asli dulu
            exitRect.anchoredPosition = exitPopupOriginalPosition;
            
            // Posisikan di luar layar
            exitRect.anchoredPosition = new Vector2(exitPopupOriginalPosition.x, exitPopupOriginalPosition.y + fallHeight);
            
            // Delay sedikit agar tidak jatuh bersamaan dengan parent
            StartCoroutine(DelayedExitPopupAnimation(exitRect, exitPopupOriginalPosition));
            
            Debug.Log("Exit popup opened with physics animation");
        }
        else
        {
            Debug.LogWarning("Exit popup tidak ditemukan!");
        }
    }
    
    // Coroutine untuk menunda animasi popup exit
    private IEnumerator DelayedExitPopupAnimation(RectTransform exitRect, Vector2 targetPos)
    {
        // Tunggu sedikit agar tidak jatuh bersamaan dengan parent
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(AnimateExitPopupFall(exitRect, targetPos));
    }
    
    // Animasi untuk popup exit - perbaikan untuk tombol Yes dan No
    private IEnumerator AnimateExitPopupFall(RectTransform popupRect, Vector2 targetPos)
    {
        // Sembunyikan tombol Yes dan No sampai animasi papan selesai
        if (yesButton != null)
            yesButton.gameObject.SetActive(false);
            
        if (noButton != null)
            noButton.gameObject.SetActive(false);
            
        Vector2 startPos = popupRect.anchoredPosition;
        float time = 0;
        
        while (time < fallDuration)
        {
            time += Time.deltaTime;
            float progress = time / fallDuration;
            
            // Kurva dengan efek physics
            float posY;
            if (progress < 0.7f) {
                // Efek jatuh dengan percepatan
                posY = Mathf.Lerp(startPos.y, targetPos.y, EaseOutQuad(progress / 0.7f));
            } else {
                // Efek bounce setelah mendarat
                float bounceProgress = (progress - 0.7f) / 0.3f;
                float bounceCurve = Mathf.Sin(bounceProgress * Mathf.PI * swingCount);
                posY = targetPos.y + bounceCurve * bounceAmount * (1 - bounceProgress);
            }
            
            // Tambahkan sedikit gerakan horizontal untuk efek alami
            float swingX = Mathf.Sin(progress * Mathf.PI * 2) * (swingAmount * 0.3f) * (1 - progress);
            
            popupRect.anchoredPosition = new Vector2(targetPos.x + swingX, posY);
            yield return null;
        }
        
        // Pastikan posisi akhir tepat
        popupRect.anchoredPosition = targetPos;
        
        // Simpan posisi awal tombol Yes dan No
        Vector2 yesButtonOrigPos = Vector2.zero;
        Vector2 noButtonOrigPos = Vector2.zero;
        
        if (yesButton != null)
            yesButtonOrigPos = yesButton.GetComponent<RectTransform>().anchoredPosition;
            
        if (noButton != null)
            noButtonOrigPos = noButton.GetComponent<RectTransform>().anchoredPosition;
        
        // Tunggu hingga animasi popup benar-benar selesai
        yield return new WaitForSeconds(0.2f);
        
        // Sekarang mulai animasi tombol Yes dan No
        if (yesButton != null) {
            yesButton.gameObject.SetActive(true);
            RectTransform yesRect = yesButton.GetComponent<RectTransform>();
            // Siapkan posisi awal - Sembunyikan tombol dengan skala 0
            yesRect.localScale = Vector3.zero;
            StartCoroutine(AnimateButtonFromCave(yesRect, yesButtonOrigPos, 0f));
        }
        
        yield return new WaitForSeconds(0.15f);
        
        if (noButton != null) {
            noButton.gameObject.SetActive(true);
            RectTransform noRect = noButton.GetComponent<RectTransform>();
            // Siapkan posisi awal - Sembunyikan tombol dengan skala 0
            noRect.localScale = Vector3.zero;
            StartCoroutine(AnimateButtonFromCave(noRect, noButtonOrigPos, 0.1f));
        }
    }
    
    // Animasi khusus untuk tombol "keluar dari gua"
    private IEnumerator AnimateButtonFromCave(RectTransform buttonRect, Vector2 finalPos, float extraDelay)
    {
        // Tunggu delay tambahan jika diperlukan
        if (extraDelay > 0)
            yield return new WaitForSeconds(extraDelay);
        
        // Set posisi awal (mungkin sudah benar, tidak perlu ubah posisi)
        buttonRect.anchoredPosition = finalPos;
        
        // Animasi muncul secara bertahap dari skala 0 ke skala 1
        float duration = 0.4f;
        float elapsed = 0f;
        
        // Efek awal memutar keluar
        Vector3 startRotation = new Vector3(0, 0, -45);
        Vector3 endRotation = Vector3.zero;
        buttonRect.localRotation = Quaternion.Euler(startRotation);
        
        // Animasi skala dan rotasi
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float easedProgress = EaseOutBounce(progress); // Easing function untuk efek memantul
            
            // Animasi skala dari 0 ke 1.1 lalu ke 1
            float scaleFactor = Mathf.Min(easedProgress * 1.2f, 1f);
            if (progress > 0.7f)
            {
                // Pada akhir animasi, lakukan sedikit bounce
                float bounceProgress = (progress - 0.7f) / 0.3f;
                scaleFactor = Mathf.Lerp(1f, 1.1f, Mathf.Sin(bounceProgress * Mathf.PI) * 0.5f);
            }
            
            // Terapkan skala dengan efek memantul
            buttonRect.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            
            // Terapkan rotasi yang berputar ke posisi normal
            buttonRect.localRotation = Quaternion.Euler(Vector3.Lerp(startRotation, endRotation, easedProgress));
            
            yield return null;
        }
        
        // Pastikan akhirnya dalam kondisi tepat
        buttonRect.localScale = Vector3.one;
        buttonRect.localRotation = Quaternion.identity;
    }
    
    // Easing function untuk animasi bounce
    private float EaseOutBounce(float t)
    {
        if (t < 0.36f)
        {
            return 7.5625f * t * t;
        }
        else if (t < 0.72f)
        {
            t -= 0.545f;
            return 7.5625f * t * t + 0.75f;
        }
        else if (t < 0.9f)
        {
            t -= 0.8175f;
            return 7.5625f * t * t + 0.9375f;
        }
        else
        {
            t -= 0.95875f;
            return 7.5625f * t * t + 0.984375f;
        }
    }
    
    // Fungsi untuk konfirmasi exit (tombol Yes)
    public void ConfirmExit()
    {
        Debug.Log("Exit confirmed, quitting application");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    // Fungsi untuk membatalkan exit dengan animasi
    public void CancelExit()
    {
        if (exitPopup != null)
        {
            RectTransform exitRect = exitPopup.GetComponent<RectTransform>();
            StartCoroutine(AnimateExitPopupClose(exitRect));
            Debug.Log("Exit canceled with physics animation");
        }
    }
    
    // Animasi untuk menutup popup exit
    private IEnumerator AnimateExitPopupClose(RectTransform popupRect)
    {
        Vector2 startPos = popupRect.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, startPos.y + fallHeight);
        
        float time = 0;
        float closeDuration = fallDuration * 0.7f;
        
        while (time < closeDuration)
        {
            time += Time.deltaTime;
            float progress = time / closeDuration;
            
            // Kurva untuk efek terbang ke atas
            float posY = Mathf.Lerp(startPos.y, targetPos.y, EaseOutQuad(progress));
            
            // Tambahkan rotasi untuk efek dramatis
            float rotation = progress * 10f;
            popupRect.rotation = Quaternion.Euler(0, 0, rotation);
            
            popupRect.anchoredPosition = new Vector2(startPos.x, posY);
            yield return null;
        }
        
        // Sembunyikan popup
        exitPopup.SetActive(false);
        
        // Reset rotasi dan posisi untuk penggunaan berikutnya
        popupRect.rotation = Quaternion.identity;
        popupRect.anchoredPosition = exitPopupOriginalPosition;
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
        entryEnter.callback.AddListener((data) => { OnHoverEnter(button); });
        trigger.triggers.Add(entryEnter);
        
        // Menambahkan event untuk pointer exit (selesai hover)
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnHoverExit(button); });
        trigger.triggers.Add(entryExit);
    }

    private void OnHoverEnter(Button button)
    {
        // Mendapatkan komponen image dan mengubah warnanya
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = highlightColor;
        }
        
        // Opsional: menambahkan efek lain seperti skala atau suara
        button.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    private void OnHoverExit(Button button)
    {
        // Mengembalikan warna ke normal
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
        
        // Mengembalikan skala ke normal
        button.transform.localScale = Vector3.one;
    }
}
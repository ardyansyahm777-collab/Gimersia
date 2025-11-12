using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    // --- Referensi yang Perlu Diatur di Inspector ---

    [Header("Video & Scene Settings")]
    [Tooltip("Eplog")]
    public VideoPlayer videoPlayer;

    [Tooltip("Panel UI (Canvas Group) yang digunakan untuk transisi fade.")]
    public CanvasGroup fadePanel;

    [Tooltip("Nama Scene yang akan dimuat setelah video selesai (misal: 'MainMenu').")]
    public string nextSceneName = "MainMenu";

    [Header("Timing Settings")]
    [Tooltip("Durasi jeda awal sebelum fade out dimulai.")]
    public float initialDelay = 0.5f;

    [Tooltip("Durasi total transisi fade (masuk dan keluar).")]
    public float fadeDuration = 1.0f;


    // Dipanggil sebelum Start. Mempersiapkan layar agar tertutup (hitam) di awal.
    void Awake()
    {
        if (fadePanel != null)
        {
            // Mulai dengan alpha penuh (layar tertutup)
            fadePanel.alpha = 1;
            fadePanel.interactable = false;
            fadePanel.blocksRaycasts = false;
        }
    }

    void Start()
    {
        // Memulai seluruh urutan cutscene
        StartCoroutine(CutsceneSequence());
    }


    IEnumerator CutsceneSequence()
    {


        // Jeda singkat selama 0.5 detik
        yield return new WaitForSeconds(initialDelay);

        // Fade alpha dari 1 (tertutup) ke 0 (terbuka)
        yield return StartCoroutine(FadeCanvasGroup(fadePanel, 1, 0, fadeDuration));

        if (videoPlayer != null && videoPlayer.clip != null)
        {
            videoPlayer.Play();

            while (videoPlayer.isPlaying || !videoPlayer.isPrepared)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogError("VideoPlayer atau Video Clip tidak ditemukan. Pastikan sudah diatur di Inspector.");
            // Beri jeda singkat agar transisi tidak terlalu tiba-tiba
            yield return new WaitForSeconds(1.0f);
        }


        // Fade alpha dari 0 (terbuka) ke 1 (tertutup)
        yield return StartCoroutine(FadeCanvasGroup(fadePanel, 0, 1, fadeDuration));

        // Pindah ke scene berikutnya
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeCanvasGroup(CanvasGroup canvas, float startAlpha, float endAlpha, float duration)
    {
        float startTime = Time.time;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed = Time.time - startTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            canvas.alpha = newAlpha;
            yield return null;
        }

        // Pastikan alpha berakhir di nilai target
        canvas.alpha = endAlpha;
    }
}
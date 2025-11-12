using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // AudioClip untuk suara saat kursor di atas (Hover) - Anda akan mengisi ini dengan aset Pilih-Menu
    public AudioClip hoverSFX;

    // AudioClip untuk suara saat tombol diklik - Anda akan mengisi ini dengan aset Confirm-Menu
    public AudioClip clickSFX;

    // Referensi ke AudioSource di objek ini
    private AudioSource audioSource;

    void Awake()
    {
        // Ambil komponen AudioSource saat startup
        audioSource = GetComponent<AudioSource>();

        // Optional: Cek apakah AudioSource ditemukan
        if (audioSource == null)
        {
            Debug.LogError("SoundManager memerlukan komponen AudioSource pada GameObject yang sama.");
        }
    }

    public void PlayHoverSound()
    {
        if (audioSource != null && hoverSFX != null)
        {
            // Menggunakan PlayOneShot agar tidak mengganggu suara yang sedang berjalan
            audioSource.PlayOneShot(hoverSFX);
        }
    }

    public void PlayClickSound()
    {
        if (audioSource != null && clickSFX != null)
        {
            // Menggunakan PlayOneShot agar tidak mengganggu suara yang sedang berjalan
            audioSource.PlayOneShot(clickSFX);
        }
    }
}

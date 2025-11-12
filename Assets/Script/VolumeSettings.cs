using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        // Pastikan slider memiliki nilai default (misalnya 1 = maks)
        // jika PlayerPrefs tidak ada.
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            musicSlider.value = 1f; // Nilai default 1 (maks)
            SetMusicVolume();
        }
        else
        {
            LoadVolume();
        }

    }

    // Perbaikan: Ganti nama ke SetMusicVolume (tanpa 'u' ganda) dan hapus parameter
    public void SetMusicVolume()
    {
        // Ambil nilai dari slider
        float volume = musicSlider.value;

        // Atur nilai mixer. Pastikan "music" adalah nama parameter yang benar di AudioMixer.
        // Konversi linier slider.value (0.0001 hingga 1) ke logaritma dB (-80 hingga 0).
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);

        // Simpan nilai slider
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    private void LoadVolume()
    {
        // Muat nilai yang disimpan ke slider
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");

        // Terapkan nilai tersebut ke mixer
        SetMusicVolume();
    }
}
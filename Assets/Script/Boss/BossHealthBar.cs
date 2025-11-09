using UnityEngine;
using UnityEngine.UI; // Wajib ada untuk mengakses Slider

public class BossHealthBar : MonoBehaviour
{
    private Slider slider;

    // Ambil komponen Slider saat game baru dimulai
    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Fungsi untuk mengatur nilai maksimum (dipanggil 1x saat bos muncul)
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health; // Pastikan bar penuh saat mulai
    }

    // Fungsi untuk meng-update HP (dipanggil setiap kali bos kena damage)
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
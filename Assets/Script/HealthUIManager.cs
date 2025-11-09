using UnityEngine;
using UnityEngine.UI; // Wajib ada untuk UI

public class HealthUIManager : MonoBehaviour
{
    // Kita akan seret 3 gambar hati ke sini
    public Image[] hearts;

    // Fungsi ini akan dipanggil oleh PlayerMovement
    public void UpdateHearts(int currentHealth)
    {
        // Ulangi untuk setiap hati di dalam array
        for (int i = 0; i < hearts.Length; i++)
        {
            // Jika 'i' (hati ke-0, 1, atau 2)
            // lebih kecil dari nyawa sekarang...
            if (i < currentHealth)
            {
                hearts[i].enabled = true; // ...hati ini NYALA
            }
            else
            {
                hearts[i].enabled = false; // ...hati ini MATI
            }
        }
    }
}
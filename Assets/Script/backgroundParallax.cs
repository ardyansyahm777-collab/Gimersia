using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    private float length; // Panjang (lebar) sprite background
    private float startpos; // Posisi X awal background

    // Objek Kamera yang akan diikuti (biasanya Main Camera)
    public GameObject cam;

    // Kontrol seberapa cepat background bergerak relatif terhadap kamera
    [Range(-1f, 1f)]
    public float parallaxEffect;

    void Start()
    {
        // Menyimpan posisi X awal objek background
        startpos = transform.position.x;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            length = sr.bounds.size.x;
        }
        else
        {
            Debug.LogError("SpriteRenderer tidak ditemukan pada objek ini!");
        }

        // Pastikan variabel cam sudah di-set di Inspector
        if (cam == null)
        {
            // Coba temukan kamera utama secara otomatis jika belum di-set
            cam = Camera.main.gameObject;
        }
    }

    void FixedUpdate()
    {
        // Periksa jika cam masih null
        if (cam == null) return;

        // --- Perhitungan Parallax ---
        float dist = (cam.transform.position.x * parallaxEffect);

        // 2. Hitung posisi X baru
        // Posisi X baru = Posisi X awal + pergerakan paralaks
        float newPosX = startpos + dist;

        // 3. Terapkan posisi paralaks
        // transform.position.y TIDAK berubah, sehingga Y tetap
        transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);

        // --- Logika Looping (Reset Posisi) ---
        float offsetCameraX = (cam.transform.position.x - startpos) * (1 - parallaxEffect);

        if (offsetCameraX > length)
        {
            startpos += length;
        }
        else if (offsetCameraX < -length)
        {
            startpos -= length;
        }
    }
}
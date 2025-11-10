using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    [Header("Parallax Settings")]
    [Tooltip("Seberapa kuat efek parallax. 0 = diam, 1 = bergerak sama cepat dengan kamera.")]
    // Atur ini menjadi nilai kecil untuk background yang jauh (misal: 0.1)
    // dan nilai lebih besar untuk yang dekat (misal: 0.5)
    public Vector2 parallaxEffectMultiplier = new Vector2(0.1f, 0.1f);

    void Start()
    {
        // Cari kamera utama secara otomatis
        cameraTransform = Camera.main.transform;

        // Simpan posisi awal kamera
        lastCameraPosition = cameraTransform.position;
    }

    // Gunakan LateUpdate agar berjalan SETELAH kamera bergerak
    void LateUpdate()
    {
        // Hitung seberapa jauh kamera bergerak sejak frame terakhir
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Hitung pergerakan parallax
        float parallaxX = deltaMovement.x * parallaxEffectMultiplier.x;
        float parallaxY = deltaMovement.y * parallaxEffectMultiplier.y;

        // Gerakkan background ini sejauh (delta * multiplier)
        transform.Translate(new Vector3(parallaxX, parallaxY, 0));

        // Update posisi kamera terakhir untuk frame berikutnya
        lastCameraPosition = cameraTransform.position;
    }
}
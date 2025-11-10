using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour
{
    [Header("Settings")]
    public int ammoAmount = 5;
    public float respawnCooldown = 10f; // Cooldown 10 detik

    // Variabel internal
    private bool isCollected = false;
    private SpriteRenderer sprite;
    private Collider2D col;

    void Start()
    {
        // Ambil komponen yang akan kita nyala/matikan
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (sprite == null || col == null)
        {
            Debug.LogError("AmmoPickup butuh SpriteRenderer dan Collider2D!");
        }
    }

    // Biarkan pickup yang mendeteksi player
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek jika itu player DAN pickup ini sedang aktif (belum diambil)
        if (other.CompareTag("Player") && !isCollected)
        {
            // Ambil script player
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // Panggil fungsi baru di player untuk menambah ammo
                player.AddAmmo(ammoAmount);

                // Mulai proses respawn
                StartCoroutine(RespawnCoroutine());
            }
        }
    }

    // Coroutine untuk proses menghilang dan muncul kembali
    IEnumerator RespawnCoroutine()
    {
        // 1. Tandai sebagai "diambil"
        isCollected = true;

        // 2. Sembunyikan pickup
        SetVisible(false);

        // 3. Tunggu selama cooldown
        yield return new WaitForSeconds(respawnCooldown);

        // 4. Munculkan kembali pickup
        SetVisible(true);

        // 5. Tandai sebagai "siap diambil" lagi
        isCollected = false;
    }

    // Fungsi ini dipanggil dari luar (oleh Player) saat Game Over
    public void ResetPickup()
    {
        // Hentikan timer respawn yang mungkin sedang berjalan
        StopAllCoroutines();

        // Langsung munculkan kembali pickup
        SetVisible(true);
        isCollected = false;
    }

    // Fungsi helper agar rapi
    private void SetVisible(bool visible)
    {
        if (sprite == null || col == null) return;

        sprite.enabled = visible;
        col.enabled = visible;
    }
}
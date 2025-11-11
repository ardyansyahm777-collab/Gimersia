using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float speed = 10f;
    private int damage;
    private Vector2 direction;
    private float lifetime = 3f; // Hancur setelah 3 detik

    // Inisialisasi dari PlayerMovement
    public void Initialize(Vector2 dir, int dmg)
    {
        direction = dir;
        damage = dmg;
        Destroy(gameObject, lifetime);

        // --- TAMBAHAN BARU DI SINI ---

        // 1. Hitung sudut (derajat) dari vektor arah
        // Mathf.Atan2(y, x) menghitung sudut dalam radian.
        // Karena sprite Anda hadap kanan (0 derajat), ini sempurna.
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 2. Terapkan sudut itu ke rotasi Z proyektil
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // --- AKHIR TAMBAHAN ---
    }

    void Update()
    {
        // Gerakkan proyektil
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    // Deteksi tabrakan (pastikan collider-nya 'Is Trigger')
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek jika mengenai Boss
        if (other.CompareTag("Boss"))
        {
            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            SmallMonster monster = other.GetComponent<SmallMonster>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
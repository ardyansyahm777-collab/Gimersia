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
    }

    void Update()
    {
        // Gerakkan proyektil
        transform.Translate(direction * speed * Time.deltaTime);
    }

    // Deteksi tabrakan (pastikan collider-nya 'Is Trigger')
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek jika mengenai Boss
        if (other.CompareTag("Boss"))
        {
            // Ambil script BossController dan berikan damage
            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
            
            // Hancurkan diri sendiri setelah mengenai bos
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground")) // Hancur jika kena tanah
        {
            Destroy(gameObject);
        }
    }
}
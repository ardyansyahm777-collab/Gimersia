using UnityEngine;

public class MonsterUlat : MonoBehaviour
{
    private GameOverManager gameOverManager;
    public float speed = 2f;
    public Transform player;
    private bool canMove = true;
    private Vector3 startPosition;

    // --- TAMBAHKAN INI ---
    private Animator anim; // Variabel untuk Animator
    // ---

    void Start()
    {
        startPosition = transform.position;
        gameOverManager = FindObjectOfType<GameOverManager>();
        transform.localScale = new Vector3(1, 1, 1);

        // --- TAMBAHKAN INI ---
        anim = GetComponent<Animator>(); // Ambil komponen Animator
        // ---

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        // --- TAMBAHKAN INI ---
        // Kirim status 'canMove' ke Animator
        if (anim != null)
        {
            // Set parameter "isMoving" di Animator sesuai dengan variabel "canMove"
            anim.SetBool("isMoving", canMove);
        }
        // ---

        // Kode gameplay Anda TIDAK BERUBAH
        if (!canMove) return;
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    public void ResetMonster()
    {
        transform.position = startPosition;
        canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Barrier"))
        {
            StopMoving();
            Debug.Log("Monster menabrak barrier dan berhenti (trigger mode)!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Barrier"))
        {
            StopMoving();
            Debug.Log("Monster menabrak barrier dan berhenti!");
        }
    }

    public void StopMoving() => canMove = false;
    public void ResumeMoving() => canMove = true;
}
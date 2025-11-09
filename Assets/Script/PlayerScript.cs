using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public int maxJumps = 2;

    [Header("Health Settings")]
    public HealthUIManager healthUI;
    public int maxHealth = 3;
    private int currentHealth;

    private Rigidbody2D rb;
    private int jumpCount;
    private bool isGrounded;
    private GameOverManager gameOverManager;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth); // Update HP saat game mulai
        }
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        gameOverManager = FindObjectOfType<GameOverManager>();
    }

    void Update()
    {
        // Gerak kiri-kanan
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Lompat / double jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // --- SISTEM HP ---
    public void TakeDamage(int amount)
    {

        currentHealth -= amount;
        Debug.Log($"Player terkena serangan! HP tersisa: {currentHealth}");

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player mati — Game Over!");
            gameOverManager?.ShowGameOver();
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        // Update UI juga saat reset
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        // (Opsional) Aktifkan kembali player jika dimatikan
        gameObject.SetActive(true);
    }
}

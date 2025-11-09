using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public int maxJumps = 2; // jumlah maksimal lompatan (2 = double jump)

    private Rigidbody2D rb;
    private int jumpCount;   // menghitung sudah lompat berapa kali
    private bool isGrounded;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }



    void Update()
    {
        // Gerakan kiri-kanan (A dan D)
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Lompat dan double jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }
    }

    // Cek kalau player menyentuh tanah
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0; // reset lompat saat menyentuh tanah
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

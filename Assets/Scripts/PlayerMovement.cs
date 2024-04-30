using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    
    public float walkSpeed = 3;
    public float runSpeed = 5;
    public float jumpForce = 1;

    public bool isDead;
    public bool isGrounded;
    public LayerMask groundLayerMask;
    
    public TextMeshProUGUI scoreText;
    public float scoreValue;
    public TextMeshProUGUI gameOverText;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        gameOverText.gameObject.SetActive(false);
    }

    private void Start()
    {
        scoreValue = 0;
        scoreText.text = "Score: " + scoreValue;
        
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        // Execute movement only when not dead
        if (!isDead)
        {
            Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), 0);
        
            // Checking for movement using input axis
            if (Input.GetAxis("Horizontal") != 0)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Run(movement);
                }
                else
                {
                    Walk(movement);
                }
            }
            else
            {
                Idle();
            }

            // Checking for ground collision using a downward raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.75f, groundLayerMask);
            if (hit.collider != null)
            {
                isGrounded = true;
                animator.SetBool("isGrounded", true);
            }
            else
            {
                isGrounded = false;
                animator.SetBool("isGrounded", false);
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }
        }
        else
        {
            // Press R to respawn when you die
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    private void Idle()
    {
        animator.SetBool("isWalking", false);
    }

    private void Walk(Vector2 movement)
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
        rb.velocity = new Vector2(movement.x * walkSpeed, rb.velocity.y);

        // Flipping Player sprite based on direction of movement
        if (movement.x < 0)
        {
            transform.localScale = new Vector3(-0.6f, 0.6f, 0.6f);
        }
        else if (movement.x > 0)
        {
            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }
    }

    private void Run(Vector2 movement)
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", true);
        rb.velocity = new Vector2(movement.x * runSpeed, rb.velocity.y);
        
        // Flipping Player sprite based on direction of movement
        if (movement.x < 0)
        {
            transform.localScale = new Vector3(-0.6f, 0.6f, 0.6f);
        }
        else if (movement.x > 0)
        {
            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }
    }

    private void Jump()
    {
        animator.SetTrigger("isJumping");
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            IncreaseScore(5);
            audioSource.Play();
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DeathZone") || other.gameObject.CompareTag("EnemyFire"))
        {
            GameOver();
        }
    }

    private void IncreaseScore(int value)
    {
        scoreValue += value;
        scoreText.text = "Score: " + scoreValue;
    }

    private void GameOver()
    {
        animator.SetTrigger("isDead");
        gameOverText.gameObject.SetActive(true);
        isDead = true;
    }
}

using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Ground Check")]
    public Transform GroundCheck;
    public float GroundRadius = 0.2f;
    public LayerMask whatIsGround;
    public bool isGrounded;

    [Header("Dash Settings")]
    public bool podeDash = false;  // Ativado ao pegar a bota
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Ataque")]
    public int danoNormal = 50;        // Ataque Z
    public int danoEspecial = 10;      // Ataque X (menor)
    public AtaqueJogador ataqueHitbox; // arrastar no inspector
    public bool podeAtaqueEspecial = false; // Ativado ao pegar a espada

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (ataqueHitbox == null)
            Debug.LogWarning("Ataque Hitbox não está definido no Inspector!");
    }

    void Update()
    {
        CheckGround();

        if (!isDashing)
            Movement();

        Jump();
        Attack();
        Dash();
        UpdateAnimator();
        MirrorChildren();
    }

    // -------------------------------
    // Ground Check
    // -------------------------------
    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundRadius, whatIsGround);
    }

    // -------------------------------
    // Animations
    // -------------------------------
    private void UpdateAnimator()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsJumping", !isGrounded);
    }

    // -------------------------------
    // Ataque
    // -------------------------------
    private void Attack()
    {
        // ATAQUE NORMAL (Z)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (ataqueHitbox != null)
                ataqueHitbox.DefinirDano(danoNormal);

            animator.SetTrigger("Attack");
        }

        // ATAQUE ESPECIAL (X) — só se tiver a espada
        if (Input.GetKeyDown(KeyCode.X) && !isDashing && podeAtaqueEspecial)
        {
            if (ataqueHitbox != null)
                ataqueHitbox.DefinirDano(danoEspecial);

            animator.SetTrigger("Attack2");
        }
    }

    // -------------------------------
    // Dash
    // -------------------------------
    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && podeDash)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        animator.SetTrigger("Dash");

        float dashDirection = spriteRenderer.flipX ? -1 : 1;
        rb.velocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // -------------------------------
    // Pulo
    // -------------------------------
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    // -------------------------------
    // Movimento Horizontal
    // -------------------------------
    private void Movement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        MirrorSprite(moveInput);
    }

    private void MirrorSprite(float moveInput)
    {
        if (moveInput > 0)
            spriteRenderer.flipX = false;
        else if (moveInput < 0)
            spriteRenderer.flipX = true;
    }

    private void MirrorChildren()
    {
        foreach (var child in transform.GetComponentsInChildren<Transform>())
        {
            if (child == transform) continue;

            Quaternion newRotation = Quaternion.identity;

            if (spriteRenderer.flipX)
                newRotation = Quaternion.Euler(0, 180f, 0);

            child.localRotation = newRotation;
        }
    }
}

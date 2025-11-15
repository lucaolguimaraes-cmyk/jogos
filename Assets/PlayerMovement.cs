using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // -------------------------------
    // GROUND CHECK
    // -------------------------------
    [Header("Ground Check")]
    public Transform GroundCheck;
    public float GroundRadius = 0.2f;
    public LayerMask whatIsGround;
    public bool isGrounded;

    // -------------------------------
    // DASH SYSTEM
    // -------------------------------
    [Header("Dash Settings")]
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool isDashing = false;
    private bool canDash = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckGround();

        if (!isDashing)
            Movement(); // Desabilita movimento comum durante o dash

        Jump();
        Attack();
        Dash();
        UpdateAnimator();
        MirrorChildren();
    }

    // -------------------------------
    // GROUND CHECK
    // -------------------------------
    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundRadius, whatIsGround);
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsJumping", !isGrounded);
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetTrigger("Attack");
        }
    }

    // -------------------------------
    // DASH SYSTEM
    // -------------------------------
    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) // Remova "isGrounded" se quiser dash no ar
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

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

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

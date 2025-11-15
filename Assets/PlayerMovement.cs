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
    // NOVO SISTEMA DE DETECÇÃO DE CHÃO
    // -------------------------------
    [Header("Ground Check")]
    public Transform GroundCheck;
    public float GroundRadius = 0.2f;
    public LayerMask whatIsGround;
    public bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckGround();
        Movement();
        Jump();
        Attack();
        UpdateAnimator();
        MirrorChildren();
    }

    // -------------------------------
    // DETECÇÃO DE CHÃO CORRIGIDA
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

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
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

    // Removemos OnCollisionEnter2D e OnCollisionExit2D (NÃO SERVEM MAIS)
}

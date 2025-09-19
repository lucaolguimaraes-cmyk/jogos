using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variáveis públicas para ajuste no Inspector
    public float moveSpeed = 5f; // Velocidade de movimento
    public float jumpForce = 10f; // Força do pulo

    private Rigidbody2D rb; // Referência ao Rigidbody2D
    public bool isGrounded = true; // Verifica se o jogador está no chão

    void Start()
    {
        // Obtém o componente Rigidbody2D do GameObject
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movimento horizontal
        float moveInput = Input.GetAxis("Horizontal"); // Captura entrada do teclado (A/D ou setas)
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Pulo
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    // Verifica se o jogador está no chão (não é a melhor forma de fazer isso)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
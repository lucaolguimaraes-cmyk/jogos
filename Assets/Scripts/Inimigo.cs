using System.Collections;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [Header("Configurações")]
    public float moveSpeed = 2f;       // Velocidade de movimento
    public int maxHealth = 2;          // Vida do inimigo
    public float knockbackForce = 5f;  // Força do recuo ao levar dano
    [SerializeField] private bool movingRight = false;   // começar andando para a esquerda
    private bool vivo = true;
    private bool isKnockBacked = false;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        // Force a direção/flip inicial para evitar comportamento inconsistente
        SetDirecaoInicial();
    }

    void Update()
    {
        if (isKnockBacked || !vivo) return;
        Move();
    }

    void Move()
    {
        float direction = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // Usa movingRight para flip (mais previsível)
        spriteRenderer.flipX = movingRight == false ? true : false;

        anim.SetFloat("Velocidade", Mathf.Abs(rb.velocity.x));
    }

    private void SetDirecaoInicial()
    {
        // Garante que o sprite e física comecem no estado correto
        float initialDir = movingRight ? 1f : -1f;
        spriteRenderer.flipX = movingRight == false ? true : false;
        // optional: zera velocidade inicial
        rb.velocity = new Vector2(initialDir * moveSpeed, rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Se colidiu com player
        if (collision.gameObject.CompareTag("Player"))
        {
            SistemaDeVida sistemaDeVida = collision.gameObject.GetComponent<SistemaDeVida>();
            if (sistemaDeVida != null)
                sistemaDeVida.AplicarDano(10);
            return;
        }

        // Inverte apenas se colisão for majoritariamente horizontal (evita flip ao tocar chão)
        // Verifica os pontos de contato e checa a componente X da normal
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // normal aponta para fora do inimigo; se |normal.x| > |normal.y| então foi um impacto lateral
            if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
            {
                // opcional: ignore colisões com outros inimigos se quiser
                if (collision.gameObject.CompareTag("Inimigo")) continue;

                movingRight = !movingRight;
                return;
            }
        }
    }

    public void EfeitoDeRecuo()
    {
        isKnockBacked = true;

        float knockbackDirection = movingRight ? -1f : 1f;
        Vector2 force = new Vector2(knockbackDirection * knockbackForce, 0f);

        rb.velocity = new Vector2(0f, rb.velocity.y);
        rb.AddForce(force, ForceMode2D.Impulse);

        StartCoroutine(ResetKnockback());
    }

    IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(0.5f);
        isKnockBacked = false;
    }

    public void EfeitoDePiscar()
    {
        StartCoroutine(Piscar());
    }

    IEnumerator Piscar()
    {
        Color corOriginal = spriteRenderer.color;
        Color corTransparente = new Color(corOriginal.r, corOriginal.g, corOriginal.b, 0.5f);

        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = corTransparente;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = corOriginal;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void AnimacaoDeDano()
    {
        anim.SetTrigger("Machucado");
        StartCoroutine(ResetMachucado());
    }

    IEnumerator ResetMachucado()
    {
        yield return new WaitForSeconds(0.5f);
        anim.ResetTrigger("Machucado");
    }

    internal void AnimacaoDeMorte()
    {
        vivo = false;

        rb.isKinematic = true;
        col.enabled = false;

        anim.SetBool("Vivo", vivo);
        EfeitoDePiscar();

        Destroy(gameObject, 3f);
    }
}

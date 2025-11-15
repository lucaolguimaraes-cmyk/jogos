using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Configurações do BOSS")]
    public int maxHealth = 1000;      // Vida do BOSS
    public float moveSpeed = 2f;
    public float knockbackForce = 5f;
    [SerializeField] private bool movingRight = false;

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

        spriteRenderer.flipX = movingRight == false ? true : false;

        anim.SetFloat("Velocidade", Mathf.Abs(rb.velocity.x));
    }

    private void SetDirecaoInicial()
    {
        float initialDir = movingRight ? 1f : -1f;
        spriteRenderer.flipX = movingRight == false ? true : false;
        rb.velocity = new Vector2(initialDir * moveSpeed, rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SistemaDeVida sistemaDeVida = collision.gameObject.GetComponent<SistemaDeVida>();
            if (sistemaDeVida != null)
                sistemaDeVida.AplicarDano(10);
            return;
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
            {
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

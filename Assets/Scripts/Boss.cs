using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Configurações do BOSS")]
    public int maxHealth = 1000;
    public float moveSpeed = 2f;
    public float knockbackForce = 5f;
    public bool movingRight = false;

    [Header("Ataque")]
    public int attackDamage = 10;
    public float attackInterval = 0.8f; // tempo entre ataques enquanto o player estiver em contato

    private bool vivo = true;
    private bool isKnockBacked = false;

    [Header("Perseguição")]
    public Transform player;
    public float distanciaVisao = 8f;
    public float alturaVisao = 3f;
    private bool vendoPlayer = false;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private float desiredHorizontalVelocity = 0f;

    // controle de ataque contínuo
    private bool playerEmContato = false;
    private GameObject playerEmContatoObj = null;
    private Coroutine ataqueCoroutine = null;

    private Coroutine piscarCoroutine = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (player == null)
        {
            var pObj = GameObject.FindWithTag("Player");
            if (pObj != null) player = pObj.transform;
        }

        rb.isKinematic = false;
        SetDirecaoInicial();
    }

    void Update()
    {
        if (!vivo || isKnockBacked)
        {
            desiredHorizontalVelocity = 0f;
            anim.SetFloat("Velocidade", 0f);
            return;
        }

        VerificarVisao();

        if (vendoPlayer)
            PerseguirJogador();
        else
            AndarNormal();

        anim.SetFloat("Velocidade", Mathf.Abs(desiredHorizontalVelocity));
    }

    void FixedUpdate()
    {
        if (isKnockBacked)
            return; // não sobrescreve força durante knockback

        rb.velocity = new Vector2(desiredHorizontalVelocity, rb.velocity.y);
    }

    // ------------------- MOVIMENTO --------------------
    void AndarNormal()
    {
        float dir = movingRight ? 1f : -1f;
        desiredHorizontalVelocity = dir * moveSpeed;
        spriteRenderer.flipX = !movingRight;
    }

    void PerseguirJogador()
    {
        if (player == null)
        {
            desiredHorizontalVelocity = 0f;
            return;
        }

        float dir = player.position.x > transform.position.x ? 1f : -1f;
        movingRight = dir > 0;

        desiredHorizontalVelocity = dir * moveSpeed;
        spriteRenderer.flipX = !movingRight;
    }

    void VerificarVisao()
    {
        if (player == null)
        {
            vendoPlayer = false;
            return;
        }

        float distX = Mathf.Abs(player.position.x - transform.position.x);
        float distY = Mathf.Abs(player.position.y - transform.position.y);

        vendoPlayer = (distX <= distanciaVisao && distY <= alturaVisao);
    }

    private void SetDirecaoInicial()
    {
        desiredHorizontalVelocity = (movingRight ? 1f : -1f) * moveSpeed;
        spriteRenderer.flipX = !movingRight;
    }

    // ------------------- COLISÕES --------------------
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IniciarContatoComPlayer(collision.gameObject);
            return;
        }

        if (!vendoPlayer)
        {
            foreach (var contact in collision.contacts)
            {
                if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
                {
                    if (collision.gameObject.CompareTag("Inimigo")) continue;

                    movingRight = !movingRight;
                    desiredHorizontalVelocity = (movingRight ? 1f : -1f) * moveSpeed;
                    return;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PararContatoComPlayer();
        }
    }

    // ------------------- ATAQUE --------------------
    void IniciarContatoComPlayer(GameObject playerObj)
    {
        playerEmContato = true;
        playerEmContatoObj = playerObj;

        if (ataqueCoroutine == null && vivo && !isKnockBacked)
            ataqueCoroutine = StartCoroutine(LoopAtaqueContínuo());
    }

    void PararContatoComPlayer()
    {
        playerEmContato = false;
        playerEmContatoObj = null;

        if (ataqueCoroutine != null)
        {
            StopCoroutine(ataqueCoroutine);
            ataqueCoroutine = null;
        }
    }

    IEnumerator LoopAtaqueContínuo()
    {
        while (playerEmContato && vivo)
        {
            if (isKnockBacked)
            {
                yield return null;
                continue;
            }

            anim.SetTrigger("Atacando");

            if (playerEmContatoObj != null)
            {
                var vida = playerEmContatoObj.GetComponent<SistemaDeVida>();
                if (vida != null)
                    vida.AplicarDano(attackDamage);
            }

            yield return new WaitForSeconds(attackInterval);
        }

        ataqueCoroutine = null;
    }

    // ------------------- DANO / MORTO --------------------
    public void EfeitoDeRecuo()
    {
        isKnockBacked = true;

        float dir = movingRight ? -1f : 1f;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        rb.AddForce(new Vector2(dir * knockbackForce, 0f), ForceMode2D.Impulse);

        StartCoroutine(ResetKnockback());
    }

    IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(0.5f);
        isKnockBacked = false;

        if (playerEmContato && ataqueCoroutine == null && vivo)
            ataqueCoroutine = StartCoroutine(LoopAtaqueContínuo());
    }

    public void EfeitoDePiscar()
    {
        if (piscarCoroutine != null)
            StopCoroutine(piscarCoroutine);

        piscarCoroutine = StartCoroutine(Piscar());
    }

    IEnumerator Piscar()
    {
        Color original = spriteRenderer.color;
        Color transparente = new Color(original.r, original.g, original.b, 0.5f);

        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = transparente;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = original;
            yield return new WaitForSeconds(0.1f);
        }

        spriteRenderer.color = original;
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

    public void AnimacaoDeMorte()
    {
        vivo = false;

        rb.isKinematic = true;
        col.enabled = false;

        anim.SetBool("Vivo", false);

        PararContatoComPlayer();

        Destroy(gameObject, 3f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position;
        Gizmos.DrawWireCube(center, new Vector3(distanciaVisao * 2f, alturaVisao * 2f, 0.1f));
    }
}

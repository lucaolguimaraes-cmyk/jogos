using UnityEngine;

public class AtaqueJogador : MonoBehaviour
{
    [SerializeField] int danoJogador = 50;

    void OnTriggerEnter2D(Collider2D other)
    {
        // -------------------------
        // Dano em Inimigo normal
        // -------------------------
        if (other.CompareTag("Inimigo"))
        {
            var vidaInimigo = other.GetComponent<SistemaDeVidaInimigo>();

            if (vidaInimigo != null)
            {
                vidaInimigo.AplicarDano(danoJogador);
            }

            return;
        }

        // -------------------------
        // Dano no Boss
        // -------------------------
        if (other.CompareTag("Boss"))
        {
            var vidaBoss = other.GetComponent<SistemaDeVidaBoss>();

            if (vidaBoss != null)
            {
                vidaBoss.AplicarDano(danoJogador);
            }

            return;
        }
    }
}

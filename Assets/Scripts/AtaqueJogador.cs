using UnityEngine;

public class AtaqueJogador : MonoBehaviour
{
    [SerializeField] int danoJogador = 50;

    public void DefinirDano(int novoDano)
    {
        danoJogador = novoDano;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Inimigo"))
        {
            var vidaInimigo = other.GetComponent<SistemaDeVidaInimigo>();
            if (vidaInimigo != null)
                vidaInimigo.AplicarDano(danoJogador);

            return;
        }

        if (other.CompareTag("Boss"))
        {
            var vidaBoss = other.GetComponent<SistemaDeVidaBoss>();
            if (vidaBoss != null)
                vidaBoss.AplicarDano(danoJogador);

            return;
        }

        if (other.CompareTag("Voador"))
        {
            var vidaVoador = other.GetComponent<SistemaDeVidaVoador>();
            if (vidaVoador != null)
                vidaVoador.AplicarDano(danoJogador);

            return;
        }
    }
}

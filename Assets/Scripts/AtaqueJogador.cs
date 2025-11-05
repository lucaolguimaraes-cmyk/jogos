using UnityEngine;

public class AtaqueJogador : MonoBehaviour
{
    [SerializeField] int danoJogador = 10;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Inimigo"))
        {
            SistemaDeVidaInimigo sistemaDeVidaInimigo = other.gameObject.GetComponent<SistemaDeVidaInimigo>();
            sistemaDeVidaInimigo.AplicarDano(danoJogador);
        }
    }
}

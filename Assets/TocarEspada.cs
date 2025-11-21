using UnityEngine;
using UnityEngine.UI;

public class TocarEspada : MonoBehaviour
{
    public Image imagemDoCanvas;
    public Sprite novaImagem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Troca a imagem do item no HUD
            if (imagemDoCanvas != null && novaImagem != null)
            {
                imagemDoCanvas.sprite = novaImagem;
            }

            // Ativa o ataque especial do player (X)
            other.GetComponent<PlayerMovement>().podeAtaqueEspecial = true;

            // Remove o item da cena
            Destroy(gameObject);
        }
    }
}

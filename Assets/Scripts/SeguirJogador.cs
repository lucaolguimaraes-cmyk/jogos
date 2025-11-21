using UnityEngine;

public class SeguirJogador : MonoBehaviour
{
    [SerializeField] GameObject jogador;
    [SerializeField] Vector3 offset;
    void Awake()
    {
        if (jogador == null)
        {
            jogador = FindObjectOfType<PlayerMovement>().gameObject;
        }
    }
    void LateUpdate()
    {
        transform.position = jogador.transform.position + offset;
    }
}

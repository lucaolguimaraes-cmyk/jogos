public class SistemaDeVidaBoss : SistemaDeVida
{
    Boss boss;
    BarraDeVidaInimigo barraDeVidaInimigo;

    new void Start()
    {
        boss = GetComponent<Boss>();
        barraDeVidaInimigo = GetComponentInChildren<BarraDeVidaInimigo>();

        // Boss usa maxHealth definido no script Boss
        vidaMaxima = boss.maxHealth;
        vidaAtual = vidaMaxima;
    }

    public override void AplicarDano(float dano)
    {
        vidaAtual -= dano;
        if (vidaAtual <= 0)
        {
            Morrer();
        }

        boss.AnimacaoDeDano();
        boss.EfeitoDePiscar();
        boss.EfeitoDeRecuo();
        AtualizarVida();
    }

    protected override void Morrer()
    {
        boss.AnimacaoDeMorte();
    }

    void AtualizarVida()
    {
        barraDeVidaInimigo.AtualizarUI(vidaAtual / vidaMaxima);
    }
}

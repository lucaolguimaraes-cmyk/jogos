public class SistemaDeVidaBoss : SistemaDeVida
{
    Boss boss;
    BarraDeVidaInimigo barraDeVidaInimigo;

    new void Start()
    {
        boss = GetComponent<Boss>();
        barraDeVidaInimigo = GetComponentInChildren<BarraDeVidaInimigo>();

        vidaMaxima = boss.maxHealth;
        vidaAtual = vidaMaxima;

        AtualizarVida(); // garante barra cheia no início
    }

    public override void AplicarDano(float dano)
    {
        if (vidaAtual <= 0) return; // já morreu, ignora

        vidaAtual -= dano;

        if (vidaAtual <= 0)
        {
            vidaAtual = 0;
            Morrer();
        }
        else
        {
            boss.AnimacaoDeDano();
            boss.EfeitoDePiscar();
            boss.EfeitoDeRecuo();
        }

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

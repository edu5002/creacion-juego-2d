using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    public int danio = 1;

    [Header("Solo para enemigos con embestida")]
    public bool soloDuranteEmbestida = false;
    public EnemigoSeguidor seguidor;

    private EnemyVida vidaEnemigo;

    private void Awake()
    {
        vidaEnemigo = GetComponentInParent<EnemyVida>();

        if (seguidor == null)
            seguidor = GetComponentInParent<EnemigoSeguidor>();
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        AplicarDanio(otro);
    }

    private void OnTriggerStay2D(Collider2D otro)
    {
        AplicarDanio(otro);
    }

    private void AplicarDanio(Collider2D otro)
    {
        if (!isActiveAndEnabled || Time.timeScale == 0f)
            return;

        if (vidaEnemigo != null && vidaEnemigo.EstaMuerto)
            return;

        PlayerVida jugador =
            otro.GetComponentInParent<PlayerVida>();

        if (jugador == null || jugador.EstaMuerto)
            return;

        if (soloDuranteEmbestida)
        {
            if (seguidor == null || !seguidor.PuedeHacerDanio)
                return;

            seguidor.RegistrarImpacto();
        }

        jugador.RecibirDanio(danio, transform.position);
    }
}
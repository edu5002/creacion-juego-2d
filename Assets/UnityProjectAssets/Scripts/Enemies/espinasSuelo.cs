using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EspinasSuelo : MonoBehaviour
{
    [Min(1)]
    public int danio = 1;

    private BoxCollider2D zona;

    private void Awake()
    {
        zona = GetComponent<BoxCollider2D>();
        zona.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        AplicarDanio(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        AplicarDanio(other);
    }

    private void AplicarDanio(Collider2D other)
    {
        if (!isActiveAndEnabled || Time.timeScale == 0f)
            return;

        PlayerVida jugador = other.GetComponentInParent<PlayerVida>();

        if (jugador == null || jugador.EstaMuerto)
            return;

        jugador.RecibirDanio(danio, zona.bounds.center);
    }
}
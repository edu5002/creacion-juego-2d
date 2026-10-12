using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    public int danio = 1;

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
        PlayerVida vida = otro.GetComponentInParent<PlayerVida>();

        if (vida != null)
        {
            vida.RecibirDanio(danio, transform.position);
        }
    }
}
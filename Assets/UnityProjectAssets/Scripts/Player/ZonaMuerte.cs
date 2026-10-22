using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ZonaMuerte : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D otro)
    {
        PlayerVida jugador =
            otro.GetComponentInParent<PlayerVida>();

        if (jugador != null)
        {
            jugador.MorirPorCaida();
            return;
        }

        EnemyVida enemigo =
            otro.GetComponentInParent<EnemyVida>();

        if (enemigo != null)
        {
            enemigo.gameObject.SetActive(false);
            Destroy(enemigo.gameObject);
        }
    }
}
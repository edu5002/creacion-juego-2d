using UnityEngine;
using UnityEngine.UI;

public class BarraVidaUI : MonoBehaviour
{
    public PlayerVida jugador;
    public Image relleno;

    [Header("Velocidad de la animación")]
    public float velocidadCambio = 2f;

    private void Start()
    {
        if (jugador != null && relleno != null)
            relleno.fillAmount = ObtenerPorcentaje();
    }

    private void Update()
    {
        if (jugador == null || relleno == null)
            return;

        relleno.fillAmount = Mathf.MoveTowards(
            relleno.fillAmount,
            ObtenerPorcentaje(),
            velocidadCambio * Time.deltaTime
        );
    }

    private float ObtenerPorcentaje()
    {
        return Mathf.Clamp01(
            (float)jugador.VidaActual /
            Mathf.Max(1, jugador.vidaMaxima)
        );
    }
}
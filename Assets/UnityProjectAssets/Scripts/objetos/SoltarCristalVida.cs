using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyVida))]
public class SoltarCristalVida : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerVida jugador;
    public CristalVida prefabCristal;
    public Transform puntoAparicion;
    public Vector2 desplazamiento = new Vector2(0f, 0.5f);

    [Header("Probabilidad")]
    [Range(0f, 100f)]
    public float probabilidadBase = 20f;

    [Min(1f)]
    public float multiplicadorMaximo = 2.5f;

    [Header("Garantía opcional")]
    public bool usarGarantia = true;

    [Min(1)]
    public int fallosAntesDeGarantizar = 5;

    private bool procesado;
    [Header("Curación del cristal")]
    [Min(1)]
    public int cantidadCuracion = 1;

    // Un contador compartido por los enemigos de cada escena.
    private static readonly Dictionary<ulong, int> fallosPorNivel =
        new Dictionary<ulong, int>();

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarContadores()
    {
        fallosPorNivel.Clear();
    }

    public void IntentarSoltar()
    {
        if (procesado)
            return;

        procesado = true;

        if (!isActiveAndEnabled ||
            jugador == null ||
            prefabCristal == null ||
            jugador.EstaMuerto ||
            probabilidadBase <= 0f)
            return;

        int vidaMaxima = Mathf.Max(1, jugador.vidaMaxima);

        if (jugador.VidaActual >= vidaMaxima)
            return;

        ulong nivel = gameObject.scene.handle.GetRawData();

        fallosPorNivel.TryGetValue(nivel, out int fallos);

        // Con un punto de vida alcanza el multiplicador máximo.
        float necesidad = Mathf.Clamp01(
            (vidaMaxima - jugador.VidaActual) /
            (float)Mathf.Max(1, vidaMaxima - 1)
        );

        float multiplicador = Mathf.Lerp(
            1f,
            Mathf.Max(1f, multiplicadorMaximo),
            necesidad
        );

        float probabilidad = Mathf.Clamp(
            probabilidadBase * multiplicador,
            0f,
            100f
        );

        bool garantizado =
            usarGarantia &&
            fallos >= Mathf.Max(1, fallosAntesDeGarantizar);

        bool soltar =
            garantizado || Random.value < probabilidad / 100f;

        if (!soltar)
        {
            fallosPorNivel[nivel] = fallos + 1;
            return;
        }

        fallosPorNivel[nivel] = 0;

        Vector3 posicion = puntoAparicion != null
            ? puntoAparicion.position
            : transform.position + (Vector3)desplazamiento;

        CristalVida cristal = Instantiate(
           prefabCristal,
           posicion,
           Quaternion.identity
       );

        cristal.cantidadCuracion = Mathf.Max(1, cantidadCuracion);
    }
}
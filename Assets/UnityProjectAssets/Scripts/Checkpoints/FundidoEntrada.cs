using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FundidoEntrada : MonoBehaviour
{
    [SerializeField, Min(0.1f)]
    private float duracionEntrada = 0.7f;

    [SerializeField, Min(0.1f)]
    private float duracionOscurecerPuerta = 0.3f;

    private CanvasGroup grupo;

    public bool EnCurso { get; private set; } = true;

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        grupo.alpha = 1f;
        grupo.interactable = false;
        grupo.blocksRaycasts = true;
        transform.SetAsLastSibling();
    }

    private IEnumerator Start()
    {
        yield return null;
        yield return Fundir(0f, duracionEntrada);
        TerminarFundido();
    }

    public void CruzarPuerta(Action trasladar)
    {
        if (EnCurso)
            return;

        EnCurso = true;
        StartCoroutine(TransicionPuerta(trasladar));
    }

    private IEnumerator TransicionPuerta(Action trasladar)
    {
        grupo.blocksRaycasts = true;
        transform.SetAsLastSibling();

        try
        {
            yield return Fundir(1f, duracionOscurecerPuerta);

            trasladar.Invoke();

            // Deja un fotograma en negro tras recolocar la cámara.
            yield return null;

            yield return Fundir(0f, duracionEntrada);
        }
        finally
        {
            TerminarFundido();
        }
    }

    private IEnumerator Fundir(float destino, float duracion)
    {
        float origen = grupo.alpha;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;
            float progreso = Mathf.Clamp01(tiempo / duracion);

            grupo.alpha = Mathf.SmoothStep(origen, destino, progreso);
            yield return null;
        }

        grupo.alpha = destino;
    }

    private void TerminarFundido()
    {
        grupo.alpha = 0f;
        grupo.blocksRaycasts = false;
        EnCurso = false;
    }
}
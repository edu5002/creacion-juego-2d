using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PresentacionNivelUI : MonoBehaviour
{
    [Header("Contenido")]
    [Min(1)] public int numeroNivel = 1;
    public string nombreNivel = "Bosque Encantado";
    public TMP_Text textoNivel;

    [Header("Duración")]
    [Min(0.01f)] public float tiempoAparicion = 0.6f;
    [Min(0f)] public float tiempoVisible = 2.5f;
    [Min(0.01f)] public float tiempoDesaparicion = 0.8f;

    private CanvasGroup grupo;

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        grupo.alpha = 0f;
        grupo.interactable = false;
        grupo.blocksRaycasts = false;
    }

    private IEnumerator Start()
    {
        if (textoNivel == null)
        {
            Debug.LogWarning(
                "Asigna Texto Nivel en PresentacionNivelUI.", this
            );
            yield break;
        }

        textoNivel.text =
            $"<size=65%>NIVEL {numeroNivel}</size>\n{nombreNivel}";

        yield return Desvanecer(0f, 1f, tiempoAparicion);

        yield return new WaitForSeconds(tiempoVisible);

        yield return Desvanecer(1f, 0f, tiempoDesaparicion);
    }

    private IEnumerator Desvanecer(
        float inicio, float final, float duracion)
    {
        float transcurrido = 0f;

        while (transcurrido < duracion)
        {
            transcurrido += Time.deltaTime;

            grupo.alpha = Mathf.Lerp(
                inicio,
                final,
                transcurrido / Mathf.Max(0.01f, duracion)
            );

            yield return null;
        }

        grupo.alpha = final;
    }
}
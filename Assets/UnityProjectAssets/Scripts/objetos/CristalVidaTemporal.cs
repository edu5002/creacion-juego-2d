using UnityEngine;

public class CristalVidaTemporal : MonoBehaviour
{
    [Min(0.1f)]
    public float duracion = 10f;

    [Min(0f)]
    public float segundosParpadeo = 3f;

    [Min(0.02f)]
    public float intervaloParpadeo = 0.12f;

    private SpriteRenderer[] imagenes;
    private bool[] visibilidadInicial;
    private float tiempo;

    private void Awake()
    {
        imagenes = GetComponentsInChildren<SpriteRenderer>(true);
        visibilidadInicial = new bool[imagenes.Length];

        for (int i = 0; i < imagenes.Length; i++)
            visibilidadInicial[i] = imagenes[i].enabled;
    }

    private void Update()
    {
        tiempo += Time.deltaTime;

        float limite = Mathf.Max(0.1f, duracion);

        if (tiempo >= limite)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        float inicioParpadeo =
            limite - Mathf.Clamp(segundosParpadeo, 0f, limite);

        if (tiempo < inicioParpadeo)
            return;

        float intervalo = Mathf.Max(0.02f, intervaloParpadeo);

        bool visible = Mathf.FloorToInt(
            (tiempo - inicioParpadeo) / intervalo
        ) % 2 == 0;

        for (int i = 0; i < imagenes.Length; i++)
        {
            if (imagenes[i] != null)
            {
                imagenes[i].enabled =
                    visibilidadInicial[i] && visible;
            }
        }
    }
}
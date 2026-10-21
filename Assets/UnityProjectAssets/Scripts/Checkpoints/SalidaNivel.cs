using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class SalidaNivel : MonoBehaviour
{
    [Header("Destino")]
    public string siguienteNivel = "Nivel2";

    [Header("Interfaz")]
    public GameObject textoEntrar;
    public CanvasGroup fundido;

    [Header("Transición")]
    [Min(0.1f)]
    public float duracionFundido = 0.8f;

    private readonly HashSet<Collider2D> contactos = new();
    private bool cambiandoNivel;

    private void Awake()
    {
        if (textoEntrar != null)
            textoEntrar.SetActive(false);

        if (fundido != null)
        {
            fundido.alpha = 0f;
            fundido.interactable = false;
            fundido.blocksRaycasts = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        RegistrarJugador(otro);
    }

    private void OnTriggerStay2D(Collider2D otro)
    {
        RegistrarJugador(otro);
    }

    private void RegistrarJugador(Collider2D otro)
    {
        if (otro.GetComponentInParent<PlayerVida>() != null)
            contactos.Add(otro);
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        contactos.Remove(otro);
    }

    private bool HayJugadorVivo()
    {
        foreach (Collider2D contacto in contactos)
        {
            if (contacto == null ||
                !contacto.enabled ||
                !contacto.gameObject.activeInHierarchy)
                continue;

            PlayerVida jugador =
                contacto.GetComponentInParent<PlayerVida>();

            if (jugador != null && !jugador.EstaMuerto)
                return true;
        }

        return false;
    }

    private void Update()
    {
        if (cambiandoNivel)
            return;

        bool puedeEntrar =
            Time.timeScale > 0f && HayJugadorVivo();

        if (textoEntrar != null)
            textoEntrar.SetActive(puedeEntrar);

        if (puedeEntrar && Input.GetKeyDown(KeyCode.E))
            IntentarCambiarNivel();
    }

    private void IntentarCambiarNivel()
    {
        if (fundido == null)
        {
            Debug.LogWarning("Asigna FundidoNivel en SalidaNivel.");
            return;
        }

        if (string.IsNullOrWhiteSpace(siguienteNivel) ||
            !Application.CanStreamedLevelBeLoaded(siguienteNivel))
        {
            Debug.LogWarning(
                $"No se puede cargar '{siguienteNivel}'. " +
                "Revisa su nombre y la Scene List."
            );
            return;
        }

        if (siguienteNivel == SceneManager.GetActiveScene().name)
        {
            Debug.LogWarning("El destino es la misma escena actual.");
            return;
        }

        cambiandoNivel = true;
        StartCoroutine(CambiarNivel());
    }

    private IEnumerator CambiarNivel()
    {
        if (textoEntrar != null)
            textoEntrar.SetActive(false);

        fundido.gameObject.SetActive(true);
        fundido.transform.SetAsLastSibling();
        fundido.blocksRaycasts = true;

        // Detiene el juego mientras se oscurece la pantalla.
        Time.timeScale = 0f;

        float tiempo = 0f;

        while (tiempo < duracionFundido)
        {
            tiempo += Time.unscaledDeltaTime;

            fundido.alpha = Mathf.Clamp01(
                tiempo / duracionFundido
            );

            yield return null;
        }

        fundido.alpha = 1f;

        GuardadoPartida.GuardarInicioNivel(siguienteNivel);

        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(siguienteNivel);
    }
}
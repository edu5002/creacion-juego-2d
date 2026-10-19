using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameOverUI : MonoBehaviour
{
    [Header("Referencias")]
    public CanvasGroup panel;
    public PlayerVida jugador;
    public Button botonReintentar;
    public Button botonMenu;

    [Header("Presentación")]
    public float duracionAparicion = 0.4f;

    [Header("Menú")]
    public string nombreEscenaMenu = "";

    private bool visible;
    private bool listo;
    private Coroutine animacion;

    private void Awake()
    {
        if (panel != null)
        {
            panel.alpha = 0f;
            panel.interactable = false;
            panel.blocksRaycasts = false;
            panel.gameObject.SetActive(false);
        }
    }

    public void Mostrar()
    {
        if (visible)
            return;

        if (panel == null)
        {
            Debug.LogError("Asigna el panel de Game Over.", this);
            return;
        }

        visible = true;
        listo = false;

        panel.gameObject.SetActive(true);
        panel.transform.SetAsLastSibling();
        panel.alpha = 0f;
        panel.interactable = false;
        panel.blocksRaycasts = true;

        if (botonMenu != null)
            botonMenu.interactable = MenuDisponible();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        animacion = StartCoroutine(Aparecer());
    }

    private IEnumerator Aparecer()
    {
        float duracion = Mathf.Max(0.01f, duracionAparicion);
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;
            panel.alpha = Mathf.Clamp01(tiempo / duracion);
            yield return null;
        }

        panel.alpha = 1f;
        panel.interactable = true;
        listo = true;
        animacion = null;

        if (EventSystem.current != null &&
            botonReintentar != null)
        {
            EventSystem.current.SetSelectedGameObject(
                botonReintentar.gameObject
            );
        }
    }

    public void Reintentar()
    {
        if (!visible || !listo || jugador == null ||
            !jugador.EstaMuerto)
            return;

        Ocultar();
        jugador.ReintentarDesdeCheckpoint();
    }

    public void IrAlMenu()
    {
        if (!visible || !listo || !MenuDisponible())
            return;

        Ocultar();
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    private bool MenuDisponible()
    {
        return !string.IsNullOrWhiteSpace(nombreEscenaMenu) &&
               Application.CanStreamedLevelBeLoaded(nombreEscenaMenu);
    }

    private void Ocultar()
    {
        if (animacion != null)
        {
            StopCoroutine(animacion);
            animacion = null;
        }

        visible = false;
        listo = false;

        panel.interactable = false;
        panel.blocksRaycasts = false;
        panel.alpha = 0f;
        panel.gameObject.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        Time.timeScale = 1f;
    }
}